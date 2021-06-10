using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CheckLine : MonoBehaviour
{
    private LineRenderer line;

    private Vector3[][] storedLinePoints;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void InstantiateLines(int amountStored) {
        storedLinePoints = new Vector3[amountStored][];
    }

    public void UpdateLine(int index, Vector3[] points) {
        bool test = false;
        for (int i = 0; i < storedLinePoints.Length; i++) {
            if (storedLinePoints[index] != null) {
                test = true;
            }
        }

        if (test) {
            for (int i = 0; i < storedLinePoints[index].Length; i++) {
                storedLinePoints[index][i] = Vector3.Lerp(storedLinePoints[index][i], points[i], 0.5f);
            }
        } else {
            storedLinePoints[index] = points;
        }
    }

    public void ShowLine(int index, Color clr) {
        line.positionCount = 0;
        line.positionCount = storedLinePoints[index].Length;
        line.startColor = clr;
        line.endColor = clr;
        for (int i = 0; i < storedLinePoints[index].Length; i++) {
            line.SetPosition(i, storedLinePoints[index][i]);
        }
    }

    public void ResetLine() {
        line.positionCount = 0;
    }

    public Vector3[] FloatToPoint(float[] floats) {
        Vector3[] points = new Vector3[floats.Length / 3];
        for (int i = 0; i < floats.Length; i += 3) {
            points[i / 3] = new Vector3(floats[i], floats[i + 1], floats[i + 2]);
        }
        return points;
    }
}
