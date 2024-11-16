using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour {
    
    private string bundleUrl = $"{Application.streamingAssetsPath}/test";
    private string sceneName = "test_stage";
    private AssetBundle bundle;
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            StartCoroutine(LoadSceneCoroutine());
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