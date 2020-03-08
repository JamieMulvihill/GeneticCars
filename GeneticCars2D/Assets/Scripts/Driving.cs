using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Driving : MonoBehaviour
{
    [Header("Text Labels")]
    [SerializeField] Text[] generationText;
    [SerializeField] Text[] bestFitnessText;
    [SerializeField] Text[] currentFitnessText;

    private Follow follow;
    public Spawn[] spawner;
    private GeneticAglorithm<float>[] geneticAlgorithm;
    public List<float> bestPrerformanceList;
    public List<DNA<float>> orderedCars;
    public int mutationRate = 10;
    int selectionProcessCompleted = 0;
    System.Random random;
    int dnaSize = 5;
    float longestRun;
    bool running = false;
    int initVariableIndex = 0;
    public float timeScale = 3f;
    // Start is called before the first frame update
    void Start()
    {
        spawner[0].color = new Color(1, 0, 0);
        spawner[1].color = new Color(0, 1, 0);
        spawner[2].color = new Color(0, 0, 1);
        geneticAlgorithm = new GeneticAglorithm<float>[3];
        for (int i = 0; i < 3; i++) {
            geneticAlgorithm[i] = new GeneticAglorithm<float>(i, 10, dnaSize, 0, random, GetRandomGene, GetInitGene, FitnessFunction, mutationRate: mutationRate);
        }
        
        //geneticAlgorithm[1] = new GeneticAglorithm<float>(10, dnaSize, 0,  random, GetRandomGene, GetInitGene, FitnessFunction, mutationRate: mutationRate);
        //geneticAlgorithm[2] = new GeneticAglorithm<float>(10, dnaSize, 0,  random, GetRandomGene, GetInitGene, FitnessFunction, mutationRate: mutationRate);
        bestPrerformanceList = new List<float>();
        follow = Camera.main.GetComponent<Follow>();
       
        orderedCars = new List<DNA<float>>();

        for (int i = 0; i < 3; i++)
        {
            generationText[i].text = geneticAlgorithm[i].Generation.ToString();
            bestFitnessText[i].text = geneticAlgorithm[i].BestFitness.ToString("F2");
            currentFitnessText[i].text = Camera.main.transform.position.x.ToString("F2");
        }
    }

    void Update()
    {
        Time.timeScale = timeScale;
        {//if (running)
         //{
         //    // If the agents are currently jumping, stop the script if the ALL enter the DeadZone
         //    if (agentManager.AreAgentsJumping())
         //    {
         //        if (agentManager.AllAgentsTouchedDeadZone())
         //        {
         //            this.enabled = false;
         //        }
         //    }
         //    // Make the agents jump if they are not jumping
         //    else
         //    {
         //        agentManager.UpdateAgentJumpingStrength(ga.Population);
         //        ga.NewGeneration();
         //        bestJump = ga.BestFitness;
         //        agentManager.MakeAgentsJump();
         //    }
         //}
        }
        {
            //for (int i = 0; i < spawner[0].cars.Length; i++)
            //{
            //    if (spawner[0].cars[i].transform.position.x > spawner[0].bestRun.x)
            //    {

            //        spawner[0].bestRun = spawner[0].cars[i].transform.position;
            //        follow.target = spawner[0].cars[i].transform;
            //    }
            //}
        }

        EndOfGeneration();
        for (int i = 0; i < 3; i++)
        {
            if (spawner[i].bestCar.transform.position.x > longestRun) {
                longestRun = spawner[i].bestCar.transform.position.x;
                follow.target = spawner[i].bestCar.transform;
            }
            generationText[i].text = geneticAlgorithm[i].Generation.ToString();
            bestFitnessText[i].text = geneticAlgorithm[i].BestFitness.ToString("F2");
            currentFitnessText[i].text = spawner[i].bestCar.transform.position.x.ToString("F2");
        }


        {
            //if (spawner[0].deadCars == spawner[0].cars.Length) {
            //    bestPrerformanceList.Add(spawner[0].bestRun.x);
            //    geneticAlgorithm.CalculateFitness();
            //    orderedCars = geneticAlgorithm.Population;
            //    orderedCars.Sort((DNA<float> a, DNA<float> b) => {
            //        return a.Fitness > b.Fitness ? -1 : 1;
            //    });

            //    float medianFitness = (orderedCars[(orderedCars.Count - 1) / 2].Fitness + orderedCars[(orderedCars.Count + 1) / 2].Fitness)/2; 

            //    WriteToFile(bestPrerformanceList.Count.ToString(), medianFitness.ToString(), "CarPerformance.csv");
            //    geneticAlgorithm.NewGeneration();
            //    spawner[0].deadCars = 0;
            //    NextGeneration();
            //    spawner[0].bestRun = new Vector2(0, 0);
            //}
        }
    }
    private float GetRandomGene(float gene)
    {
        int i = (int)gene;
        gene = spawner[0].cars[i].GetComponent<CarConstructor>().GeneRandomizer(gene);
        return gene;
       
    }
    private float GetInitGene(int index)
    {
        float[] initGenes = spawner[0].cars[index].GetComponent<CarConstructor>().genes;
        int returnValue = initVariableIndex;
        initVariableIndex++;
        if (initVariableIndex > 4) {
            initVariableIndex = 0;
        }
        return (initGenes[returnValue]);
    }

    private float FitnessFunction(int index, int spawnerIndex){
        float score = 0;

        score = spawner[spawnerIndex].cars[index].transform.position.x;

        return score;
    }

    private void NextGeneration(int index) {
        for (int i = 0; i < spawner[0].cars.Length; i++) {
            spawner[index].cars[i].GetComponent<CarConstructor>().genes = geneticAlgorithm[index].Population[i].Genes;
            spawner[index].cars[i].GetComponent<CarConstructor>().Construct();
            spawner[index].cars[i].transform.position = new Vector3(spawner[0].transform.position.x, spawner[0].transform.position.y, 0);
            spawner[index].cars[i].GetComponent<CarController>().deathCheckTime = 0;
            spawner[index].cars[i].gameObject.SetActive(true);
            follow.target = spawner[0].cars[i].transform;
            
        }
    }
    public static void WriteToFile(string ID, string resultA, string resultB, string resultC, string filePath) {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true)) {
            file.WriteLine(ID + "," + resultA + "," + resultB + "," + resultC);
        }
    }
    private void EndOfGeneration()
    {
        float[] medianValues = new float[3];
        int complete = 0;
        for (int i = 0; i < 3; i++) {
            if (spawner[i].deadCars == spawner[i].cars.Length) {
                complete++;
            }
        }

        if (complete == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                bestPrerformanceList.Add(spawner[i].bestRun.x);
                geneticAlgorithm[i].CalculateFitness();
                orderedCars = geneticAlgorithm[i].Population;
                orderedCars.Sort((DNA<float> a, DNA<float> b) =>
                {
                    return a.Fitness > b.Fitness ? -1 : 1;
                });

                float medianFitness = (orderedCars[(orderedCars.Count - 1) / 2].Fitness + orderedCars[(orderedCars.Count + 1) / 2].Fitness) / 2;
                medianValues[i] = medianFitness;
                geneticAlgorithm[i].NewGeneration();
                spawner[i].deadCars = 0;
                NextGeneration(i);
                spawner[i].bestRun = new Vector2(0, 0);
            }
            WriteToFile(bestPrerformanceList.Count.ToString(), medianValues[0].ToString(), medianValues[1].ToString(), medianValues[2].ToString(), "CarPerformance.csv");
            complete = 0;
            longestRun = 0;
        }
    }
       
}
