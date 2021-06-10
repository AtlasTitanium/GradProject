using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTracker : MonoBehaviour
{
    public LineRenderer testLine;
    [HideInInspector]
    public Vector3[] points;

    private int point = 0;
    private int recordTime;
    private int iterateInSecond;

    private Vector3[] setPoints;

    private GuestureRecognizer guestureRecognizer;

    private void Start() {
        guestureRecognizer = GuestureRecognizer.Instance;

        recordTime = guestureRecognizer.secondsOfRecord;
        iterateInSecond = guestureRecognizer.iterationsPerSecond;
        if (testLine != null) {
            testLine.positionCount = recordTime * iterateInSecond; //20 over 2 seconds = 40 iterantion
        }
        else {
            setPoints = new Vector3[recordTime * iterateInSecond];
        }
    }

    public void StartRecord() {
        if(testLine != null) {
            testLine.positionCount = 0;
            testLine.positionCount = recordTime * iterateInSecond;
        } else {
            setPoints = new Vector3[recordTime * iterateInSecond];
        }
       
        StartCoroutine(IteratePoints());
    }

    IEnumerator IteratePoints() {
        while (point < iterateInSecond * recordTime) {
            if (testLine != null) {
                SetLineCorrectly(guestureRecognizer.transform.InverseTransformPoint(transform.parent.position), point);
            }
            else {
                setPoints[point] = guestureRecognizer.transform.InverseTransformPoint(transform.parent.position);
            }
            point++;
            yield return new WaitForSeconds(1.0f / iterateInSecond);
        }
        point = 0;
        points = new Vector3[recordTime * iterateInSecond];
        if (testLine != null) {
            testLine.GetPositions(points);
        } else {
            points = setPoints;
        }
    }

    private void SetLineCorrectly(Vector3 point, int _index) {
        for (int i = _index; i < (iterateInSecond * recordTime); i++) {
            testLine.SetPosition(i, point);
        }
    }
}
