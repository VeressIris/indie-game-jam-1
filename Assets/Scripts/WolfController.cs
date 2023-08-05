using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    [SerializeField] private Transform fence;
    private bool canMove = false;
    [SerializeField] private float speed = 3.2f;
    private GameObject[] sheep;
    private Vector2 destination;
    [HideInInspector] public bool arrived = false;

    void Start()
    {
        sheep = GameObject.FindGameObjectsWithTag("Sheep");
    }

    void Update()
    {
        if (canMove && !arrived) transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        else if (!canMove && GameManager.Instance.gameOver)
        {
            StartCoroutine(WalkDelay());
        }
        else if (GameManager.Instance.timeOut)
        {
            destination = GetClosestSheep();
            if (!canMove) canMove = true;
        }

        if (Vector2.Distance(transform.position, destination) < 0.2f && !arrived)
        {
            canMove = false;
            arrived = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep") && !GameManager.Instance.timeOut)
        {
            Debug.Log("Your sheep got eaten");
            GameManager.Instance.gameOver = true;
        }
    }

    IEnumerator WalkDelay()
    {
        destination = fence.position;

        yield return new WaitForSeconds(1.4f);

        canMove = true;
    }

    Vector2 GetClosestSheep()
    {
        float closestDist = 1000;
        Vector2 closestSheep = Vector2.zero;

        for (int i = 0;i < sheep.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, sheep[i].transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestSheep = sheep[i].transform.position;
            }
        }

        Debug.Log(closestSheep);
        return closestSheep;
    }
}
