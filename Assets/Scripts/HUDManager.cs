using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private RaycastHit2D pickupHit;
    public GameObject lives;
    public GameObject scoreKeeper;
    public GameObject gameTimer;
    public GameObject ghostScaredTimer;
    public GameObject startCountdown;
    public GameObject gameOverScreen;
    private GameObject currentCountdown;
    GameObject currentGameOverScreen;

    public static int LivesCount { get; set; }
    public static float StartCounter { get; private set; }

    public static int Score { get; private set; }
    public static float GameTime { get; private set; }

    public static bool GameOver { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        LivesCount = 2;
        StartCounter = 3.0f;
        GameTime = 0.0f;
        GameOver = false;
        currentCountdown = Instantiate(startCountdown, gameObject.transform);
        currentCountdown.GetComponent<UnityEngine.UI.Text>().text = Mathf.CeilToInt(StartCounter).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (StartCounter <= -1.0f)
        {
            if (currentCountdown != null)
                Destroy(currentCountdown);

            //Start the game timer
            GameTime += Time.deltaTime;
            gameTimer.GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt(GameTime / 60), Mathf.FloorToInt(GameTime), Mathf.FloorToInt((GameTime - Mathf.FloorToInt(GameTime)) * 100));
        }

        else if (StartCounter <= 0.0f && StartCounter > -1.0f)
        {
            StartCounter -= Time.deltaTime;
            currentCountdown.GetComponent<UnityEngine.UI.Text>().text = "GO!";
        }
        else if (StartCounter > 0.0f)
        {
            StartCounter -= Time.deltaTime;
            currentCountdown.GetComponent<UnityEngine.UI.Text>().text = Mathf.CeilToInt(StartCounter).ToString();
        }

        pickupHit = PacStudentController.PickupHit;

        if (pickupHit.collider != null && pickupHit.distance < 0.1f)
        {
            if (pickupHit.collider.gameObject.tag == "Pellet")
            {
                Destroy(pickupHit.collider.gameObject);
                Score += 10;
            }

            else if (pickupHit.collider.gameObject.tag == "BonusCherry")
            {
                Destroy(pickupHit.collider.gameObject);
                Score += 100;
            }

            else if (pickupHit.collider.gameObject.tag == "PowerPill")
            {
                Destroy(pickupHit.collider.gameObject);
            }
        }

        //Add 300 points if ghost is dead
        if (PacStudentController.GhostIsDead)
        {
            Score += 300;
            PacStudentController.GhostIsDead = false;
        }

        //Show the ghost scared timer if pacman is powered
        if (PacStudentController.CurrentState == PacStudentController.PacState.Powered)
        {
            ghostScaredTimer.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 0.0f);
        }
        else
            ghostScaredTimer.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f, 0.0f);


        scoreKeeper.GetComponent<UnityEngine.UI.Text>().text = Score.ToString("00000");
        ghostScaredTimer.GetComponent<UnityEngine.UI.Text>().text = PacStudentController.PowerTime.ToString();

        //Update Lives UI when pacman dies
        lives.GetComponent<RectTransform>().sizeDelta = new Vector2(lives.GetComponent<RectTransform>().sizeDelta.y * LivesCount, lives.GetComponent<RectTransform>().sizeDelta.y);

        //Run saving method if game is over
        if (LivesCount == -1 || (GameObject.FindGameObjectsWithTag("Pellet").Length == 0 && GameObject.FindGameObjectsWithTag("PowerPill").Length == 0))
        {
            //Save if high score
            SaveManager.SaveScores();
            //Run the game over screen
            if (!GameOver)
            {
                currentGameOverScreen = Instantiate(gameOverScreen, gameObject.transform);
                Destroy(currentGameOverScreen, 3);
                GameOver = true;
            }

            //Go back to start screen
            if (currentGameOverScreen == null && GameOver)
                GameObject.FindObjectOfType<UISceneManager>().loadLevel(0);
        }
    }
}
