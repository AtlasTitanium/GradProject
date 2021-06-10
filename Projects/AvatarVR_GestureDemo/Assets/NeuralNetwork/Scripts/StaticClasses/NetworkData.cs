using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

[System.Serializable]
public class NeuralNetworkData
{
    public float[][] neurons;
    public float[][] desiredNeurons;

    public float[][] biases;
    public float[][] biasesSmudge;

    public float[][][] weights;
    public float[][][] weightsSmudge;

    public NeuralNetworkData(IReadOnlyList<int> structure) {
        System.Random Random = new System.Random();
        neurons = new float[structure.Count][];
        desiredNeurons = new float[structure.Count][];

        biases = new float[structure.Count][];
        biasesSmudge = new float[structure.Count][];

        weights = new float[structure.Count - 1][][];
        weightsSmudge = new float[structure.Count - 1][][];

        for (int i = 0; i < structure.Count; i++) {
            neurons[i] = new float[structure[i]];
            desiredNeurons[i] = new float[structure[i]];

            biases[i] = new float[structure[i]];
            biasesSmudge[i] = new float[structure[i]];
        }

        for (int i = 0; i < structure.Count - 1; i++) {
            weights[i] = new float[neurons[i + 1].Length][];
            weightsSmudge[i] = new float[neurons[i + 1].Length][];
            for (int j = 0; j < weights[i].Length; j++) {
                weights[i][j] = new float[neurons[i].Length];
                weightsSmudge[i][j] = new float[neurons[i].Length];
                for (int k = 0; k < weights[i][j].Length; k++) {
                    weights[i][j][k] = (float)Random.NextDouble() * Mathf.Sqrt(2f / weights[i][j].Length);
                }
            }
        }
    }
}

public static class SaveNetworkData
{
    //public static NeuralNetworkData loadedData = null;
    public static void Save(NeuralNetworkData data, string name) {
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log(Application.dataPath + "/Assets/" + name.Replace(" ", "_") + ".sav");
        FileStream file = File.Create(Application.dataPath + "/" + name.Replace(" ", "_") + ".sav"); //you can call it anything you want
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Saved Game: " + name.Replace(" ", "_"));
    }

    public static NeuralNetworkData Load(Object assetData) {
        BinaryFormatter bf = new BinaryFormatter();
        //string path = EditorUtility.OpenFilePanel("Load network data", Application.streamingAssetsPath, "sav");
        string path = AssetDatabase.GetAssetPath(assetData);
        if (path.Length != 0) {
            FileStream file = File.Open(path, FileMode.Open);
            NeuralNetworkData loadedData = (NeuralNetworkData)bf.Deserialize(file);
            file.Close();
            Debug.Log("Loaded Game: " + path);
            return loadedData;
        }
        return null;
    }
}