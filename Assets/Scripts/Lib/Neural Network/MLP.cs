using System.Collections.Generic;
using UnityEngine;
public sealed class MLP<TInput, TOutput>
{
    private int[] args;
    private List<int>[] layers;
    private List<float> weights;
    private int[] m;

    public MLP(params int[] args)
    {
        this.args = args;
        this.layers = new List<int>[args.Length];

        for (int i = 0; i < args.Length; i++)
        {
            layers[i] = new List<int>(args[i] + (i == 0 ? 1 : 0));
            for (int j = 0; j < this.layers[i].Count; j++)
                layers[i][j] = 1;
        }

        this.weights = new List<float>();
        for(int j = 0; j < args.Length - 1; j++)
            this.weights.Add((2 * Random.Range(this.layers[j].Count, this.layers[j + 1].Count) - 1) * .2f);

        for(int j = 0; j < this.weights.Count; j++)
            this.m[j] = 0;
    }

    public void Update(object inputs)
    {
        int p = layers[0][layers[0].Count - 1];
        layers[0].Clear();
        layers[0].
        // [[0, 0], 0]
    }
}
