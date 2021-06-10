using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrown : MonoBehaviour
{
    Rigidbody rb;
    public Transform hand;

    private void Start() {
        GetComponent<MeshRenderer>().enabled = false;
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void ShootBall() {
        transform.position = hand.position + hand.forward;

        rb.isKinematic = false;

        rb.AddForce(hand.forward * 5, ForceMode.Impulse);
        rb.useGravity = true;
    }
}
