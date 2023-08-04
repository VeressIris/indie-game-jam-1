using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep"))
        {
            Debug.Log("Your sheep got eaten");
            GameManager.Instance.gameOver = true;
        }
    }
}
