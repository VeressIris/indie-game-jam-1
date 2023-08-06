using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBatToMousePos : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    void Update()
    {
        RotateWithMouse();
    }

    void RotateWithMouse()
    {
        if (!GameManager.Instance.paused)
        {
            Vector2 mousePos = playerController.mousePos;
            Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
            transform.up = dir;
        }
    }
}
