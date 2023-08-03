using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private float speed = 10f;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        FollowMousePos();
    }

    void FollowMousePos()
    {
        Vector2 mousePos = GetMouseWorldPos();
        
        //follow
        transform.position = Vector2.Lerp(transform.position, mousePos, speed * Time.deltaTime);

        //rotate
        Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        transform.up = dir;
    }

    Vector2 GetMouseWorldPos()
    {
        return mainCam.ScreenToWorldPoint(Input.mousePosition);
    }
}
