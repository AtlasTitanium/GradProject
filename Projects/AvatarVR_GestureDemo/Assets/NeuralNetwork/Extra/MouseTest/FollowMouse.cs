using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private int point = 0;
    private bool check = false;
    private bool left = false;

    private int recordTime = 3;
    private int iterateInSecond = 12;

    public LineRenderer line;
    public DrawNNTest testClass;
    private Vector3 lastPoint;

    private int currentShortMoves = 0;
    private int maxShortMoves = 4;

    public bool switchMode = false;

    private void Start() {
        int[] hiddenLayers = new int[1] { 5 };
        testClass.StartNetwork(recordTime * iterateInSecond, hiddenLayers, 2);
        line.positionCount = recordTime * iterateInSecond;
    }

    private void Update() {
        Vector3 temp = Input.mousePosition;
        temp.z = Random.Range(9.5f,10.5f);
        transform.position = Camera.main.ScreenToWorldPoint(temp);

        if (Input.GetMouseButtonDown(0)) {
            lastPoint = transform.position;
            SetLineCorrectly(transform.position, 0);
            check = true;
            left = true;
            StartCoroutine(IteratePoints());
        }
        if (Input.GetMouseButtonDown(1)) {
            lastPoint = transform.position;
            SetLineCorrectly(transform.position, 0);
            check = true;
            left = false;
            StartCoroutine(IteratePoints());
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            switchMode = !switchMode;
        }
    }

    IEnumerator IteratePoints() {
        point++;
        if (point >= iterateInSecond * recordTime) {
            check = false;
        }
        else {
            SetLineCorrectly(transform.position, point);

            if (Vector3.Distance(transform.position, lastPoint) <= 0.1f) {
                currentShortMoves++;
                if (currentShortMoves >= maxShortMoves) {
                    Debug.Log("too many short moves");
                    check = false;
                }
            }
            else {
                currentShortMoves = 0;
            }

            Debug.DrawRay(Vector3.zero, Camera.main.transform.InverseTransformDirection(transform.position - lastPoint).normalized, Color.blue);
            lastPoint = transform.position;
        }

        yield return new WaitForSeconds(1.0f / iterateInSecond);

        if (check) {
            StartCoroutine(IteratePoints());
        }
        else {
            point = 0;
            currentShortMoves = 0;
            Vector3[] points = new Vector3[line.positionCount];
            line.GetPositions(points);

            if (left) {
                testClass.Teach(points, switchMode ? 1 : 0);
            }
            else {
                testClass.Test(points);
            }
        }
    }

    private void SetLineCorrectly(Vector3 point, int index) {
        for (int i = index; i < (iterateInSecond * recordTime); i++) {
            line.SetPosition(i, point);
        }
    }
}
