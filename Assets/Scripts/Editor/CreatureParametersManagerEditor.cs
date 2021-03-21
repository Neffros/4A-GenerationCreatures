using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreatureParametersManager))]
public class CreatureParametersManagerEditor : Editor
{
    #region Fonctions publiques

    public override void OnInspectorGUI()
    {
        CreatureParametersManager instance = (CreatureParametersManager)this.target;

        this.DrawDefaultInspector();

        // TODO: don't draw default inspector and manage all inputs here
        // If instance scriptable object (_customData) is not null, bind inputs to it
        // Add inputs for genetic algorithm parameters and scriptable object management
        
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
