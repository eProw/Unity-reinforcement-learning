using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*  Implementacion de algoritmos evolutivos para el entrenamiento de 
 *                      redes neuronales
 *          -Creado por Jose Antonio Sanchez Suarez-
 *                          30/3/18
 */

public class EvolutionAlgorithm : MonoBehaviour {

    public LayerMask whatToHit;
    public GameObject prefab;
    public Transform start;
    public Text infoText;

    public Text neuronInfo;

    Individual[] population,bestIndividuals,parents;
    int currentIndiv = 0,maxIndividuals = 10, generation = 0;

    bool firstIndividualGenerated=false,foundBestIndividual = false,startEvolving = false;

    float fitness = 0;

	void Start () {
        Time.timeScale = 1;
        population = new Individual[maxIndividuals];
        bestIndividuals = parents = new Individual[2];
        GenerateNextIndividual(false);
    }
	
	void Update () {
        /*
         *Cuando el objeto actual se elimine por colisionar contra una pared, se generara el siguiente
         * individuo de la poblacion. 
         * Mientras tanto, se realizara un seguimiento del objeto actual:
         */

        if (currentIndiv < maxIndividuals)
        {
            if (population[currentIndiv].obj.activeSelf)
            {
                //FITNESS... (El individuo que mas se aleje del punto de partida se convertira en candidato para transmitir su genoma):
                //Calcular distancia entre los dos puntos: el inicio y el individuo
                float distance = Vector2.Distance(start.position, population[currentIndiv].obj.transform.position);
                if (distance > fitness)
                {
                    fitness = distance;

                    population[currentIndiv].bestScore = distance;
                    //Reordenar los dos mejores individuos

                    if (!foundBestIndividual)
                    {
                        //Se ha encontrado el primer individuo con mejor marca; se coloca en primer lugar
                        bestIndividuals[0] = population[currentIndiv];
                        foundBestIndividual = true;
                    }
                    else
                    {
                        //Se ha encontrado un individuo con mejor marca que el anterior; este queda en segundo puesto
                        if (currentIndiv != bestIndividuals[0].index) //Un mismo individuo no puede ser primero y segundo al mismo tiempo
                        {
                            bestIndividuals[1] = bestIndividuals[0];
                        }
                        bestIndividuals[0] = population[currentIndiv];
                    }
                }

                //Aunque este individuo no haya conseguido superar el record, se guardara su puntuacion 
                //para compararla con la segunda mejor marca
                if (distance > population[currentIndiv].bestScore)
                {
                    population[currentIndiv].bestScore = distance;
                    if (population[currentIndiv].bestScore > bestIndividuals[1].bestScore && currentIndiv != bestIndividuals[0].index)
                    {
                        bestIndividuals[1] = population[currentIndiv];
                    }
                }
            }
            else if (!population[currentIndiv].obj.activeSelf)
            {
                if (currentIndiv < maxIndividuals - 1)
                    //Anterior individuo ha terminado. Generar siguiente individuo en la lista
                    GenerateNextIndividual(startEvolving);
                else
                    currentIndiv=11; //Termina la generacion actual 
            }
        }
        if (currentIndiv >= maxIndividuals)
        {
            //Comienza siguiente generacion, mezcla de los genomas de los ultimos candidatos y formacion de los siguientes individuos
            NextGeneration();
        }

        //Actualizar informacion en pantalla
        infoText.text = "Generation: "+generation+"\nIndividual: "+(currentIndiv+1)+"\nFitness in generation "+generation+": "+fitness+
            "\nBest individuals: "+"\n-Candidate "+(bestIndividuals[0].index+1)+": "+bestIndividuals[0].bestScore+"\n"+(bestIndividuals[1].bestScore==0?"":("-Candidate "+(bestIndividuals[1].index + 1) + ": " + bestIndividuals[1].bestScore));

        neuronInfo.text = "";
        for (int i = 0; i < population[currentIndiv].brain.p.red.Count; i++)
        {
            neuronInfo.text += "\n";
            List<Neuron> layer = population[currentIndiv].brain.p.red[i];
            for (int n = 0; n < layer.Count; n++)
            {
                neuronInfo.text += "  ";
                Neuron neuron = layer[n];
                for (int w = 0; w < neuron.weights.Length; w++)
                {
                    neuronInfo.text += neuron.weights[w].ToString();
                }
            }
        }
    }

    void GenerateNextIndividual(bool evolving)
    {
        GameObject indiv = ((GameObject)Instantiate(prefab, start.position, Quaternion.Euler(0,0,270)));
        NeuralNetwork net = indiv.AddComponent<NeuralNetwork>();

        if (evolving)
        {
            net.randomNet = false;
        }

        net.ForceInit();

        net.whatToHit = whatToHit;

        if (evolving)
        {
            net.p.red = CrossoverNets(net.p.red);
        }

        if (firstIndividualGenerated) {
            currentIndiv++;
        }
        else
        {
            firstIndividualGenerated = true;
        }
        population[currentIndiv] = new Individual(currentIndiv,net,indiv);
    }

    void NextGeneration()
    {

        parents = bestIndividuals;
        population = new Individual[maxIndividuals];  

        bestIndividuals = new Individual[2];

        firstIndividualGenerated = false;
        foundBestIndividual = false;

        ///
        if(!startEvolving)
            startEvolving = true;
        ///

        fitness = 0;
        currentIndiv = 0;
        generation++;

        GenerateNextIndividual(startEvolving);
    }

    List<List<Neuron>> CrossoverNets(List<List<Neuron>> originalNet)
    {
        float mutationRate = 5;
        List<List<Neuron>> combinedNet = originalNet;
        List<List<Neuron>> netA = parents[0].brain.p.red;
        List<List<Neuron>> netB = parents[1].brain.p.red;

        for (int i = 0; i < combinedNet.Count;i++)
        {
            List<Neuron> layer = combinedNet[i];
            for (int n = 0; n < layer.Count;n++)
            {
                Neuron neuron = layer[n];
                for (int w = 0;w < neuron.weights.Length;w++)
                {
                    float perc = Random.Range(0f, 100f);
                    if (perc > mutationRate && perc < 100)
                    {
                        neuron.weights[w] = netA[i][n].weights[w];
                        neuron.bias = netA[i][n].bias;
                    }else if (perc > 70)
                    {
                        neuron.weights[w] = netB[i][n].weights[w];
                        neuron.bias = netB[i][n].bias;
                    }/*else if (perc < mutationRate)
                    {
                        perc = Random.Range(0f, 100f);
                        float perc1 = Random.Range(0f, 100f);
                        float perc2 = Random.Range(0f, 100f);
                        float percBias = Random.Range(0f, 100f);
                        float perc4 = Random.Range(0f, 100f);

                        if (perc > 30)
                        {
                            neuron.weights[w] = (neuron.weights[w] = netA[i][n].weights[w] + netB[i][n].weights[w])/2;
                            neuron.bias = (netA[i][n].bias+ netB[i][n].bias)/2;
                        }
                        else
                        {
                            neuron.weights[w] = (perc1 > 40 ? netA : netB)[i][n].weights[w] += (perc2 > 50 ? 0.1f : -0.1f);
                            neuron.bias = (percBias > 40 ? netA : netB)[i][n].bias += (perc4 > 50 ? 0.1f : -0.1f);
                        }
                    }*/
                }
            }
        }

        return combinedNet;
    }

    struct Individual
    {
        public int index;
        public float bestScore;
        public NeuralNetwork brain;
        public GameObject obj;

        public Individual(int index, NeuralNetwork brain, GameObject obj)
        {
            this.index = index;
            this.brain = brain;
            this.bestScore = 0;
            this.obj = obj;
        }

        public Individual(int index,float bestScore,NeuralNetwork brain, GameObject obj)
        {
            this.index = index;
            this.brain = brain;
            this.bestScore = bestScore;
            this.obj = obj;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawIcon(Vector2.zero,"JELLO");
    }
}
