using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FishHealth))]
public class FishHealthEditor : Editor
{
    private Editor EditorInstance;

    private void OnEnable()
    {
        EditorInstance = null;
    }

    public override void OnInspectorGUI()
    {
        FishHealth fishHealth = (FishHealth)target;

        if (EditorInstance == null)
            EditorInstance = Editor.CreateEditor(fishHealth.Config);

        base.OnInspectorGUI();

        EditorInstance.DrawDefaultInspector();

    }
}
