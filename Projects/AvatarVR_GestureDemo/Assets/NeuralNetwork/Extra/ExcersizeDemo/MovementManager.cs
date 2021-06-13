using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public GameObject goodUI, badUI;
    public Renderer rend;
    public Texture[] sprites;
    private int randomIndex;
    public void SetSprite(int index) {
        rend.material.mainTexture = sprites[index];
    }

    public void GetWorking() {
        goodUI.SetActive(false);
        badUI.SetActive(false);
        randomIndex = Random.Range(0, sprites.Length);
        rend.material.mainTexture = sprites[randomIndex];
        GuestureRecognizer gr = GuestureRecognizer.Instance;
        gr.StartTest();
    }

    public void Check(int index) {
        Debug.Log(index+" = index, random index = "+randomIndex);
        if(index == randomIndex) {
            goodUI.SetActive(true);
        } else {
            badUI.SetActive(true);
        }
    }
}
