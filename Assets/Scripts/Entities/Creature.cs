using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data/Creature", menuName = "Create Creature config file", order = 1)]
public class Creature : ScriptableObject
{
    #region Variables Unity

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

    #endregion

    #region Propriétés

    public Vector3 Scale
    {
        get => this._scale;
        set => this._scale = value;
    }

    public ushort LegNumber
    {
        get => this._legNumber;
        set => this._legNumber = value;
    }

    public bool LegAutoDistance
    {
        get => this._legAutoDistance;
        set => this._legAutoDistance = value;
    }

    public float LegWantedDistance
    {
        get => this._legWantedDistance;
        set => this._legWantedDistance = value;
    }

    public float LegSpreadAngle
    {
        get => this._legSpreadAngle;
        set => this._legSpreadAngle = value;
    }

    public Vector3 LegCustomOffset
    {
        get => this._legCustomOffset;
        set => this._legCustomOffset = value;
    }

    #endregion

    #region Opérateurs

    public static implicit operator Creature(Genotype data)
    {
        byte[] bytes = data.Values;
        Creature c = ScriptableObject.CreateInstance<Creature>();

        c.Scale = new Vector3(
            BitConverter.ToSingle(bytes, 0),
            BitConverter.ToSingle(bytes, 4),
            BitConverter.ToSingle(bytes, 8)
        );
        c.LegNumber = BitConverter.ToUInt16(bytes, 12);
        c.LegAutoDistance = BitConverter.ToBoolean(bytes, 14);
        c.LegWantedDistance = BitConverter.ToSingle(bytes, 15);
        c.LegSpreadAngle = BitConverter.ToSingle(bytes, 19);
        c.LegCustomOffset = new Vector3(
            BitConverter.ToSingle(bytes, 23),
            BitConverter.ToSingle(bytes, 27),
            BitConverter.ToSingle(bytes, 31)
        );

        return c;
    }

    public static implicit operator Genotype(Creature data)
    {
        byte[] scaleX = BitConverter.GetBytes(data.Scale.x);
        byte[] scaleY = BitConverter.GetBytes(data.Scale.y);
        byte[] scaleZ = BitConverter.GetBytes(data.Scale.z);
        byte[] legNumber = BitConverter.GetBytes(data.LegNumber);
        byte[] legAutoDistance = BitConverter.GetBytes(data.LegAutoDistance);
        byte[] legWantedDistance = BitConverter.GetBytes(data.LegWantedDistance);
        byte[] legSpreadAngle = BitConverter.GetBytes(data.LegSpreadAngle);
        byte[] legCustomOffsetX = BitConverter.GetBytes(data.LegCustomOffset.x);
        byte[] legCustomOffsetY = BitConverter.GetBytes(data.LegCustomOffset.y);
        byte[] legCustomOffsetZ = BitConverter.GetBytes(data.LegCustomOffset.z);

        byte[] result = new byte[35];

        scaleX.CopyTo(result, 0);
        scaleY.CopyTo(result, 4);
        scaleZ.CopyTo(result, 8);
        legNumber.CopyTo(result, 12);
        legAutoDistance.CopyTo(result, 14);
        legWantedDistance.CopyTo(result, 15);
        legSpreadAngle.CopyTo(result, 19);
        legCustomOffsetX.CopyTo(result, 23);
        legCustomOffsetY.CopyTo(result, 27);
        legCustomOffsetZ.CopyTo(result, 31);

        return new Genotype(result);
    }

    #endregion

    #region Fonctions publiques

    public void Save(string name)
    {
        AssetDatabase.CreateAsset(this, string.Concat("Assets/Data/", name, ".asset"));
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = this;
    }

    public Creature ComputeGeneticAlgorithm()
    {
        // TODO: Give genetic algorithm custom parameters
        // Manage all of this in the editor-related script
        return GA.Generate(this);
    }

    #endregion
}
