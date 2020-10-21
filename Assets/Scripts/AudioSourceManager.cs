using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public GameObject pacman;
    public GameObject[] ghosts = new GameObject[4];
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
    void Start()
    {
        collisionPlayed = false;
        timeCounter = 0;
        //Play intro clip
        audioManager = GetComponent<AudioSource>();
        audioManager.clip = intro;
        audioManager.spatialBlend = 0.2f;
        audioManager.loop = true;
        audioManager.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Only run sounds after the start countdown is finished
        if (HUDManager.StartCounter <= -1.0f)
        {
            //Sound Effects
            if (timeCounter >= 0.5)
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


            //Background Music
            if (audioManager.clip == intro)
            {
                changeBackgroundMusic(backgroundNormal);
            }

            else if (audioManager.clip == backgroundNormal)
            {
                if (PacStudentController.CurrentState == PacStudentController.PacState.Powered)
                    changeBackgroundMusic(backgroundScared);
            }

            else if (audioManager.clip == backgroundScared)
            {
                if (PacStudentController.CurrentState == PacStudentController.PacState.Normal && !anyDeadGhosts())
                    changeBackgroundMusic(backgroundNormal);

                else if (anyDeadGhosts())
                    changeBackgroundMusic(backgroundDead);
            }
            else if (audioManager.clip == backgroundDead)
            {
                if (!anyDeadGhosts())
                {
                    if (PacStudentController.CurrentState == PacStudentController.PacState.Normal)
                        changeBackgroundMusic(backgroundNormal);

                    else if (PacStudentController.CurrentState == PacStudentController.PacState.Powered)
                        changeBackgroundMusic(backgroundScared);
                }
            }
        }
    }

    private void changeBackgroundMusic(AudioClip clip)
    {
        audioManager.clip = clip;
        audioManager.Play();
    }

    private bool anyDeadGhosts()
    {
        return ghosts[0].GetComponent<Animator>().GetBool("isDead") || ghosts[1].GetComponent<Animator>().GetBool("isDead") || ghosts[2].GetComponent<Animator>().GetBool("isDead") || ghosts[3].GetComponent<Animator>().GetBool("isDead");
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
