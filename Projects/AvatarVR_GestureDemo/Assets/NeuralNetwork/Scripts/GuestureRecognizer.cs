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
    [Tooltip("Set the minimum output value the network needs to Invoke an output event")]
    [Range(0,1)]
    public float minValue = 0.75f;

    [Tooltip("Set CheckLine to show input and output lines.\n(can leave empty)")]
    public CheckLine checkLine;
    [Tooltip("Set point trackers for the network to learn and track")]
    public PointTracker[] pointTrackers;
    [Tooltip("Use a preset network data file")]
    public Object usePresetData;

    List<List<float>> inputs = new List<List<float>>();
    List<List<float>> outputs = new List<List<float>>();

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

        inputLenght = (iterationsPerSecond * secondsOfRecord) * vectorBreakdownAmount;
        if (checkLine != null) { 
            checkLine.InstantiateLines(outputEvents.Length);
        }

        if(usePresetData != null) {
            backprop = new BackpropagationNetwork(SaveNetworkData.Load(usePresetData), networkName);
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
            outputEvents[maxIndex].Invoke();
            if (checkLine != null) {
                checkLine.ShowLine(maxIndex, Color.red);
            }
        }
    }

    private void Teach(Vector3[] points, int index) {
        inputs.Add(new List<float>());
        outputs.Add(new List<float>());

        int last = inputs.Count - 1;
        for (int i = 0; i < points.Length; i++) {
            inputs[last].Add(Mathf.Abs(points[i].x));
            inputs[last].Add(Mathf.Abs(points[i].y));
            inputs[last].Add(Mathf.Abs(points[i].z));
        }

        last = outputs.Count - 1;
        for (int i = 0; i < outputEvents.Length; i++) {
            outputs[last].Add(index == i?1:0);
        }

        if (checkLine != null) {
            checkLine.ShowLine(index, Color.blue);
        }

        SaveNetworkData.Save(backprop.SetData(), networkName);
    }

    private void Update() {
        Learn();
    }

    private void Learn() {
        float[][] trainingValues = new float[inputs.Count][];
        float[][] desiredValues = new float[outputs.Count][];

        for (int i = 0; i < inputs.Count; i++) {
            trainingValues[i] = inputs[i].ToArray();
            desiredValues[i] = outputs[i].ToArray();
        }
        backprop.Train(trainingValues, desiredValues);
    }
}
