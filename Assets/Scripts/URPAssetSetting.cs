using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR 
using UnityEditor;
#endif

public class URPAssetSetting : MonoBehaviour {
    public UniversalRenderPipelineAsset urpAsset;
    public UniversalRendererData urpData;
}

#if UNITY_EDITOR
[CustomEditor(typeof(URPAssetSetting))]
public class URPAssetSettingEditor : Editor {
    private URPAssetSetting urpAssetSetting;
    // private Editor urpDataEditor;
    
    private void OnEnable() {
        urpAssetSetting = (URPAssetSetting)target;
        // urpDataEditor = null;
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.BeginHorizontal();
        
        urpAssetSetting.urpAsset = (UniversalRenderPipelineAsset)EditorGUILayout.ObjectField(
            "URP Asset", 
            urpAssetSetting.urpAsset, 
            typeof(UniversalRenderPipelineAsset), 
            true
        );
        
        if (GUILayout.Button("New", GUILayout.Width(60))) {
            var defaultUrpAsset = Resources.Load<UniversalRenderPipelineAsset>("Default URP Asset");
            urpAssetSetting.urpAsset = Instantiate(defaultUrpAsset);
        
            var defaultUrpData = Resources.Load<UniversalRendererData>("Default URP Data");
            urpAssetSetting.urpData = Instantiate(defaultUrpData);
            urpAssetSetting.urpAsset.SetRendererData(urpAssetSetting.urpData);
        }
        
        EditorGUILayout.EndHorizontal();
        
        var editorAsset = CreateEditor(urpAssetSetting.urpAsset);
        if (editorAsset!= null) {
            editorAsset.OnInspectorGUI();
            DestroyImmediate(editorAsset);
        }
        
        
        // UniversalRendererDataEditorでCreateEditorするとUnityエディタのバグでEditorインスタンスが削除できなくなり
        // 実行時にSerializedObjectNotCreatableException: Object at index 0 is nullエラーが発生するため使用できない
        // DestroyImmediate()でインスタンスを削除するとエラーは回避できるがRendererFeatureListがクリアされる
        // https://discussions.unity.com/t/im-getting-now-error-serializedobjectnotcreatableexception-object-at-index-0-is-null-how-to-fix/788961
        
        // GUILayout.Space(10);
        // EditorGUILayout.LabelField("URP Data", EditorStyles.boldLabel);
        // EditorGUI.indentLevel++;
        //
        // if (urpAssetSetting.urpData != null) {
        //     urpDataEditor = CreateEditor(urpAssetSetting.urpData);
        //     urpDataEditor.OnInspectorGUI();
        // }
        
        if (GUILayout.Button("Show URP Data")) {
            URPDataWindow.Open(CreateEditor(urpAssetSetting.urpData));
        }
    }

    private void OnValidate() {
        Debug.Log("OnValidate");
    }

    private void OnDestroy() {
        Debug.Log("OnDestroy");
    }

    private void OnDisable() {
        Debug.Log("OnDisable");
    }
}

internal class URPDataWindow : EditorWindow {
    
    private Editor urpDataEditor;
    
    public static void Open(Editor urpDataEditor)
    {
        var window = GetWindow<URPDataWindow>("URP Data");
        window.urpDataEditor = urpDataEditor;
    }

    private void OnGUI()
    {
        EditorGUI.indentLevel++;
        EditorGUIUtility.labelWidth = 250;
        urpDataEditor.OnInspectorGUI();
    }

    private void OnDestroy() {
        DestroyImmediate(urpDataEditor);
    }
}

public static class UniversalRenderPipelineAssetExtensions
{
    public static void SetRendererData(this UniversalRenderPipelineAsset urpAsset, ScriptableRendererData rendererData)
    {
        var field = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null) return;
        
        var rendererDataList = (ScriptableRendererData[])field.GetValue(urpAsset);
        rendererDataList[0] = rendererData;
        field.SetValue(urpAsset, rendererDataList);
    }
}
#endif

