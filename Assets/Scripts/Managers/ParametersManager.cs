using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ParametersManager : MonoBehaviour
{
    #region Variables statiques

    private static ParametersManager _instance = null;

    #endregion

    #region Propriétés statiques

    public static ParametersManager Instance => ParametersManager._instance;

    #endregion

    #region Variables Unity

    [SerializeField]
    private string _blenderExecPath = @"D:\Applications\Blender\2.92\blender.exe";

    [SerializeField]
    private string _scriptTemplatePath = @"D:\Projects\Unity\4A-GenerationCreatures\Blender\template.py";

    [SerializeField]
    private string _BATTemplatePath = @"D:\Projects\Unity\4A-GenerationCreatures\Blender\template.bat";

    [Space]
    [SerializeField]
    private Vector3 _scale = Vector3.zero;

    [SerializeField]
    private ushort _legNumber = 0;

    [SerializeField]
    private bool _legAutoDistance = true;

    [SerializeField]
    private float _legWantedDistance = 0f;

    [SerializeField]
    private float _legSpreadAngle = 0f;
   
    [SerializeField]
    private Vector3 _legCustomOffset = Vector3.zero;
    
    [Space]
    [SerializeField]
    private int _creatureNb = 10;

    [SerializeField]
    private string _fileName = "CreatureParameters";

    [SerializeField]
    private Creature _customData = null;

    #endregion

    #region Fonctions Unity

    private void Awake()
    {
        ParametersManager._instance = this;
    }

    private void OnDestroy()
    {
        if (ParametersManager._instance == this)
            ParametersManager._instance = null;
    }

    #endregion

    #region Fonctions publiques

    public void LoadData()
    {
        this.Load(this._customData);
    }

    public void SaveData()
    {
        Creature asset = ScriptableObject.CreateInstance<Creature>();

        asset.Scale = this._scale;
        asset.LegNumber = this._legNumber;
        asset.LegAutoDistance = this._legAutoDistance;
        asset.LegWantedDistance = this._legWantedDistance;
        asset.LegSpreadAngle = this._legSpreadAngle;
        asset.LegCustomOffset = this._legCustomOffset;

        asset.Save(this._fileName);
    }

    public void GenerateCreatures()
    {
        for (int i = 0; i < this._creatureNb; i++)
        {
            GameObject creature = this.GenerateMesh();
            creature.transform.position = new Vector3(i * 10, 0, 0);
        }
    }

    public void ComputeGeneticAlgorithm()
    {
        this.Load(this._customData.ComputeGeneticAlgorithm());
    }

    #endregion

    #region Fonctions privées

    private void Load(Creature data)
    {
        if (data != null)
        {
            this._scale = data.Scale;
            this._legNumber = data.LegNumber;
            this._legAutoDistance = data.LegAutoDistance;
            this._legWantedDistance = data.LegWantedDistance;
            this._legSpreadAngle = data.LegSpreadAngle;
            this._legCustomOffset = data.LegCustomOffset;
        }
    }

    private GameObject GenerateMesh()
    {
        string name = Guid.NewGuid().ToString();

        this.GenerateScript(name);
        this.GenerateBatFile(name);
        return this.CreateAndImportMesh(name);
    }

    private void GenerateScript(string name)
    {
        if (File.Exists(this._scriptTemplatePath))
        {
            string contents = File.ReadAllText(this._scriptTemplatePath);

            Debug.Log(Path.Combine(Application.persistentDataPath, string.Concat("script-", name, ".fbx")));

            contents = contents
                .Replace("LEG_NUMBER", this._legNumber.ToString())
                .Replace("LEG_AUTO_DISTANCE", this._legAutoDistance.ToString())
                .Replace("LEG_WANTED_DISTANCE", this._legWantedDistance.ToString())
                .Replace("LEG_SPREAD_ANGLE", this._legSpreadAngle.ToString())
                .Replace("SCALE_X", this._scale.x.ToString())
                .Replace("SCALE_Y", this._scale.y.ToString())
                .Replace("SCALE_Z", this._scale.z.ToString())
                .Replace("LEG_CUSTOM_OFFSET_X", this._legCustomOffset.x.ToString())
                .Replace("LEG_CUSTOM_OFFSET_Y", this._legCustomOffset.y.ToString())
                .Replace("LEG_CUSTOM_OFFSET_Z", this._legCustomOffset.z.ToString())
                .Replace("FBX_EXPORT_PATH", Path.Combine(Application.dataPath, "Prefabs", string.Concat("creature-", name, ".fbx")));

            File.WriteAllText(Path.Combine(Application.persistentDataPath, string.Concat("script-", name, ".py")), contents);
        }
    }

    private void GenerateBatFile(string name)
    {
        if (File.Exists(this._BATTemplatePath))
        {
            string contents = File.ReadAllText(this._BATTemplatePath);

            contents = contents
                .Replace("BLENDER_EXEC", this._blenderExecPath)
                .Replace("SCRIPT_PATH", Path.Combine(Application.persistentDataPath, string.Concat("script-", name, ".py")));

            File.WriteAllText(Path.Combine(Application.persistentDataPath, string.Concat("exec-", name, ".bat")), contents);
        }
    }

    private GameObject CreateAndImportMesh(string name)
    {
        try
        {
            ProcessManager.Launch(Path.Combine(Application.persistentDataPath, string.Concat("exec-", name, ".bat")));

            AssetDatabase.ImportAsset(string.Concat("Assets/Prefabs/", "creature-", name, ".fbx"));
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(string.Concat("Assets/Prefabs/", "creature-", name, ".fbx"));

            if (asset)
                return GameObject.Instantiate(asset);
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    #endregion
}
