using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    public float minSheepDistance = 3.25f;

    private Vector2 randomPos;
    private float speed;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public Vector2 destination;
    [HideInInspector] public bool movingTowardsFence = false;

    void Start()
    {
        randomPos = GameManager.Instance.GetRandomPos(minSheepDistance, transform);
        destination = randomPos;

        speed = Random.Range(0.8f, 1.5f);
    }

    void Update()
    {
        if (canMove)
        { 
            RotateToFaceDestination();
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }

        //find new destination
        if (Vector2.Distance(transform.position, destination) <= 0.5f && canMove)
        {
            canMove = false;
            if (!movingTowardsFence) StartCoroutine(GetNewRandomPos());
        }
    }

    IEnumerator GetNewRandomPos()
    {
        yield return new WaitForSeconds(2.5f);

        randomPos = GameManager.Instance.GetRandomPos(minSheepDistance, transform);
        destination = randomPos;
        canMove = true;
    }

    void RotateToFaceDestination()
    {
        Vector2 dir = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        transform.up = dir;
    }
}
