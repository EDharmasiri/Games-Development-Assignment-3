using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public GameObject pacman;
    [SerializeField]
    private AudioClip intro;
    [SerializeField]
    private AudioClip backgroundNormal;
    [SerializeField]
    private AudioClip walking;
    [SerializeField]
    private AudioClip eatingPellet;
    [SerializeField]
    private AudioClip collide;
    [SerializeField]
    private AudioClip backgroundScared;
    [SerializeField]
    private AudioClip backgroundDead;

    private AudioSource audioManager;
    private float timeCounter;
    private bool collisionPlayed;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        collisionPlayed = false;
        timeCounter = 0;
        //Play intro clip
        audioManager = GetComponent<AudioSource>();
        audioManager.clip = intro;
        audioManager.Play();
        yield return new WaitForSeconds(audioManager.clip.length);
        //Switch to background normal music
        audioManager.clip = backgroundNormal;
        audioManager.spatialBlend = 0.2f;
        audioManager.loop = true;
        audioManager.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(timeCounter >= 0.5)
        {
            timeCounter = 0;
            checkForPickups();
        }
        timeCounter += Time.deltaTime;

        if (pacman.GetComponent<Animator>().GetFloat("pacAniSpeed") == 0 && !collisionPlayed && !PacStudentController.tweening())
        {
            audioManager.PlayOneShot(collide);
            collisionPlayed = true;
        }

        else if (pacman.GetComponent<Animator>().GetFloat("pacAniSpeed") == 1)
            collisionPlayed = false;

        //Change background music depending on state
        if (PacStudentController.CurrentState == PacStudentController.PacState.Powered && audioManager.clip != backgroundScared)
        {
            audioManager.clip = backgroundScared;
            audioManager.Play();
        }
        if(PacStudentController.PowerTime <= 0.0 && audioManager.clip != intro && audioManager.clip != backgroundNormal)
        {
            audioManager.clip = backgroundNormal;
            audioManager.Play();
        }
    }

    private void checkForPickups()
    {
        if (pacman.GetComponent<Animator>().GetFloat("pacAniSpeed") == 1)
        {
            if (PacStudentController.PickupHit.collider == null)
                audioManager.PlayOneShot(walking);

            else if (PacStudentController.PickupHit.collider.tag == "Pellet" || PacStudentController.PickupHit.collider.tag == "PowerPill")
                audioManager.PlayOneShot(eatingPellet);
        }
        
    }
}
