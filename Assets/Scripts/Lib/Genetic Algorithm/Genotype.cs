public class Genotype
{
    #region Variables d'instance

    private readonly int _length;

    private readonly byte[] _values;

    #endregion

    #region Propriétés

    public int Length => this._length;

    public byte[] Values => this._values;

    #endregion

    #region Constructeurs

    public Genotype(int length)
    {
        this._length = length;
        this._values = new byte[length / 8];
    }

    public Genotype(byte[] array)
    {
        this._length = array.Length * 8;
        this._values = (byte[])array.Clone();
    }

    public Genotype(Genotype original)
    {
        this._length = original._length;
        this._values = (byte[])original._values.Clone();
    }

    #endregion

    #region Opérateurs

    public bool this[int pos]
    {
        get
        {
            return (this._values[pos / 8] & ((byte)(1 << pos % 8))) != 0;
        }
        set
        {
            if (this[pos] != value)
                this._values[pos / 8] ^= (byte)(1 << pos % 8);
        }
    }

    #endregion

    #region Fonctions statiques publiques

    public static int CountSameBits(Genotype a, Genotype b)
    {
        int length = a._length > b._length ? b._length : a._length;
        int count = 0;

        for (int i = 0; i < length; i++)
        {
            if (a[i] == b[i])
                count++;
        }

        return count;
    }

    #endregion
}
