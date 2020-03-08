using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAglorithm<T>
{

    public List<DNA<T>> Population { get; private set; }
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public T[] BestGenes { get; private set; }

    public List<DNA<T>> aboveAverageGenes;

    DNA<T> best;
    int parentChoice = 0;
    int bestDNAIndex = 0;
    int completedCrossovers = 0;
    public int MutationRate;
    float averageDNAFitness = 0;
    private System.Random random;
    private float fitnessSum;
    public int selectionProcess = 0;
    int id;

    public GeneticAglorithm(int ID, int populationSize, int dnaSize, int selectionChoice, System.Random random, Func<float, T> getRandomGene, Func<int, T> getInitGene, Func<int, int, float> fitnessFunction, int mutationRate = 10)
    {
        Generation = 1;
        MutationRate = mutationRate;
        selectionProcess = selectionChoice;
        Population = new List<DNA<T>>();
        aboveAverageGenes = new List<DNA<T>>();
        this.random = random;
        id = ID;
        BestGenes = new T[dnaSize];

            
        for (int i = 0; i < 10; i++)
        {
            Population.Add(new DNA<T>(i, dnaSize, random, getRandomGene, getInitGene, fitnessFunction, shouldInitGenes: true));
        }
    }

    public void NewGeneration()
    {
        
        if (Population.Count <= 0)
        {
            return;
        }

        CalculateFitness();

        List<DNA<T>> newPopulation = new List<DNA<T>>();

        if (selectionProcess == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                //DNA<T> parent1 = ChooseRandomParent();
                //DNA<T> parent2 = ChooseRandomParent();

                DNA<T> parent1 = ChooseParent(-1);
                DNA<T> parent2 = ChooseParent(parentChoice);
                parentChoice = 0;

                DNA<T> child = parent1.Crossover(parent2);

                child.Mutate(MutationRate);

                newPopulation.Add(child);
                Debug.Log("RandomParent");
            }

        }
        else if (selectionProcess == 1)
        {
            Population.Sort((DNA<T> a, DNA<T> b) =>
            {
                return a.Fitness > b.Fitness ? -1 : 1;
            });

            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    DNA<T> child = HigherThanMedianSelection(i);
                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                    Debug.Log("MedianParent");
                }
            }
        }

        else if (selectionProcess == 2)
        {
            Population.Sort((DNA<T> a, DNA<T> b) =>
            {
                return a.Fitness > b.Fitness ? -1 : 1;
            });
            List<DNA<T>> betterThenMedian = new List<DNA<T>>(Population);
            betterThenMedian.RemoveRange(Population.Count / 2, Population.Count / 2);

            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    DNA<T> parent1 = best;
                    //DNA<T> parent2 = FittestParentCrossoverSelection(i);
                    DNA<T> parent2 = betterThenMedian[i];

                    DNA<T> child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                    Debug.Log("BestParent");
                }
            }
        }

        /*else if(SelectionProcess == 3){
            //DNA<T> parent1 = best;
            //DNA<T> parent2 = betterThenMedian[i];

            //DNA<T>[] children = new DNA<T>[2];
            //children = parent1.SinglePointCrossover(parent2, 1);


            //for (int j = 0; j < 2; j++) {
            //    children[j].Mutate(MutationRate);

            //    newPopulation.Add(children[j]);
            //}
        }
         else if(selectionProcess == 4){*/
            //for (int j = 0; j < 2; j++)
            //{
            //    for (int i = 0; i < 10; i++)
            //    {

            //        DNA<T> parent1 = best;
            //        //DNA<T> parent2 = FittestParentCrossoverSelection(i);
            //        DNA<T> parent2 = betterThenMedian[i];

            //        DNA<T> child = parent1.Crossover(parent2);

            //        child.Mutate(MutationRate);

            //        newPopulation.Add(child);

            //    }
            //}
        //}

        Population = newPopulation;

        Generation++;
        aboveAverageGenes.Clear();
    }

    public void CalculateFitness()
    {
        fitnessSum = 0;

        //initially set the best DNA
        best = Population[0];

        // loop through the population
        // check the current DNA fitness is greater than current best
        // if so set best to current DNA
        for (int i = 0; i < Population.Count; i++)
        {
            fitnessSum += Population[i].CalculateFitness(i, id); // 

            if (Population[i].Fitness > best.Fitness)
            {
                best = Population[i];
                bestDNAIndex = i;
            }
        }

        BestFitness = best.Fitness;
        best.Genes.CopyTo(BestGenes, 0);
    }

    private DNA<T> ChooseParent(int otherParent)
    {
        double averageFitness = fitnessSum / Population.Count;
        for (int i = 0; i < Population.Count; i++)
        {
            if (i != otherParent)
            {
                if (averageFitness < Population[i].Fitness)
                {
                    return Population[i];
                } 
            }
            parentChoice++;
        }

        return best;
    }
    private DNA<T> FittestParentCrossoverSelection(int genesIndex){

        DNA<T> parent = Population[genesIndex];
        return parent;
    }

    private DNA<T> HigherThanMedianSelection(int crossOvers){

        List<DNA<T>> betterThenMedian = new List<DNA<T>>(Population);
        betterThenMedian.RemoveRange(Population.Count / 2, Population.Count / 2);
        
         int otherParent = UnityEngine.Random.Range(0, betterThenMedian.Count);

         while (otherParent == crossOvers) {
             otherParent = UnityEngine.Random.Range(0, betterThenMedian.Count);
         }

        return betterThenMedian[crossOvers].Crossover(betterThenMedian[otherParent]);  
    }

    private DNA<T> ChooseRandomParent()
    {
        double randomNumber = UnityEngine.Random.value * fitnessSum;

        for (int i = 0; i < Population.Count; i++)
        {
            if (randomNumber <= Population[i].Fitness)
            {
                return Population[i];
            }

            randomNumber -= Population[i].Fitness;
        }

        return Population[random.Next(0, Population.Count)];
    }

}
