using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamTheRock : MonoBehaviour
{
    public GameObject rockPrefab;

    public void ThrowRock() {
        GameObject instance = Instantiate(rockPrefab);
        instance.transform.position = transform.up * 2;
        instance.GetComponent<Rigidbody>().AddForce(transform.forward * 20,ForceMode.Impulse);
    }
}
