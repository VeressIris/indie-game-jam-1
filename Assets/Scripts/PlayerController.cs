using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private float speed = 5f;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public Vector2 mousePos;

    void Start()
    {
        mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        FollowMousePos();
    }

    void FollowMousePos()
    {
        mousePos = GetMouseWorldPos();

        //follow
        if (canMove) 
        { 
            transform.position = Vector2.Lerp(transform.position, mousePos, speed * Time.deltaTime);
        }
        
        //rotate
        Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        transform.up = dir;
    }

    Vector2 GetMouseWorldPos()
    {
        return mainCam.ScreenToWorldPoint(Input.mousePosition);
    }
}
