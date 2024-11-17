#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour {
    
    private string bundleUrl = $"{Application.streamingAssetsPath}/test";
    private string sceneName = "test_stage";
    private AssetBundle bundle;

    [SerializeField] private bool isApplyLoadURPSetting;
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            StartCoroutine(LoadSceneCoroutine());
        }
        
        if (Input.GetKeyDown(KeyCode.S)) {
            ApplyURPSettings();
        }
        
        if (Input.GetKeyDown(KeyCode.D)) {
            QualitySettings.renderPipeline = null;
        }
    }
    
    private IEnumerator LoadSceneCoroutine() {
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundleUrl);
        yield return bundleRequest;

        bundle = bundleRequest.assetBundle;
        
        if (bundle == null) {
            Debug.LogError($"Failed to load AssetBundle from {bundleUrl}");
            yield break;
        }


        if (!bundle.isStreamedSceneAssetBundle) {
            Debug.LogError($"The AssetBundle does not contain scenes: {bundleUrl}");
            bundle.Unload(false);
            yield break;
        }

        string[] scenePaths = bundle.GetAllScenePaths();
        if (scenePaths.Length == 0 || !scenePaths[0].EndsWith($"{sceneName}.unity")) {
            Debug.LogError($"Scene {sceneName} not found in AssetBundle: {bundleUrl}");
            bundle.Unload(false);
            yield break;
        }
        
        foreach (string path in scenePaths)
        {
            Debug.Log($"Scene in bundle: {path}");
        }
        
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return loadOperation;

        Debug.Log($"Scene {sceneName} loaded Additively!");
        
        if (isApplyLoadURPSetting) ApplyURPSettings();
    }

    private void ApplyURPSettings() {
        var urpAssetSettings = FindObjectOfType<URPAssetSetting>();
        if (urpAssetSettings == null || urpAssetSettings.urpAsset == null) {
            Debug.Log("URP Assetが見つかりませんでした");
            return;
        }
        QualitySettings.renderPipeline = urpAssetSettings.urpAsset;
        Debug.Log("Apply URP Settings");
    }

    private void OnDestroy() {
        bundle?.Unload(false);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(AssetBundleLoader))]
public class AssetBundleLoaderEditor : Editor {
    

}
#endif