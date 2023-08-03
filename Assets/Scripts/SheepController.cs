using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    private Vector2 randomPos;
    private float speed;
    private bool canMove = true;

    void Start()
    {
        randomPos = GameManager.Instance.GetRandomPos(3.25f, transform);

        speed = Random.Range(0.8f, 1.5f);
    }

    void Update()
    {
        if (canMove) 
        { 
            RotateToFaceDestination();
            transform.position = Vector2.MoveTowards(transform.position, randomPos, speed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, randomPos) <= 0.5f && canMove)
        {
            canMove = false;
            StartCoroutine(GetNewRandomPos());
        }
    }

    IEnumerator GetNewRandomPos()
    {
        yield return new WaitForSeconds(2.5f);
        randomPos = GameManager.Instance.GetRandomPos(3.25f, transform);
        canMove = true;
    }

    void RotateToFaceDestination()
    {
        Vector2 dir = new Vector2(randomPos.x - transform.position.x, randomPos.y - transform.position.y);
        transform.up = dir;
    }
}
