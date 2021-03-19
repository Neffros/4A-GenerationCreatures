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
    int offset = 0;

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
        
        var allBits = new bool[bitsScaleX.Length + bitsLegNumber.Length + bitsLegCustomOffsetX.Length + 
                               bitsLegWantedDistance.Length + bitsLegSpreadAngle.Length];

        //ushort, float, float ,float , float 
        allBits = AppendToAllBits(allBits, bitsScaleX);
        allBits = AppendToAllBits(allBits, bitsLegNumber);
        allBits = AppendToAllBits(allBits, bitsLegCustomOffsetX);
        allBits = AppendToAllBits(allBits, bitsLegWantedDistance);
        allBits = AppendToAllBits(allBits, bitsLegSpreadAngle);

        return allBits;
    }
    
    Byte[] ConvertBitsToBytes(bool[] allBits)
    {
        Byte[] bytes = new byte[allBits.Length/8];
        var currByte = new byte[1];
        BitArray currBits = new BitArray(8);

        for (int i = 0, k = 0; i < allBits.Length; i+=8, k++)
        {
            for (int j = 0; j < 8; j++)
            {
                currBits[j] = allBits[i + j];
            }

            currBits.CopyTo(currByte, 0);

            currByte.CopyTo(bytes, k);
        }
        return bytes;
    }

    bool[] AppendToAllBits(bool[] allBits, BitArray newBits)
    {
        newBits.CopyTo(allBits, offset);
        offset += newBits.Length;
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
            bestElements[n] = Tournament(population, scores, 4);

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

    void Mutation(bool[][] selectedCreatures)
    {
        
    }

    void Recombine(BitArray[] g)
    {
        
    }
    bool[][] InitPopulation(int nbGeneratedCreatures, int dataSize)
    {
        bool[][] population = new bool[nbGeneratedCreatures][];


        for (int i = 0; i < nbGeneratedCreatures; i++)
        {
            population[i] = new bool[dataSize];
            for (int j = 0; j < dataSize; j++)
            {
                population[i][j] = Random.Range(0, 2) != 0;
            }
        }
        /*for (int i = 0; i < population[0].Length; i++)
        {
            Debug.Log(population[0][i]);
        }
        
        Debug.Log("size: " +  population[0].Length);*/
        return population;
    }

    BitArray[] GenerateGenerations()
    {
        bool[] properties = InitBitArrayFromParams();
        bool[][] population = InitPopulation(20, properties.Length);
        int[] scores = Evaluate(population, properties);
        population = Selection(population, scores, 5);

        for (int j = 0; j < population.Length; j++)
        {
            for (int i = 0; i < population[j].Length; i++)
                Debug.Log(population[j][i]);
            Debug.Log("size: " +  population[j].Length);
        }

        /*for (int i = 1; i < 50; i++)
        {
            population = Selection(population, scores, 5);
            //Recombine(i)
            //Mutation(gen)
            scores = Evaluate(population, properties);
        }*/
        return null;
    }


  
    // Start is called before the first frame update
    void Start()
    {
        GenerateGenerations();
        /*bool[] allBits = InitBitArrayFromParams();
        for (int i = 0; i < allBits.Length; i++)
        {
            Debug.Log(allBits[i]);
        }
        Debug.Log("size total: " + allBits.Length);
        Byte[] bytes = ConvertBitsToBytes(allBits);

        float test = BitConverter.ToSingle(bytes, 10);
        Debug.Log(test);
        //GenerateGenerations()

        /*float scaleX = 5f;
        ushort legNumber = 3;
        float legCustomOffsetX = 10.5f;
        //Byte yannis = 1;
        Byte[] bytesScaleX = BitConverter.GetBytes(scaleX);
        Byte[] byteLegNumber = BitConverter.GetBytes(legNumber);
        Byte[] byteLegCustomOffsetX = BitConverter.GetBytes(legCustomOffsetX);
        
        //get specific bits of bytearray
        var bits = new BitArray(bytesScaleX);
        /*for (int i = 0; i < bits.Length; i++)
        {
            Debug.Log(bits[i]);
        }
        Debug.Log("size float:" + bits.Length);
        */
        //var bitsShort = new BitArray(byteLegNumber);
        /* for (int i = 0; i < bitsShort.Length; i++)
         {
             Debug.Log(bitsShort[i]);
         }
         Debug.Log("size short: " + bitsShort.Length);
         */
        //concatenate bytes to bit array
        /*var allBits = new bool[bits.Length + bitsShort.Length];

        int offset = 0;
        bits.CopyTo(allBits, offset);
        offset += bits.Length;
        bitsShort.CopyTo(allBits, offset);
        offset += bitsShort.Length;
        for (int i = 0; i < allBits.Length; i++)
        {
            Debug.Log(allBits[i]);
        }
        Debug.Log("size total: " + allBits.Length);*/

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}