using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject UiHighScore;
    public GameObject UiTime;

    public static int highScore { get; set; } = 0;
    public static float time { get; set; } = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        SaveManager.LoadScores();
    }

    // Update is called once per frame
    void Update()
    {
        UiTime.GetComponent<UnityEngine.UI.Text>().text = string.Format("Time: {0:00}:{1:00}:{2:00}", Mathf.FloorToInt(time / 60), Mathf.FloorToInt(time), Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 100));
        UiHighScore.GetComponent<UnityEngine.UI.Text>().text = string.Format("High Score: {0}", highScore);
    }
}
