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
public class URPAssetSettingEditor : Editor
{
    public override void OnInspectorGUI() {
        URPAssetSetting urpAssetSetting = (URPAssetSetting)target;
        
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
        
        GUILayout.Space(10);
        GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("URP Data", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        
        var editorData = CreateEditor(urpAssetSetting.urpData);
        if (editorData != null) {
            editorData.OnInspectorGUI();
            DestroyImmediate(editorData);
        }
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

