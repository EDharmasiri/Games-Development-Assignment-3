using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    private float startTime;
    private float timeCounter;
    public GameObject cherry;
    private GameObject currentCherry;
    private Vector3 startPos;
    private Vector3 endPos;
    private float duration;

    // Start is called before the first frame update
    void Start()
    {
        timeCounter = 0;
        startPos = new Vector3(-52, -56, 0);
        endPos = new Vector3(162, -56, 0);
        duration = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeCounter >= 30)
        {
            timeCounter = 0;
            //Instantiate the cherry
            currentCherry = Instantiate(cherry, new Vector3(-52, -56, 0), Quaternion.identity, gameObject.transform);

            //Set the start time for lerping
            startTime = Time.time;
        }


        if (currentCherry != null)
        {
            float timeFraction = (Time.time - startTime) / duration;
            currentCherry.transform.position = Vector3.Lerp(startPos, endPos, timeFraction);

            if (currentCherry != null)
                if (currentCherry.transform.position == endPos)
                    Destroy(currentCherry);
        }       

        timeCounter += Time.deltaTime;
    }
}