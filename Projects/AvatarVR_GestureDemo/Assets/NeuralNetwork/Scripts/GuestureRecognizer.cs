using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class GuestureRecognizer : MonoBehaviour
{
    #region Singleton
    private static GuestureRecognizer _instance;
    public static GuestureRecognizer Instance { get { return _instance; } }
    #endregion
    [Tooltip("Set the name of your network (add folder specifications for code cleanup \n(example: <folder name>/dataname))")]
    public string networkName;
    [Tooltip("Sets the amount of times that the point tracker will learn within the second")]
    public int iterationsPerSecond = 20;
    [Tooltip("Sets the time the network will record your guesture")]
    public int secondsOfRecord = 3;
    [Tooltip("amount of neurons in amount of hidden layers.\n(more layers = a more specific & a harder configuarble -network)")]
    public int[] hiddenLayers = new int[2] { 16, 16 };
    [Tooltip("Set amount of outputs and their event handlers.")]
    public UnityEvent[] outputEvents = new UnityEvent[2];
    [Tooltip("Set how many times the network will learn per given guesture. (* the amount of outputs)")]
    public int learnIterations = 10;
    [Tooltip("Set the minimum output value the network needs to Invoke an output event")]
    [Range(0,1)]
    public float minValue = 0.75f;

    [Tooltip("Set CheckLine to show input and output lines.\n(can leave empty)")]
    public CheckLine checkLine;
    [Tooltip("Set point trackers for the network to learn and track")]
    public PointTracker[] pointTrackers;
    [Tooltip("Use a preset network data file")]
    public bool usePresetData;

    private float[][] inputs;
    private float[][] outputs;
    private int inputLenght;
    private int vectorBreakdownAmount = 3;

    BackpropagationNetwork backprop;

    private void Awake() {
        #region Singleton
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
        #endregion

        SetInputsAndOutputs();
        if (checkLine != null) { 
            checkLine.InstantiateLines(outputEvents.Length);
        }

        if(usePresetData) {
            SaveNetworkData.Load();
            backprop = new BackpropagationNetwork(SaveNetworkData.loadedData, networkName);
        } else {
            backprop = new BackpropagationNetwork(GetStructure(), networkName);
        }
    }

    private List<int> GetStructure() {
        List<int> structure = new List<int>();
        structure.Add(inputLenght);
        foreach (int i in hiddenLayers) {
            structure.Add(i);
        }
        structure.Add(outputEvents.Length);
        return structure;
    }

    private void SetInputsAndOutputs() {
        inputLenght = (iterationsPerSecond * secondsOfRecord)*vectorBreakdownAmount;

        inputs = new float[outputEvents.Length][];
        outputs = new float[outputEvents.Length][];

        for (int i = 0; i < inputs.Length; i++) {
            inputs[i] = new float[inputLenght*pointTrackers.Length];
        }

        int setIndex = 0;
        for (int i = 0; i < outputs.Length; i++) {
            outputs[i] = new float[outputEvents.Length];
            for (int j = 0; j < outputs[i].Length; j++) {
                outputs[i][j] = (j == setIndex) ? 1 : 0;
            }
            setIndex++;
        }
    }

    public void StartTeach(int index) {
        StartCoroutine(TrackPoints(index));
    }

    public void StartTest() {
        StartCoroutine(TrackPoints(-1));
    }

    private IEnumerator TrackPoints(int index) {
        for (int i = 0; i < pointTrackers.Length; i++) {
            pointTrackers[i].StartRecord();
        }

        yield return new WaitForSeconds(secondsOfRecord + 0.5f);

        Vector3[] points = new Vector3[(iterationsPerSecond * secondsOfRecord) * pointTrackers.Length];
        for (int i = 0; i < pointTrackers.Length; i++) {
            for (int j = 0; j < (iterationsPerSecond * secondsOfRecord); j++) {
                points[(i * (iterationsPerSecond * secondsOfRecord)) + j] = pointTrackers[i].points[j];
            }
        }

        if (index >= 0 && checkLine != null) {
            checkLine.UpdateLine(index, points);
        }

        Vector3[] directionPoints = StaticMath.ConvertPointsToDirection(points, transform);

        if (index < 0) {
            Test(directionPoints);
        } else {
            Teach(directionPoints, index);
        }
    }

    private void Test(Vector3[] points) {
        float[] input = new float[inputLenght];
        for (int i = 0; i < inputLenght; i += vectorBreakdownAmount) {
            input[i] = Mathf.Abs(points[i / vectorBreakdownAmount].x);
            input[i + 1] = Mathf.Abs(points[i / vectorBreakdownAmount].y);
            input[i + 2] = Mathf.Abs(points[i / vectorBreakdownAmount].z);
        }

        float[] output = backprop.Test(input);
        for (int i = 0; i < output.Length; i++) {
            Debug.Log("output value for " + i + " = " + output[i]);
        }
        float maxValue = output.Max();
        int maxIndex = output.ToList().IndexOf(maxValue);

        if(maxValue >= minValue) {
            Debug.Log(maxIndex);
            outputEvents[maxIndex].Invoke();
            if (checkLine != null) {
                checkLine.ShowLine(maxIndex, Color.red);
                Debug.Log("line should show up");
            }
        }
    }

    private void Teach(Vector3[] points, int index) {
        bool test = false;
        for (int i = 0; i < inputs[index].Length; i++) {
            if (inputs[index][i] != 0) {
                test = true;
            }
        }

        if (test) {
            for (int j = 0; j < inputLenght; j += vectorBreakdownAmount) {
                inputs[index][j] = Mathf.Lerp(inputs[index][j], points[j / vectorBreakdownAmount].x, 0.5f);
                inputs[index][j + 1] = Mathf.Lerp(inputs[index][j + 1], points[j / vectorBreakdownAmount].y, 0.5f);
                inputs[index][j + 2] = Mathf.Lerp(inputs[index][j + 2], points[j / vectorBreakdownAmount].z, 0.5f);
            }
        }
        else {
            for (int j = 0; j < inputLenght; j += vectorBreakdownAmount) {
                inputs[index][j] = points[j / vectorBreakdownAmount].x;
                inputs[index][j + 1] = points[j / vectorBreakdownAmount].y;
                inputs[index][j + 2] = points[j / vectorBreakdownAmount].z;
            }
        }

        if (checkLine != null) {
            checkLine.ShowLine(index, Color.blue);
        }
        Learn(learnIterations);
    }

    private void Learn(int iterations = 10) {
        int setIterations = iterations * outputEvents.Length;
        float[][] trainingValues = new float[setIterations][];
        float[][] desiredValues = new float[setIterations][];

        for (int i = 0; i < setIterations; i++) {
            trainingValues[i] = new float[inputLenght];
            desiredValues[i] = new float[outputEvents.Length];

            int index = Mathf.FloorToInt((float)i / iterations);
            //int index = Mathf.RoundToInt(Random.Range(0, amountOfOutputs));
            trainingValues[i] = inputs[index];
            desiredValues[i] = outputs[index];
        }

        backprop.Train(trainingValues, desiredValues);
    }
}
