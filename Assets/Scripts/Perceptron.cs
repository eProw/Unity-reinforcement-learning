using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Perceptron
{
    public float[] inputs;
    public List<List<Neuron>> red;

    public Perceptron(int[] neuronsPerLayer, float[] inputs)
    {
        this.inputs = inputs;

        red = new List<List<Neuron>>();
        initNet(neuronsPerLayer);
    }

    public Perceptron(List<List<Neuron>> red)
    {
        this.red = red;
    }

    private void initNet(int[] neuronsPerLayer)
    {
        for (int i = 0; i < neuronsPerLayer.Length; i++)
        {
            List<Neuron> layer = new List<Neuron>();
            int neurons = neuronsPerLayer[i];

            for (int z = 0; z < neurons; z++)
            {
                if (i == 0)
                {
                    layer.Add(new Neuron(inputs));
                }
                else
                {
                    layer.Add(new Neuron(new float[red[i - 1].Count]));
                }
            }
            red.Add(layer);
        }
    }

    public void UpdateInputs(float[] inputs)
    {
        this.inputs = inputs;
    }

    public float[] Calculate()
    {
        float[] outputs = new float[red[red.Count - 1].Count];

        for (int i = 0; i < red.Count; i++)
        {
            List<Neuron> layer = red[i];
            float[] neuronsOutput;

            if (i != 0)
            {
                //Obtener calculos como entrada de las siguientes neuronas
                neuronsOutput = new float[red[i - 1].Count];
                for (int neurona = 0; neurona < neuronsOutput.Length; neurona++)
                {
                    neuronsOutput[neurona] = (red[i - 1])[neurona].output;
                }
            }
            else
            {
                neuronsOutput = new float[0];
            }

            for (int z = 0; z < layer.Count; z++)
            {

                if (i == 0)
                {
                    //Asignando a cada neurona el numero de entradas a recibir (DE LOS VALORES DE ENTRADA DEL PERCEPTRON)
                    layer[z].inputs = this.inputs;
                }
                else
                {
                    //Asignando a cada neurona el numero de entradas a recibir (DE LA ANTERIOR CAPA DE NEURONAS DEL PERCEPTRON)
                    layer[z].inputs = neuronsOutput;
                }

                layer[z].Calculation();

                if (i == red.Count - 1)
                {
                    outputs[z] = layer[z].output;
                }
            }
        }

        return outputs;
    }
}

public class Neuron
{
    public float[] inputs, weights;
    public float bias;
    public float output;

    //Random rand;

    public Neuron()
    {

    }

    public Neuron(float[] inputs)
    {
        this.inputs = inputs;
        this.weights = new float[inputs.Length];

        //rand = new Random();

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(-1f,1); //rand.nextFloat() * 2 - 1; (JAVA)
        }

        bias = Random.Range(-1f, 1f);
    }

    public Neuron(float[] inputs, float[] weights, float bias)
    {
        this.inputs = inputs;
        this.weights = weights;
        this.bias = bias;
    }

    public float Calculation()
    {
        float result = 0;

        for (int i = 0; i < inputs.Length; i++)
        {
            result += inputs[i] * weights[i];
        }

        float output = activation(result + bias);
        this.output = output;

        return output;
    }

    float activation(float x)
    {
        //return(x>=0?1:-1);
        return (1 / (1 + (Mathf.Exp(-x))));
    }
}
