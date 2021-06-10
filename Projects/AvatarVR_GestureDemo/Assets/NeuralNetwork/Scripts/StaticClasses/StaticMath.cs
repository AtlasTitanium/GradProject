using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class StaticMath
{
    public static float Summize(IEnumerable<float> values, IReadOnlyList<float> weights) => values.Select((v, i) => v * weights[i]).Sum();     //the sum function, (v1*w1 + v2*w2 ... vn*wn) to sum up all the weights connected to 1 neuron

    public static float Sigmoid(float x) => 1f / (1f + (float)Mathf.Exp(-x));      //The sigmoid expression simplified

    public static float SigmoidDerivative(float x) => x * (1 - x);     //the derivative of the sigmoid expression simplified

    public static float RandomGaussian(float valueAround, float strenght) { //-----------------------------------------...these, maybe make a static script that holds all of these math functions and enumerators
        float minValue = valueAround - strenght;
        float maxValue = valueAround + strenght;
        float u, v, S;

        do {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    public static Vector3[] ConvertPointsToDirection(Vector3[] points, Transform fromPoint) {
        Vector3[] directionPoints = new Vector3[points.Length];
        for (int i = 1; i < directionPoints.Length; i++) {
            directionPoints[i] = fromPoint.InverseTransformDirection(points[i] - points[i - 1]).normalized;
        }

        return directionPoints;
    }
}
