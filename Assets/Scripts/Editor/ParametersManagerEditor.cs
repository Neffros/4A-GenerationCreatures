using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParametersManager))]
public class ParametersManagerEditor : Editor
{
    #region Fonctions publiques

    public override void OnInspectorGUI()
    {
        ParametersManager instance = (ParametersManager)this.target;

        this.DrawDefaultInspector();

        if (GUILayout.Button("Save data to ScriptableObject"))
            instance.SaveData();
        if (GUILayout.Button("Load data from ScriptableObject"))
            instance.LoadData();
        if (GUILayout.Button("Generate creature"))
            instance.GenerateMesh();
    }

    #endregion
}
