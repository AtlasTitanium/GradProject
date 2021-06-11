using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour
{
    public GameObject ps;
    private void OnCollisionEnter(Collision collision) {
        GameObject instance = Instantiate(ps);
        instance.transform.position = transform.position;
        instance.GetComponent<ParticleSystem>().Play();
        Destroy(instance, 1);
        Destroy(gameObject);
    }
}
