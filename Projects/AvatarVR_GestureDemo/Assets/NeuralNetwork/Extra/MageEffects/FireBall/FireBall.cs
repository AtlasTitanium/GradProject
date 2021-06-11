using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject fireballPrefab;

    public void Fire(int strenght) {
        Debug.Log("start fire");
        GameObject instance = Instantiate(fireballPrefab);
        Debug.Log("he's here = " + instance);
        Rigidbody rb = instance.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * strenght, ForceMode.Impulse);
    }
}
