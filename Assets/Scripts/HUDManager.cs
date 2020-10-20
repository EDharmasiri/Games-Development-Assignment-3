using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private int score;
    private RaycastHit2D pickupHit;
    public GameObject lives;
    public GameObject scoreKeeper;
    public GameObject gameTimer;
    public GameObject ghostScaredTimer;

    public static int LivesCount { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        LivesCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        pickupHit = PacStudentController.PickupHit;

        if (pickupHit.collider != null && pickupHit.distance < 0.1f)
        {
            if (pickupHit.collider.gameObject.tag == "Pellet")
            {
                Destroy(pickupHit.collider.gameObject);
                score += 10;
            }

            else if (pickupHit.collider.gameObject.tag == "BonusCherry")
            {
                Destroy(pickupHit.collider.gameObject);
                score += 100;
            }

            else if (pickupHit.collider.gameObject.tag == "PowerPill")
            {
                Destroy(pickupHit.collider.gameObject);
            }
        }

        //Show the ghost scared timer if pacman is powered
        if (PacStudentController.CurrentState == PacStudentController.PacState.Powered)
        {
            ghostScaredTimer.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 0.0f);
        }
        else
            ghostScaredTimer.GetComponent<RectTransform>().localScale = new Vector3(0.0f, 0.0f, 0.0f);


        scoreKeeper.GetComponent<UnityEngine.UI.Text>().text = score.ToString("00000");
        ghostScaredTimer.GetComponent<UnityEngine.UI.Text>().text = PacStudentController.PowerTime.ToString();

        //Update Lives UI when pacman dies
        lives.GetComponent<RectTransform>().sizeDelta = new Vector2(lives.GetComponent<RectTransform>().sizeDelta.y * LivesCount, lives.GetComponent<RectTransform>().sizeDelta.y);
    }
}
