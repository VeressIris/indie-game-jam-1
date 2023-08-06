using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    [SerializeField] private Transform fence;
    private bool canMove = false;
    [SerializeField] private float speed = 3.5f;
    private GameObject[] sheep;
    private Vector2 destination;
    [HideInInspector] public bool arrived = false;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Transform initSpawnPoint;
    private float delayToMove;
    private bool isAtSpawn = false;

    void Start()
    {
        sheep = GameObject.FindGameObjectsWithTag("Sheep");

        delayToMove = GameManager.Instance.levelDuration / 2;
        StartCoroutine(WaitToMoveToSpawn());
    }

    void Update()
    {
        if (canMove && !isAtSpawn) transform.position = Vector2.MoveTowards(transform.position, initSpawnPoint.position, speed * Time.deltaTime);
        else if (canMove && !arrived) transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        else if (!canMove && GameManager.Instance.gameOver)
        {
            StartCoroutine(WalkDelay());
        }
        else if (GameManager.Instance.timeOut)
        {
            destination = GetClosestSheep();
            if (!canMove) canMove = true;
        }

        if (Vector2.Distance(transform.position, initSpawnPoint.position) < 0.2f && !isAtSpawn) //arrived at spawn point
        {
            canMove = false;
            isAtSpawn = true;
        }
        else if (Vector2.Distance(transform.position, destination) < 0.2f && !arrived && isAtSpawn)
        {
            canMove = false;
            arrived = true;
        }

        FaceTarget();
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

        return closestSheep;
    }

    void FaceTarget()
    {
        //flip accordingly
        if (destination.x < transform.position.x) sr.flipX = false;
        else sr.flipX = true;

        //rotate
        Vector2 dir = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        transform.up = dir;

        //clamp rotation
        float clampedRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        clampedRotation = Mathf.Clamp(clampedRotation, -14f, 9f);
        transform.rotation = Quaternion.Euler(0f, 0f, clampedRotation);
    }

    IEnumerator WaitToMoveToSpawn()
    {
        yield return new WaitForSeconds(delayToMove);
        canMove = true;
    }
}
