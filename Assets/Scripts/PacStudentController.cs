using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    //Pacman & Ghosts
    public GameObject pacman;
    public GameObject[] ghosts = new GameObject[4];
    //Movement
    private KeyCode lastInput;
    private KeyCode currentInput;
    private static Tweener tweener;
    private float duration;
    //Raycasting, Particles and collisions
    private static RaycastHit2D pickupHit;
    private static RaycastHit2D ghostHit;
    public GameObject wallParticle;
    private bool firstWallCollision;
    public GameObject pacDeathParticles;
    //Teleportation
    private Vector3 leftTeleport;
    private Vector3 rightTeleport;
    private bool justTeleported;

    //Keep track of pacman's state
    public enum PacState { Normal, Powered, Dead }

    //Keep track of ghost death timer
    private float[] ghostDeathTimer;

    //Dictionaries
    private Dictionary<KeyCode, Vector2> keyToDirection = new Dictionary<KeyCode, Vector2>();
    private Dictionary<KeyCode, Vector3> keyToEndPosition = new Dictionary<KeyCode, Vector3>();
    private Dictionary<KeyCode, Quaternion> keyToRotation = new Dictionary<KeyCode, Quaternion>();
    private Dictionary<KeyCode, Vector3> keyToLocalScale = new Dictionary<KeyCode, Vector3>();


    public static RaycastHit2D PickupHit { get { return pickupHit; } }

    public static PacState CurrentState { get; private set; }
    public static float PowerTime { get; private set; }

    public static bool PacmanIsMoving { get; private set; }

    public static bool GhostIsDead { get; set; }
    public static bool PlayDeathSound { get; set; }

    private void Start()
    {
        //Movement
        lastInput = KeyCode.D;
        currentInput = lastInput;
        duration = 0.25f;
        tweener = gameObject.GetComponent<Tweener>();

        //Raycasting and collisions
        PacmanIsMoving = true;
        firstWallCollision = true;

        //Teleportation
        leftTeleport = new Vector3(-4.0f, -56.0f, pacman.transform.position.z);
        rightTeleport = new Vector3(112.0f, -56.0f, pacman.transform.position.z);
        justTeleported = false;

        //Pacman state
        CurrentState = PacState.Normal;
        PowerTime = 10.0f;

        //Ghost Trackers
        GhostIsDead = false;
        ghostDeathTimer = new float[] { 5.0f, 5.0f, 5.0f, 5.0f };
        PlayDeathSound = false;

        //Keycode to Direction Dictionary
        keyToDirection.Add(KeyCode.W, Vector2.up);
        keyToDirection.Add(KeyCode.S, Vector2.down);
        keyToDirection.Add(KeyCode.A, Vector2.left);
        keyToDirection.Add(KeyCode.D, Vector2.right);

        //Keycode to Endposition Dictionary
        keyToEndPosition.Add(KeyCode.W, new Vector3(0.0f, 4.0f, 0.0f));
        keyToEndPosition.Add(KeyCode.A, new Vector3(-4.0f, 0.0f, 0.0f));
        keyToEndPosition.Add(KeyCode.S, new Vector3(0.0f, -4.0f, 0.0f));
        keyToEndPosition.Add(KeyCode.D, new Vector3(4.0f, 0.0f, 0.0f));

        //Keycode to Rotation Dictionary
        keyToRotation.Add(KeyCode.W, Quaternion.Euler(0.0f, 0.0f, 90.0f));
        keyToRotation.Add(KeyCode.A, Quaternion.identity);
        keyToRotation.Add(KeyCode.S, Quaternion.Euler(0.0f, 0.0f, 90.0f));
        keyToRotation.Add(KeyCode.D, Quaternion.identity);

        //Keycode to LocalScale Dictionary
        keyToLocalScale.Add(KeyCode.W, new Vector3(1.0f, -1.0f, 0.0f));
        keyToLocalScale.Add(KeyCode.A, new Vector3(-1.0f, 1.0f, 0.0f));
        keyToLocalScale.Add(KeyCode.S, new Vector3(-1.0f, -1.0f, 0.0f));
        keyToLocalScale.Add(KeyCode.D, new Vector3(1.0f, 1.0f, 1.0f));
    }

    private void Update()
    {
        //Only run start everything if the start countdown is finished or if the game is not finished
        if (HUDManager.StartCounter <= -1.0f && HUDManager.LivesCount != -1)
        {


            //Lerp to move pacman from one position to the next if there isn't a wall
            if (Input.GetKey(KeyCode.W) && !nextToWall(keyToDirection[KeyCode.W]))
                movePacman(KeyCode.W);
            else if (Input.GetKey(KeyCode.A) && !nextToWall(keyToDirection[KeyCode.A]))
                movePacman(KeyCode.A);
            else if (Input.GetKey(KeyCode.S) && !nextToWall(keyToDirection[KeyCode.S]))
                movePacman(KeyCode.S);
            else if (Input.GetKey(KeyCode.D) && !nextToWall(keyToDirection[KeyCode.D]))
                movePacman(KeyCode.D);
            else if (!nextToWall(keyToDirection[lastInput]) && CurrentState != PacState.Dead)
            {
                currentInput = lastInput;
                movePacman(lastInput);
            }
            else if (!nextToWall(keyToDirection[currentInput]) && CurrentState != PacState.Dead)
                movePacman(currentInput);
            else
            {
                PacmanIsMoving = false;

                //Play the collision particle animation the first time that pacman hits a wall
                if (firstWallCollision && !tweener.tweenInProgress() && CurrentState != PacState.Dead)
                {
                    firstWallCollision = false;
                    GameObject collisionParticle = Instantiate(wallParticle, (pacman.transform.position + keyToEndPosition[lastInput]), Quaternion.identity);
                    Destroy(collisionParticle, 1);
                }
            }

            //Raycast for pickups
            raycastPickup();

            //Teleport pacman if he reaches the teleport zones and prevent the player from moving off the map
            if (pacman.transform.position == leftTeleport && !justTeleported)
            {
                tweener.RemoveTween();
                justTeleported = true;
                pacman.transform.position = rightTeleport;
                movePacman(KeyCode.A);
            }
            else if (pacman.transform.position == rightTeleport && !justTeleported)
            {
                tweener.RemoveTween();
                justTeleported = true;
                pacman.transform.position = leftTeleport;
                movePacman(KeyCode.D);
            }

            //Power Pill effects
            if (pickupHit.collider != null)
            {
                if (pickupHit.collider.gameObject.tag == "PowerPill" && pickupHit.distance < 2.0f)
                {
                    //Start a timer for 10 seconds
                    PowerTime = 10.0f;
                    CurrentState = PacState.Powered;

                    //Change ghost animator to scared
                    foreach (GameObject ghost in ghosts)
                    {
                        ghost.GetComponent<Animator>().SetBool("isScared", true);
                        ghost.GetComponent<Animator>().SetBool("isRecovering", false);
                    }

                }
            }

            //Update timer if current state is powered
            if (CurrentState == PacState.Powered)
            {
                PowerTime -= Time.deltaTime;

                //Change ghosts to recovering state if timer has 3 seconds left
                foreach (GameObject ghost in ghosts)
                {
                    if (PowerTime <= 3.0f && !ghost.GetComponent<Animator>().GetBool("isRecovering"))
                    {
                        ghost.GetComponent<Animator>().SetBool("isRecovering", true);
                        ghost.GetComponent<Animator>().SetBool("isScared", false);
                    }

                    //Go back to normal state if time runs out
                    else if (PowerTime <= 0.0f)
                    {
                        ghost.GetComponent<Animator>().SetBool("isRecovering", false);
                        CurrentState = PacState.Normal;
                    }
                }
            }

            //Raycast for ghosts
            raycastGhost();

            //Pacman collisions with ghosts
            if (ghostHit.collider != null)
            {
                if (ghostHit.distance <= 1.8f)
                {
                    //Set state to dead if pacman is touching ghost and ghost isnt in scared state
                    if (!ghostHit.collider.gameObject.GetComponent<Animator>().GetBool("isScared") && !ghostHit.collider.gameObject.GetComponent<Animator>().GetBool("isRecovering") && !ghostHit.collider.gameObject.GetComponent<Animator>().GetBool("isDead"))
                    {
                        CurrentState = PacState.Dead;
                        HUDManager.LivesCount = HUDManager.LivesCount - 1;

                        //Death particles
                        GameObject currentDeathParticles = Instantiate(pacDeathParticles, pacman.transform.position, Quaternion.identity);
                        Destroy(currentDeathParticles, 2);

                        //Reset position
                        tweener.RemoveTween();
                        pacman.transform.position = new Vector3(4.0f, -4.0f, pacman.transform.position.z);
                    }

                    //If ghost is scared or recovering, set ghost to dead
                    else if ((ghostHit.collider.gameObject.GetComponent<Animator>().GetBool("isScared") || ghostHit.collider.gameObject.GetComponent<Animator>().GetBool("isRecovering")) && !ghostHit.collider.gameObject.GetComponent<Animator>().GetBool("isDead"))
                    {
                        ghostHit.collider.gameObject.GetComponent<Animator>().SetBool("isScared", false);
                        ghostHit.collider.gameObject.GetComponent<Animator>().SetBool("isRecovering", false);
                        ghostHit.collider.gameObject.GetComponent<Animator>().SetBool("isDead", true);
                        GhostIsDead = true;
                        PlayDeathSound = true;
                    }
                }
            }

            //Update ghost death timers
            for (int i = 0; i < ghosts.Length; i++)
            {
                //Reduce timer if ghosts state is dead
                if (ghosts[i].GetComponent<Animator>().GetBool("isDead"))
                    ghostDeathTimer[i] -= Time.deltaTime;
                //Send ghost back to normal state if death timer is up
                if (ghostDeathTimer[i] <= 0.0f)
                {
                    ghosts[i].GetComponent<Animator>().SetBool("isDead", false);
                    ghostDeathTimer[i] = 5.0f;

                    //PLACEHOLDER TO MAKE ANIMATIONS CYCLE
                    ghosts[i].GetComponent<Animator>().SetBool("isUp", true);
                }
            }
        }
    }

    private void movePacman(KeyCode keyPressed)
    {
        PacmanIsMoving = true;
        firstWallCollision = true;
        lastInput = keyPressed;
        if (pacman.transform.position.x >= leftTeleport.x + 2.0f && pacman.transform.position.x <= rightTeleport.x - 2.0f)
            justTeleported = false;
        if (CurrentState == PacState.Dead)
            CurrentState = PacState.Normal;

        tweener.AddTween(pacman.transform, pacman.transform.position, pacman.transform.position + keyToEndPosition[keyPressed], duration);

        //Only adjust rotation and scale if they match pacmans current movement direction
        if (tweener.compareEndPos(pacman.transform.position + keyToEndPosition[keyPressed]))
        {
            pacman.transform.rotation = keyToRotation[keyPressed];
            pacman.transform.localScale = keyToLocalScale[keyPressed];
        }

    }

    private bool nextToWall(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(pacman.transform.position, direction, 4.0f, LayerMask.GetMask("Walls"));
        //If the raycast hits something less than 4 units away
        if (hit.collider != null)
        {
            //It is too close to a wall to tween
            return true;
        }
        //It is far away enough to tween
        return false;
    }

    private void raycastPickup()
    {
        //Raycast for pickup
        RaycastHit2D hit = Physics2D.Raycast(pacman.transform.position, keyToDirection[lastInput], 4.0f, LayerMask.GetMask("Pickups"));
        pickupHit = hit;
    }

    private void raycastGhost()
    {
        //Raycast for ghosts
        RaycastHit2D hit = Physics2D.Raycast(pacman.transform.position, keyToDirection[lastInput], 4.0f, LayerMask.GetMask("Ghosts"));
        ghostHit = hit;
    }

    public static bool tweening()
    {
        return tweener.tweenInProgress();
    }
}
