using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager saveManagerInstance;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        if (saveManagerInstance == null)
            saveManagerInstance = this;
        else
            Destroy(gameObject);


        LoadScores();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SaveScores()
    {
        if (PlayerPrefs.GetInt("HighScore") < HUDManager.Score || (PlayerPrefs.GetInt("HighScore") == HUDManager.Score && PlayerPrefs.GetFloat("Time") < HUDManager.GameTime))
        {
            PlayerPrefs.SetInt("HighScore", HUDManager.Score);
            PlayerPrefs.SetFloat("Time", HUDManager.GameTime);
        }
        
        PlayerPrefs.Save();
    }

    public static void LoadScores()
    {
        if (PlayerPrefs.GetInt("HighScore") != 0)
            TitleScreenManager.highScore = PlayerPrefs.GetInt("HighScore");
        if (PlayerPrefs.GetFloat("Time") != 0)
            TitleScreenManager.time = PlayerPrefs.GetFloat("Time");
    }
}
