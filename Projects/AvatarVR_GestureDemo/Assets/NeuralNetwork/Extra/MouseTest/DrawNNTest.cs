using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawNNTest : MonoBehaviour
{
    [Range(0.5f, 5)]
    [Tooltip("A small value makes the input more precise, a big value makes it more general")]
    public float inputStrenghtSize = 1.0f;

    public LineRenderer testLine;

    private float[][] inputs;
    private float[][] inputLines;
    private float[][] outputs;

    private int inputLenght;
    private int amountOfOutputs;

    private int vectorBreakdownAmount = 3;

    BackpropagationNetwork backprop;
    public void StartNetwork(int input, int[] hiddenLayers, int output) {
        inputLenght = input * vectorBreakdownAmount;
        amountOfOutputs = output;
        SetInputsAndOutputs();

        List<int> structure = new List<int>();
        structure.Add(inputLenght); //input layer, let's make these the x and y of a 2d direction
        foreach (int i in hiddenLayers) {
            structure.Add(i);
        }
        structure.Add(amountOfOutputs); //output layer, let's make 0 to up, and 1 to down
        backprop = new BackpropagationNetwork(structure, "MouseTest");
    }

    private void SetInputsAndOutputs() {
        inputs = new float[amountOfOutputs][];      //SetLenght
        for (int i = 0; i < inputs.Length; i++) {
            inputs[i] = new float[inputLenght];
        }
        inputLines = new float[amountOfOutputs][];      //SetLenght
        for (int i = 0; i < inputLines.Length; i++) {
            inputLines[i] = new float[inputLenght];
        }
        outputs = new float[amountOfOutputs][];     //SetLenght
        for (int i = 0; i < outputs.Length; i++) {
            outputs[i] = new float[amountOfOutputs];
        }

        int setIndex = 0;                           //SetOutputs correctly
        for (int i = 0; i < outputs.Length; i++) {
            for (int j = 0; j < outputs[i].Length; j++) {
                outputs[i][j] = (j == setIndex) ? 1 : 0;
            }
            setIndex++;
        }
    }

    public void Test(Vector3[] points) {
        Vector3[] directionPoints = ConvertPointsToDirection(points);
        float[] input = new float[inputLenght];
        for (int i = 0; i < inputLenght; i += vectorBreakdownAmount) {
            input[i] = directionPoints[i / vectorBreakdownAmount].x;
            input[i + 1] = directionPoints[i / vectorBreakdownAmount].y;
            input[i + 2] = directionPoints[i / vectorBreakdownAmount].z;
        }

        float[] output = backprop.Test(input);
        for (int i = 0; i < output.Length; i++) {
            Debug.Log("output value for " + i + " = " + output[i]);
        }

        float maxValue = output.Max();
        int maxIndex = output.ToList().IndexOf(maxValue);

        TestLine(FloatToPoint(inputLines[maxIndex]));
    }

    public void TestGausian(Vector3[] points) {
        for (int i = 0; i < points.Length; i++) {
            points[i] = new Vector3(StaticMath.RandomGaussian(points[i].x, inputStrenghtSize), StaticMath.RandomGaussian(points[i].y, inputStrenghtSize), StaticMath.RandomGaussian(points[i].z, inputStrenghtSize));
            TestLine(points);
        }
    }

    public void TestLine(Vector3[] points) {
        testLine.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++) {
            testLine.SetPosition(i, points[i]);
        }
    }

    public Vector3[] FloatToPoint(float[] floats) {
        Vector3[] points = new Vector3[floats.Length / vectorBreakdownAmount];
        for (int i = 0; i < floats.Length; i += vectorBreakdownAmount) {
            points[i / vectorBreakdownAmount] = new Vector3(floats[i], floats[i + 1], floats[i + 2]);
        }
        return points;
    }

    public void Teach(Vector3[] points, int index) {
        Vector3[] directionPoints = ConvertPointsToDirection(points);

        bool test = false;
        for (int i = 0; i < inputs[index].Length; i++) {
            if(inputs[index][i] != 0) {
                test = true;
            }
        }
        
        if (test) {
            for (int j = 0; j < inputs[index].Length; j += vectorBreakdownAmount) {
                inputs[index][j] = Mathf.Lerp(inputs[index][j], directionPoints[j / vectorBreakdownAmount].x, 0.5f);
                inputs[index][j + 1] = Mathf.Lerp(inputs[index][j + 1], directionPoints[j / vectorBreakdownAmount].y, 0.5f);
                inputs[index][j + 2] = Mathf.Lerp(inputs[index][j + 2], directionPoints[j / vectorBreakdownAmount].z, 0.5f);

                inputLines[index][j] = Mathf.Lerp(inputLines[index][j], points[j / vectorBreakdownAmount].x, 0.5f);
                inputLines[index][j + 1] = Mathf.Lerp(inputLines[index][j + 1], points[j / vectorBreakdownAmount].y, 0.5f);
                inputLines[index][j + 2] = Mathf.Lerp(inputLines[index][j + 2], points[j / vectorBreakdownAmount].z, 0.5f);
            }
        }
        else {
            for (int j = 0; j < inputs[index].Length; j += vectorBreakdownAmount) {
                inputs[index][j] = directionPoints[j / vectorBreakdownAmount].x;
                inputs[index][j + 1] = directionPoints[j / vectorBreakdownAmount].y;
                inputs[index][j + 2] = directionPoints[j / vectorBreakdownAmount].z;

                inputLines[index][j] = points[j / vectorBreakdownAmount].x;
                inputLines[index][j + 1] = points[j / vectorBreakdownAmount].y;
                inputLines[index][j + 2] = points[j / vectorBreakdownAmount].z;
            }
        }

        TestLine(FloatToPoint(inputLines[index]));
        Learn();
    }

    private Vector3[] ConvertPointsToDirection(Vector3[] points) {
        Vector3[] directionPoints = new Vector3[points.Length];
        for (int i = 1; i < directionPoints.Length; i++) {
            directionPoints[i] = transform.InverseTransformDirection(points[i] - points[i - 1]).normalized;
        }

        return directionPoints;
    }

    public void Learn(int iterations = 10) {
        int setIterations = iterations * amountOfOutputs;
        float[][] trainingValues = new float[setIterations][];
        float[][] desiredValues = new float[setIterations][];

        for (int i = 0; i < setIterations; i++) {
            trainingValues[i] = new float[inputLenght];
            desiredValues[i] = new float[amountOfOutputs];

            int index = Mathf.FloorToInt((float)i / iterations);
            trainingValues[i] = inputs[index];
            desiredValues[i] = outputs[index];
        }

        backprop.Train(trainingValues, desiredValues);
    }
}
