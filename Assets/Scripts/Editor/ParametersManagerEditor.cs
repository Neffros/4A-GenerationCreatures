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
        
        GUILayout.Space(5);

        if (GUILayout.Button("Save data to ScriptableObject"))
            instance.SaveData();
        if (GUILayout.Button("Load data from ScriptableObject"))
            instance.LoadData();
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Generate creatures from given parameters"))
            instance.GenerateCreatures();
        if (GUILayout.Button("Generate creatures from genetic algorithm with given parameters"))
            instance.ComputeGeneticAlgorithm();
    }
    #endregion
}
