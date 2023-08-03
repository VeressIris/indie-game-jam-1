using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlayer : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private float bounceDuration = 0.2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController.canMove = false;

            playerRb.AddForce(transform.right * bounceForce, ForceMode2D.Impulse);
            
            Cursor.lockState = CursorLockMode.Locked;

            StartCoroutine(DisableBounce());
        }
    }

    IEnumerator DisableBounce()
    {        
        yield return new WaitForSeconds(bounceDuration);
        playerRb.velocity = Vector3.zero;

        yield return new WaitForSeconds(bounceDuration + 0.15f);
        playerController.canMove = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
