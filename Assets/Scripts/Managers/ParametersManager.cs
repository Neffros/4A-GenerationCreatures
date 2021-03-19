using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ParametersManager : MonoBehaviour
{
    #region Variables statiques

    private static readonly string _scriptPath = @"E:\Unity\4A-GenerationCreatures\Blender\script.py";

    private static readonly string _BATPath = @"E:\Unity\4A-GenerationCreatures\Blender\exec.bat";

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
    private string _fileName = "CreatureParameters";

    [SerializeField]
    private Parameters _data = null;

    #endregion

    #region Fonctions publiques

    public void LoadData()
    {
        if (this._data != null)
        {
            this._scale = this._data.Scale;
            this._legNumber = this._data.LegNumber;
            this._legAutoDistance = this._data.LegAutoDistance;
            this._legWantedDistance = this._data.LegWantedDistance;
            this._legSpreadAngle = this._data.LegSpreadAngle;
            this._legCustomOffset = this._data.LegCustomOffset;
        }
    }

    public void SaveData()
    {
        Parameters asset = ScriptableObject.CreateInstance<Parameters>();

        asset.Scale = this._scale;
        asset.LegNumber = this._legNumber;
        asset.LegAutoDistance = this._legAutoDistance;
        asset.LegWantedDistance = this._legWantedDistance;
        asset.LegSpreadAngle = this._legSpreadAngle;
        asset.LegCustomOffset = this._legCustomOffset;

        AssetDatabase.CreateAsset(asset, string.Concat("Assets/Data/", this._fileName, ".asset"));
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    public void GenerateMesh()
    {
        this.GenerateScript();
        this.GenerateBatFile();
        this.ImportMesh();
    }

    #endregion

    #region Fonctions privées

    private void GenerateScript()
    {
        if (File.Exists(this._scriptTemplatePath))
        {
            string contents = File.ReadAllText(this._scriptTemplatePath);

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
                .Replace("FBX_EXPORT_PATH", Path.Combine(Application.dataPath, "Prefabs/creature.fbx"));

            File.WriteAllText(ParametersManager._scriptPath, contents);
        }
    }

    private void GenerateBatFile()
    {
        if (File.Exists(this._BATTemplatePath))
        {
            string contents = File.ReadAllText(this._BATTemplatePath);

            contents = contents
                .Replace("BLENDER_EXEC", this._blenderExecPath)
                .Replace("SCRIPT_PATH", ParametersManager._scriptPath);

            File.WriteAllText(ParametersManager._BATPath, contents);
        }
    }

    private void ImportMesh()
    {
        try
        {
            ProcessManager.Launch(ParametersManager._BATPath);

            AssetDatabase.ImportAsset("Assets/Prefabs/creature.fbx");
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/creature.fbx");

            if (asset)
                GameObject.Instantiate(asset);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    #endregion
}
