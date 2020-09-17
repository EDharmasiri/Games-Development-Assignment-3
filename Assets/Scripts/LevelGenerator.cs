using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject outsideCorner;
    public GameObject outsideWall;
    public GameObject insideCorner;
    public GameObject insideWall;
    public GameObject standardPellet;
    public GameObject powerPellet;
    public GameObject tJunction;
    
    int[,] levelMap =
        {
            {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
            {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
            {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
            {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
            {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
            {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
            {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
            {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
            {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
            {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
            {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
        };

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < levelMap.GetLength(0); i++) {
            for (int j = 0; j < levelMap.GetLength(1); j++) {
                switch(levelMap[i, j])
                {
                    case 0:
                        break;
                    case 1:
                        Instantiate(outsideCorner, new Vector3(j * 4, -i * 4), Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(outsideWall, new Vector3(j * 4, -i * 4), Quaternion.identity);
                        break;
                    case 3:
                        Instantiate(insideCorner, new Vector3(j * 4, -i * 4), Quaternion.identity);
                        break;
                    case 4:
                        Instantiate(insideWall, new Vector3(j * 4, -i * 4), Quaternion.identity);
                        break;
                    case 5:
                        Instantiate(standardPellet, new Vector3(j * 4, -i * 4), Quaternion.identity);
                        break;
                    case 6:
                        Instantiate(powerPellet, new Vector3(j * 4, -i * 4), Quaternion.identity);
                        break;
                    case 7:
                        Instantiate(tJunction, new Vector3(j * 4, -i * 4), Quaternion.identity);
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Quaternion checkAngle(int i, int j) {
        return Quaternion.identity;
    }
}
