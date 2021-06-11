using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    ParticleSystem ps;
    private void Start() {
        ps = GetComponent<ParticleSystem>();
    }

    public void ShootLightning() {
        StartCoroutine(LightningTillStop());
        ps.Play();
    }

    IEnumerator LightningTillStop() {
        yield return new WaitForSeconds(2);
        ps.Stop();
    }
}
