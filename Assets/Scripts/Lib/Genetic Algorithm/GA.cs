using UnityEngine;

public static class GA
{
    /// <summary>
    /// Takes two genotypes randomly and compares their scores, the winner stays
    /// and another genotype is taken to compete with the winner
    /// </summary>
    /// <param name="population">The current population</param>
    /// <param name="scores">The scores of the population</param>
    /// <param name="rounds">The number of times it compares</param>
    /// <returns>The winner of the tournament</returns>
    static Genotype Tournament(Genotype[] population, int[] scores, int rounds)
    {
        int selected = Random.Range(0, population.Length);

        for (int i = 0; i < rounds; i++)
        {
            int index = Random.Range(0, population.Length);

            if (scores[selected] < scores[index])
                selected = index;
        }

        return population[selected];
    }

    /// <summary>
    /// Selects genotypes in the population by doing tournaments
    /// </summary>
    /// <param name="population">The current population</param>
    /// <param name="scores">The scores of the population</param>
    /// <param name="tournaments">Each tournament selects one winner</param>
    /// <param name="rounds">The number of rounds per tournament</param>
    /// <returns>The selected genotypes</returns>
    static Genotype[] Selection(Genotype[] population, int[] scores, int tournaments, int rounds)
    {
        Genotype[] selectedGenotypes = new Genotype[population.Length];

        for (int n = 0; n < tournaments; n++)
            selectedGenotypes[n] = GA.Tournament(population, scores, rounds);

        return selectedGenotypes;
    }

    /// <summary>
    /// Computes the scores of the given population according
    /// to the wanted genotype
    /// </summary>
    /// <param name="population">The current population</param>
    /// <param name="genotype">The genotype to compare an element with</param>
    /// <returns>The scores of the population</returns>
    static int[] Evaluate(Genotype[] population, Genotype genotype)
    {
        int[] scores = new int[population.Length];

        for (int n = 0; n < population.Length; n++)
            scores[n] = Genotype.CountSameBits(population[n], genotype);

        return scores;
    }

    /// <summary>
    /// Each gene of the genotype of the population has a chance to mutate
    /// </summary>
    /// <param name="genotype">An element of the population</param>
    /// <param name="mutationRate">Between 0 and 1, the probability for a gene to mutate</param>
    /// <returns>The possibly mutated genotype</returns>
    static Genotype Mutation(Genotype genotype, float mutationRate)
    {
        for (int i = 0; i < genotype.Length; i++)
        {
            if (Random.Range(0f, 1f) <= mutationRate)
                genotype[i] = !genotype[i];
        }

        return genotype;
    }

    /// <summary>
    /// Generates a child from two parents by cutting two parents
    /// at a random position and recombining them
    /// </summary>
    /// <param name="parentA">An element of the population</param>
    /// <param name="parentB">An element of the population</param>
    /// <returns>A newly created element of the population</returns>
    static Genotype Recombine(Genotype parentA, Genotype parentB)
    {
        Genotype child = new Genotype(parentA);

        for (int i = Random.Range(1, parentA.Length - 1); i < parentA.Length; i++)
            child[i] = parentB[i];

        return child;
    }

    /// <summary>
    /// Creates a randomly generated population of genotypes
    /// </summary>
    /// <param name="populationSize">The number of genotypes to generate</param>
    /// <param name="genotypeSize">The size of a genotype</param>
    /// <returns>A newly generated population</returns>
    static Genotype[] InitPopulation(int populationSize, int genotypeSize)
    {
        Genotype[] population = new Genotype[populationSize];

        for (int n = 0; n < populationSize; n++)
        {
            population[n] = new Genotype(genotypeSize);

            for (int i = 0; i < genotypeSize; i++)
                population[n][i] = Random.Range(0, 2) != 0;
        }

        return population;
    }

    /// <summary>
    /// Computes a genetic algorithm with given parameters
    /// </summary>
    /// <param name="wantedGenotype">The genotype to fit around</param>
    /// <param name="generations">Number of generations</param>
    /// <param name="populationSize">Size of a generation's population</param>
    /// <param name="tournaments">Number of tournaments in selection</param>
    /// <param name="rounds">Number of rounds in each tournament</param>
    /// <param name="mutationRate">Between 0 and 1, chance to mutate a gene</param>
    /// <returns>A genotype of the last generation's population</returns>
    public static Genotype Generate(
        Genotype wantedGenotype,
        int generations = 300,
        int populationSize = 150,
        int tournaments = 100,
        int rounds = 10,
        float mutationRate = 0.01f
    )
    {
        Genotype[] population = GA.InitPopulation(populationSize, wantedGenotype.Length);
        int[] scores = GA.Evaluate(population, wantedGenotype);

        for (int g = 1; g < generations; g++)
        {
            population = GA.Selection(population, scores, tournaments, rounds);

            for (int n = tournaments; n < populationSize; n++)
                population[n] = GA.Recombine(
                    population[Random.Range(0, n)],
                    population[Random.Range(0, n)]
                );

            for (int n = 0; n < populationSize; n++)
                population[n] = GA.Mutation(population[n], mutationRate);

            scores = GA.Evaluate(population, wantedGenotype);
        }

        return GA.Selection(population, scores, 1, rounds)[0];
    }
}