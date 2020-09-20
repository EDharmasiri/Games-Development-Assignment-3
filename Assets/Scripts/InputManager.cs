using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pacman;
    private Tweener tweener;
    private Vector3 topLeft;
    private Vector3 topRight;
    private Vector3 bottomLeft;
    private Vector3 bottomRight;
    // Start is called before the first frame update
    void Start()
    {
        topLeft = new Vector3(4.0f, -4.0f, -1.0f);
        topRight = new Vector3(24.0f, -4.0f, -1.0f);
        bottomLeft = new Vector3(4.0f, -20.0f, -1.0f);
        bottomRight = new Vector3(24.0f, -20.0f, -1.0f);

        tweener = GetComponent<Tweener>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(pacman.transform.position, topLeft) <= 0.1f)
        {
            tweener.AddTween(pacman.transform, pacman.transform.position, topRight, 1.0f);
            pacman.transform.rotation = Quaternion.identity;
            pacman.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else if (Vector3.Distance(pacman.transform.position, topRight) <= 0.1f)
        {
            tweener.AddTween(pacman.transform, pacman.transform.position, bottomRight, 1.0f);
            pacman.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            pacman.transform.localScale = new Vector3(-1.0f, -1.0f, 0.0f);
        }
            
        else if (Vector3.Distance(pacman.transform.position, bottomRight) <= 0.1f)
        {
            tweener.AddTween(pacman.transform, pacman.transform.position, bottomLeft, 1.0f);
            pacman.transform.rotation = Quaternion.identity;
            pacman.transform.localScale = new Vector3(-1.0f, -1.0f, 0.0f);
        }
            
        else if (Vector3.Distance(pacman.transform.position, bottomLeft) <= 0.1f)
        {
            tweener.AddTween(pacman.transform, pacman.transform.position, topLeft, 1.0f);
            pacman.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            pacman.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
        }
            
    }
}
