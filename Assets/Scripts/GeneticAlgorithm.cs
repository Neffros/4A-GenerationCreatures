using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticAlgorithm : MonoBehaviour
{
    
    ushort legNumber = 3;
    float scaleX = 5f;
    float legCustomOffsetX = 10.5f;
    private float _legWantedDistance = 3f;
    private float _legSpreadAngle = 4f;

    bool[] InitBitArrayFromParams()
    {
        Byte[] bytesLegNumber = BitConverter.GetBytes(legNumber);
        Byte[] bytesScaleX = BitConverter.GetBytes(scaleX);
        Byte[] bytesLegCustomOffsetX = BitConverter.GetBytes(legCustomOffsetX);
        Byte[] bytesLegWantedDistance = BitConverter.GetBytes(_legWantedDistance);
        Byte[] bytesLegSpreadAngle = BitConverter.GetBytes(_legSpreadAngle);
        
        var bitsLegNumber = new BitArray(bytesLegNumber);
        var bitsScaleX = new BitArray(bytesScaleX);
        var bitsLegCustomOffsetX = new BitArray(bytesLegCustomOffsetX);
        var bitsLegWantedDistance = new BitArray(bytesLegWantedDistance);
        var bitsLegSpreadAngle = new BitArray(bytesLegSpreadAngle);
        
        var allBits = new bool[bitsLegNumber.Length + bitsScaleX.Length + bitsLegCustomOffsetX.Length + 
                               bitsLegWantedDistance.Length + bitsLegSpreadAngle.Length];
        int offset = 0;

        //ushort, float, float ,float , float
        allBits = AppendToAllBits(allBits, bitsLegNumber, offset);
        offset += bitsLegNumber.Length;

        allBits = AppendToAllBits(allBits, bitsScaleX, offset);
        offset += bitsScaleX.Length;

        allBits = AppendToAllBits(allBits, bitsLegCustomOffsetX, offset);
        offset += bitsLegCustomOffsetX.Length;

        allBits = AppendToAllBits(allBits, bitsLegWantedDistance, offset);
        offset += bitsLegWantedDistance.Length;
        
        allBits = AppendToAllBits(allBits, bitsLegSpreadAngle, offset);

        return allBits;
    }
    
    Byte[] ConvertBitsToBytes(bool[] allBits)
    {
        var length = allBits.Length / 8;
        Byte[] bytes = new byte[length];
        BitArray currBits = new BitArray(8);

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < 8; j++)
                currBits[j] = allBits[i * 8 + j];

            currBits.CopyTo(bytes, i);
        }

        return bytes;
    }

    bool[] AppendToAllBits(bool[] allBits, BitArray newBits, int offset)
    {
        newBits.CopyTo(allBits, offset);
        return allBits;
    }

    bool[] Tournament(bool[][] population, int[] scores, int steps)
    {
        int selected = Random.Range(0, population.Length);

        for (int i = 0; i < steps; i++)
        {
            int index = Random.Range(0, population.Length);

            if (scores[selected] < scores[index])
                selected = index;
        }

        return population[selected];
    }
    
    bool[][] Selection(bool[][] population, int[] scores, int selected)
    {
        bool[][] bestElements = new bool[population.Length][];

        for (int n = 0; n < selected; n++)
            bestElements[n] = Tournament(population, scores, 7);

        return bestElements;
    }

    int[] Evaluate(bool[][] population, bool[] bitsBaseProperties)
    {
        int[] creatureScores = new int[population.Length];

        for (int n = 0; n < population.Length; n++)
        {
            int score = 0;
            
            for (int i = 0; i < bitsBaseProperties.Length; i++)
            {
                if (population[n][i] == bitsBaseProperties[i])
                    score++;
            }
            creatureScores[n] = score;
        }

        return creatureScores;
    }

    bool[][] Mutation(bool[][] selectedCreatures, int mutationChance)
    {
        int randomVal = 0;
        for(int i = 0; i < selectedCreatures.Length; i++)
        {
            for (int j = 0; j < selectedCreatures[i].Length; j++)
            {
                randomVal = Random.Range(0, 101);
                if (mutationChance > randomVal)
                {
                    selectedCreatures[i][j] = !selectedCreatures[i][j];
                }
            }
        }

        return selectedCreatures;
    }

    bool[] Recombine(bool[] parent, bool[] parent2)
    {
        bool[] child = (bool[])parent.Clone();
        int line = Random.Range(1, parent.Length-1);

        for (int i = line; i < parent.Length; i++)
            child[i] = parent2[i];
        
        return child; 
    }
    bool[][] InitPopulation(int nbGeneratedCreatures, int dataSize)
    {
        bool[][] population = new bool[nbGeneratedCreatures][];

        for (int i = 0; i < nbGeneratedCreatures; i++)
        {
            population[i] = new bool[dataSize];

            for (int j = 0; j < dataSize; j++)
                population[i][j] = Random.Range(0, 2) != 0;
        }

        return population;
    }

    bool[] GenerateGenerations()
    {
        bool[] properties = InitBitArrayFromParams();
        bool[][] population = InitPopulation(40, properties.Length);
        int[] scores = Evaluate(population, properties);

        int rand;
        bool[] parent;
        bool[] parent2;
        
        for (int i = 1; i < 200; i++)
        {
            population = Selection(population, scores, 10);
          
            for (int j = 10; j < 40; j++)
            {
                rand = Random.Range(0, 10);
                parent = population[rand];
                rand = Random.Range(0, 10);
                parent2 = population[rand];
                population[j] = Recombine(parent, parent2);
            }

            Mutation(population, 10);
            scores = Evaluate(population, properties);
        }

        return Selection(population, scores, 1)[0];
    }


  
    // Start is called before the first frame update
    void Start()
    {
        bool[] allBits = InitBitArrayFromParams();
        Byte[] bytes = ConvertBitsToBytes(allBits);
        float test = BitConverter.ToSingle(bytes, 2);
        Byte[] f = BitConverter.GetBytes(this.scaleX);
        float r = BitConverter.ToSingle(f, 0);

        bool[] element = GenerateGenerations();

        Byte[] currByteCreature = ConvertBitsToBytes(element);

        ushort legNumber = BitConverter.ToUInt16(currByteCreature, 0);
        float scaleX = BitConverter.ToSingle(currByteCreature, 2);
        float legCustomOffsetX = BitConverter.ToSingle(currByteCreature, 6);
        float _legWantedDistance = BitConverter.ToSingle(currByteCreature, 10);
        float _legSpreadAngle = BitConverter.ToSingle(currByteCreature, 14);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}