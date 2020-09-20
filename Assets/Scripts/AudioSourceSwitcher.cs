using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceSwitcher : MonoBehaviour
{
    public AudioClip intro;
    public AudioClip backgroundNormal;
    public AudioClip walking;
    AudioSource audioManager;
    float timeCounter;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        timeCounter = 0;
        //Play intro clip
        audioManager = GetComponent<AudioSource>();
        audioManager.clip = intro;
        audioManager.Play();
        yield return new WaitForSeconds(audioManager.clip.length);
        //Switch to background normal music
        audioManager.clip = backgroundNormal;
        audioManager.spatialBlend = 0.44f;
        audioManager.loop = true;
        audioManager.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(timeCounter >= 0.5)
        {
            timeCounter = 0;
            audioManager.PlayOneShot(walking);
        }
        timeCounter += Time.deltaTime;
    }
}
