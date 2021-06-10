using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class BackpropagationNetwork
{
    float[][] neurons;
    float[][] desiredNeurons;

    float[][] biases;
    float[][] biasesSmudge;

    float[][][] weights;
    float[][][] weightsSmudge;

    private const float WeightDecay = 0.001f;   //keep low, because it influences the decay of our weights and biases to a point where the network will compensate over values that shouldn't be on.
    private const float LearningRate = 1f;      //percentage from 0 to 1, makes the biases and weights change only by a certain percent

    NeuralNetworkData data;
    string dataName;
    public BackpropagationNetwork(IReadOnlyList<int> structure, string name) {
        dataName = name;
        data = new NeuralNetworkData(structure);
        neurons = data.neurons;
        desiredNeurons = data.desiredNeurons;
        biases = data.biases;
        biasesSmudge = data.biasesSmudge;
        weights = data.weights;
        weightsSmudge = data.weightsSmudge;
    }

    public BackpropagationNetwork(NeuralNetworkData data, string name) {
        dataName = name;
        neurons = data.neurons;
        desiredNeurons = data.desiredNeurons;
        biases = data.biases;
        biasesSmudge = data.biasesSmudge;
        weights = data.weights;
        weightsSmudge = data.weightsSmudge;
    }

    public float[] Test(float[] inputs) {
        for (int i = 0; i < neurons[0].Length; i++) {
            neurons[0][i] = inputs[i];      //set the first row of neurons to the input values;
        }

        for (int i = 1; i < neurons.Length; i++) {
            for (int j = 0; j < neurons[i].Length; j++) {
                //Debug.Log(StaticMath.Sigmoid(StaticMath.Summize(neurons[i - 1], weights[i - 1][j]) + biases[i][j]));
                neurons[i][j] = StaticMath.Sigmoid(StaticMath.Summize(neurons[i - 1], weights[i - 1][j]) + biases[i][j]);          //this is the full calculation for getting the value of each neuron with consideration of the correlating weights and biases
                desiredNeurons[i][j] = neurons[i][j];
            }
        }

        return neurons[neurons.Length - 1];         //returns ONLY the output neurons in an array
    }

    public void Train(float[][] trainingInputs, float[][] trainingOutputs) {
        for (var i = 0; i < trainingInputs.Length; i++) {
            Test(trainingInputs[i]);            //run test so all neurons will be initialized

            for (var j = 0; j < desiredNeurons[desiredNeurons.Length - 1].Length; j++)
                desiredNeurons[desiredNeurons.Length - 1][j] = trainingOutputs[i][j];       //first set all the desired neurons to the given desired outputs

            for (var j = neurons.Length - 1; j >= 1; j--) {     //go over all neurons except the input layer backwards.
                for (var k = 0; k < neurons[j].Length; k++) {       //then go forwward over every node
                    var biasSmudge = StaticMath.SigmoidDerivative(neurons[j][k]) * (desiredNeurons[j][k] - neurons[j][k]);
                    biasesSmudge[j][k] += biasSmudge;       //calculate the bias cost and add it to the biasesSmudge

                    for (var l = 0; l < neurons[j - 1].Length; l++) {
                        var weightSmudge = neurons[j - 1][l] * biasSmudge;
                        weightsSmudge[j - 1][k][l] += weightSmudge;     //calculate the weight cost, and add it to the weightSmudge

                        var valueSmudge = weights[j - 1][k][l] * biasSmudge;
                        desiredNeurons[j - 1][l] += valueSmudge;        //calculate the neuron cost, and add it to the desired neurons
                    }
                }
            }
        }

        for (var i = neurons.Length - 1; i >= 1; i--) { //again, go over all neurons except the input layer, backwards
            for (var j = 0; j < neurons[i].Length; j++) {
                biases[i][j] += biasesSmudge[i][j] * LearningRate;
                biases[i][j] *= 1 - WeightDecay;
                biasesSmudge[i][j] = 0;

                for (var k = 0; k < neurons[i - 1].Length; k++) {
                    weights[i - 1][j][k] += weightsSmudge[i - 1][j][k] * LearningRate;
                    weights[i - 1][j][k] *= 1 - WeightDecay;
                    weightsSmudge[i - 1][j][k] = 0;
                }

                desiredNeurons[i][j] = 0;
            }
        }

        SetData();
        SaveNetworkData.Save(data, dataName);
    }

    private void SetData() {
        data.neurons = neurons;
        data.desiredNeurons = desiredNeurons;
        data.biases = biases;
        data.biasesSmudge = biasesSmudge;
        data.weights = weights;
        data.weightsSmudge = weightsSmudge;
    }
}

/* //WHY DID I MAKE THIS!? I COULD"VE JUST MADE THE VARIABLES PUBLIC!
public void SetValues(float[][] _neurons, float[][] _desiredNeurons, float[][] _biases, float[][] _biasesSmudge, float[][][] _weights, float[][][] _weightsSmudge) {
    neurons = _neurons;
    desiredNeurons = _desiredNeurons;
    biases = _biases;
    biasesSmudge = _biasesSmudge;
    weights = _weights;
    weightsSmudge = _weightsSmudge;
}
    
public float[][][] GetNeurons() {
    float[][][] _neurons = new float[2][][];
    _neurons[0] = neurons;
    _neurons[1] = desiredNeurons;
    return _neurons;
}

public float[][][] GetBiases() {
    float[][][] _biases = new float[2][][];
    _biases[0] = biases;
    _biases[1] = biasesSmudge;
    return _biases;
}

public float[][][][] GetWeights() {
    float[][][][] _weights = new float[2][][][];
    _weights[0] = weights;
    _weights[1] = weightsSmudge;
    return _weights;
}
*/

/*
 //public BackpropagationNetwork(NeuralNetworkData dataStructure) {
    //    neurons = dataStructure.neurons;
    //    desiredNeurons = dataStructure.desiredNeurons;
    //    biases = dataStructure.biases;
    //    biasesSmudge = dataStructure.biasesSmudge;
    //    weights = dataStructure.weights;
    //    weightsSmudge = dataStructure.weightsSmudge;
    //}

    public NeuralNetworkData RetrieveNetworkData() {
        NeuralNetworkData data = new NeuralNetworkData();
        data.neurons = neurons;
        data.desiredNeurons = desiredNeurons;
        data.biases = biases;
        data.biasesSmudge = biasesSmudge;
        data.weights = weights;
        data.weightsSmudge = weightsSmudge;
        return data;
    }
 */