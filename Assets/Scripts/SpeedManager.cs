using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    public GameObject pacman;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pacmanAnimationChanger(PacStudentController.PacmanIsMoving);
    }

    public void pacmanAnimationChanger(bool pacmanIsMoving)
    {
        if (pacmanIsMoving)
        {
            pacman.GetComponent<Animator>().SetFloat("pacAniSpeed", 1.0f);
            if (!pacman.GetComponentInChildren<ParticleSystem>().isPlaying)
                pacman.GetComponentInChildren<ParticleSystem>().Play();
        }

        else
        {
            pacman.GetComponent<Animator>().SetFloat("pacAniSpeed", 0.0f);
            pacman.GetComponent<Animator>().Play("Pacman Reverse", 0, 0.0f);
            pacman.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }
}
