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
            {1,2,2,2,2,2,2,2,2,2,2,2,2,7,7,2,2,2,2,2,2,2,2,2,2,2,2,1},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,4,4,5,5,5,5,5,5,5,5,5,5,5,5,2},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,4,4,5,3,4,4,4,3,5,3,4,4,3,5,2},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,3,3,5,3,4,4,4,3,5,3,4,4,3,5,2},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,2},
            {2,5,3,4,4,3,5,3,3,5,3,4,4,4,4,4,4,3,5,3,3,5,3,4,4,3,5,2},
            {2,5,3,4,4,3,5,4,4,5,3,4,4,3,3,4,4,3,5,4,4,5,3,4,4,3,5,2},
            {2,5,5,5,5,5,5,4,4,5,5,5,5,4,4,5,5,5,5,4,4,5,5,5,5,5,5,2},
            {1,2,2,2,2,1,5,4,3,4,4,3,0,4,4,0,3,4,4,3,4,5,1,2,2,2,2,1},
            {0,0,0,0,0,2,5,4,3,4,4,3,0,3,3,0,3,4,4,3,4,5,2,0,0,0,0,0},
            {0,0,0,0,0,2,5,4,4,0,0,0,0,0,0,0,0,0,0,4,4,5,2,0,0,0,0,0},
            {0,0,0,0,0,2,5,4,4,0,3,4,4,0,0,4,4,3,0,4,4,5,2,0,0,0,0,0},
            {2,2,2,2,2,1,5,3,3,0,4,0,0,0,0,0,0,4,0,3,3,5,1,2,2,2,2,2},
            {0,0,0,0,0,0,5,0,0,0,4,0,0,0,0,0,0,4,0,0,0,5,0,0,0,0,0,0},
            {2,2,2,2,2,1,5,3,3,0,4,0,0,0,0,0,0,4,0,3,3,5,1,2,2,2,2,2},
            {0,0,0,0,0,2,5,4,4,0,3,4,4,0,0,4,4,3,0,4,4,5,2,0,0,0,0,0},
            {0,0,0,0,0,2,5,4,4,0,0,0,0,0,0,0,0,0,0,4,4,5,2,0,0,0,0,0},
            {0,0,0,0,0,2,5,4,3,4,4,3,0,3,3,0,3,4,4,3,4,5,2,0,0,0,0,0},
            {1,2,2,2,2,1,5,4,3,4,4,3,0,4,4,0,3,4,4,3,4,5,1,2,2,2,2,1},
            {2,5,5,5,5,5,5,4,4,5,5,5,5,4,4,5,5,5,5,4,4,5,5,5,5,5,5,2},
            {2,5,3,4,4,3,5,4,4,5,3,4,4,3,3,4,4,3,5,4,4,5,3,4,4,3,5,2},
            {2,5,3,4,4,3,5,3,3,5,3,4,4,4,4,4,4,3,5,3,3,5,3,4,4,3,5,2},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,2},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,3,3,5,3,4,4,4,3,5,3,4,4,3,5,2},
            {2,6,4,0,0,4,5,4,0,0,0,4,5,4,4,5,4,0,0,0,4,5,4,0,0,4,6,2},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,4,4,5,3,4,4,4,3,5,3,4,4,3,5,2},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,4,4,5,5,5,5,5,5,5,5,5,5,5,5,2},
            {1,2,2,2,2,2,2,2,2,2,2,2,2,7,7,2,2,2,2,2,2,2,2,2,2,2,2,1},
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
                        Instantiate(outsideCorner, new Vector3(j * 4, -i * 4), checkAngle(i, j, 1), gameObject.transform);
                        break;
                    case 2:
                        Instantiate(outsideWall, new Vector3(j * 4, -i * 4), checkAngle(i, j, 2), gameObject.transform);
                        break;
                    case 3:
                        Instantiate(insideCorner, new Vector3(j * 4, -i * 4), checkAngle(i, j, 3), gameObject.transform);
                        break;
                    case 4:
                        Instantiate(insideWall, new Vector3(j * 4, -i * 4), checkAngle(i, j, 4), gameObject.transform);
                        break;
                    case 5:
                        Instantiate(standardPellet, new Vector3(j * 4, -i * 4), Quaternion.identity, gameObject.transform);
                        break;
                    case 6:
                        Instantiate(powerPellet, new Vector3(j * 4, -i * 4), Quaternion.identity, gameObject.transform);
                        break;
                    case 7:
                        Instantiate(tJunction, new Vector3(j * 4, -i * 4), checkAngle(i, j, 7), gameObject.transform);
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Quaternion checkAngle(int i, int j, int blockType)
    {
        int up;
        int right;
        int down;
        int left;

        if (i == 0)
            up = 0;
        else
            up = levelMap[i - 1, j];
        if (j == levelMap.GetLength(1) - 1)
            right = 0;
        else
            right = levelMap[i, j + 1];
        if (i == levelMap.GetLength(0) - 1)
            down = 0;
        else
            down = levelMap[i + 1, j];
        if (j == 0)
            left = 0;
        else
            left = levelMap[i, j - 1];

        //Set the first row of blocks
        if (i == 0)
        {
            //Outside corner block
            if (blockType == 1 && j != 0)
            {
                if (left == 2 || left == 7)
                    return Quaternion.Euler(0, 0, 270);
            }
            //Outside Wall
            else if (blockType == 2)
            {
                if (left == 0 || left == 5 || left == 6 || right == 0 || right == 5 || right == 6)
                    return Quaternion.identity;
                return Quaternion.Euler(0, 0, 90);
            }

        }
        //Set the first column of blocks
        else if (j == 0)
        {
            //Outside Corner block
            if (blockType == 1)
            {
                if (up == 2 || up == 7)
                    return Quaternion.Euler(0, 0, 90);
            }
            //T-Junction
            else if (blockType == 7)
                return Quaternion.Euler(0, 0, 270);
            //Outside wall
            else if (blockType == 2)
            {
                if (up == 0 || up == 5 || up == 6 || down == 0 || down == 5 || down == 6)
                    return Quaternion.Euler(0, 0, 90);
            }
        }
        //Set the last row of blocks
        else if (i == levelMap.GetLength(0) - 1)
        {
            //Outside Corner
            if (blockType == 1)
            {
                if (j == levelMap.GetLength(1) - 1)
                    return Quaternion.Euler(0, 0, 180);
                else if ((up != 5 || up != 6 || up != 0) && (left != 5 || left != 6 || left != 0))
                    return Quaternion.Euler(0, 0, 90);
                else if ((up != 5 || up != 6 || up != 0) && (right != 5 || right != 6 || right != 0))
                    return Quaternion.Euler(0, 0, 180);
            }
            //Outside Wall
            else if (blockType == 2)
            {
                if (left == 0 || left == 5 || left == 6 || right == 0 || right == 5 || right == 6)
                    return Quaternion.identity;
                return Quaternion.Euler(0, 0, 90);
            }
            //T Junction
            else if (blockType == 7)
                return Quaternion.Euler(0, 0, 180);
        }
        //Set the last column of blocks
        else if (j == levelMap.GetLength(1) - 1)
        {
            //Outside Corner
            if (blockType == 1)
            {
                if (down != 5 && down != 6 && down != 0 && left != 5 && left != 6 && left != 0)
                {
                    return Quaternion.Euler(0, 0, 270);
                }
                else if (up != 5 && up != 6 && up != 0 && left != 5 && left != 6 && left != 0)
                    return Quaternion.Euler(0, 0, 180);
            }
            //Outside Wall
            else if (blockType == 2)
            {
                if (up == 0 || up == 5 || up == 6 || down == 0 || down == 5 || down == 6)
                    return Quaternion.Euler(0, 0, 90);
            }
            else if (blockType == 7)
                return Quaternion.Euler(0, 0, 270);
        }
        //Set the inner block rotations
        else {
            //Corners
            if (blockType == 1 || blockType == 3)
            {
                //Fix corners with walls on all sides
                if (up != 6 && up != 5 && up != 0 && right != 6 && right != 5 && right != 0 && down != 6 && down != 5 && down != 0 && left != 6 && left != 5 && left != 0)
                {
                    if (up == 4 && right == 4 && down == 3)
                    {
                        if (levelMap[i, j - 2] == 0 || levelMap[i, j - 2] == 5 || levelMap[i, j - 2] == 6)
                            return Quaternion.Euler(0, 0, 90);
                        return Quaternion.Euler(0, 0, 180);
                    }
                    else if (left == 4 && right == 4 && up == 3)
                    {
                        if (levelMap[i, j + 2] == 0 || levelMap[i, j + 2] == 5 || levelMap[i, j + 2] == 6)
                            return Quaternion.Euler(0, 0, 270);
                        return Quaternion.identity;
                    }
                    else if (up == 4 && left == 4 && right == 3)
                    {
                        if (levelMap[i + 2, j] == 0 || levelMap[i + 2, j] == 5 || levelMap[i + 2, j] == 6)
                            return Quaternion.Euler(0, 0, 180);
                        return Quaternion.Euler(0, 0, 270);
                    }
                    else if (up == 4 && right == 4 && left == 3)
                    {
                        if (levelMap[i - 2, j] == 0 || levelMap[i - 2, j] == 5 || levelMap[i - 2, j] == 6)
                            return Quaternion.identity;
                        return Quaternion.Euler(0, 0, 90);
                    }
                }
                else
                {
                    if (up != 6 && up != 5 && up != 0 && right != 6 && right != 5 && right != 0)
                        return Quaternion.Euler(0, 0, 90);
                    else if (down != 6 && down != 5 && down != 0 && left != 6 && left != 5 && left != 0)
                        return Quaternion.Euler(0, 0, 270);
                    else if (up != 6 && up != 5 && up != 0 && left != 6 && left != 5 && left != 0)
                        return Quaternion.Euler(0, 0, 180);
                }
            }
            //Walls
            else if (blockType == 2 || blockType == 4)
            {
                if (left != 6 && left != 5 && left != 0 && right != 6 && right != 5 && right != 0)
                    return Quaternion.Euler(0, 0, 90);
                else if (up == 0 && down == 0 && (left == 0 || right == 0))
                    return Quaternion.Euler(0, 0, 90);
            }
            //T Junction
            else if (blockType == 7)
            {
                if (up != 6 && up != 5 && right != 6 && right != 5 && down != 6 && down != 5)
                    return Quaternion.Euler(0, 0, 90);
                else if (up != 6 && up != 5 && right != 6 && right != 5 && left != 6 && left != 5)
                    return Quaternion.Euler(0, 0, 180);
                else if (up != 6 && up != 5 && left != 6 && left != 5 && down != 6 && down != 5)
                    return Quaternion.Euler(0, 0, 270);
            }
        }
        //All other cases are correct rotation
        return Quaternion.identity;
    }
}
