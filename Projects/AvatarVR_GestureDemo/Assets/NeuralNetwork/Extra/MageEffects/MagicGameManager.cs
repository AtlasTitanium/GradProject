using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicGameManager : MonoBehaviour
{
    GuestureRecognizer gr;
    Material mt;

    public Texture[] atlasTextures;
    public int currentTexture;

    public AudioSource audioSource;

    public Animator guideAnimator;

    private void Start() {
        gr = GuestureRecognizer.Instance;
        mt = GetComponent<Renderer>().material;
        StartTeachMethod();
    }

    public void StartTeachMethod() {
        audioSource.Play();
    }

    public void StartTeach(int index) {
        guideAnimator.speed = 0;
        audioSource.Pause();
        gr.learnDone.AddListener(Continue);
        gr.StartTeach(index);
    }

    public void Continue() {
        gr.learnDone.RemoveListener(Continue);
        guideAnimator.speed = 1;
        audioSource.Play();
    }

    private void Update() {
        currentTexture = Mathf.Clamp(currentTexture, 0, atlasTextures.Length-1);
        mt.mainTexture = atlasTextures[currentTexture];
    }
}
