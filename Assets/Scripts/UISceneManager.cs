using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<UISceneManager>().Length < 2)
            DontDestroyOnLoad(this);
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void loadLevel(int index) {        
        SceneManager.LoadScene(index, LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void ExitGame()
    {
        loadLevel(0);
    }

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //If the scene is level 1
        if (scene.buildIndex == 1)
        {
            GameObject.FindGameObjectWithTag("ExitButton").GetComponent<Button>().onClick.AddListener(ExitGame);
        }
    }

}

