using UnityEngine;

[CreateAssetMenu(fileName = "Parameters", menuName = "Create Creature config file", order = 1)]
public class Parameters : ScriptableObject
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
        set => this._legAutoDistance= value;
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
}
