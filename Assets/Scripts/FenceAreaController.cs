using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceAreaController : MonoBehaviour
{
    private Vector2 topLeftCorner;
    private Vector2 bottomRightCorner;
    [SerializeField] private BoxCollider2D fenceColl;
    [SerializeField] private GameManager gameManager;
    private List<Vector2> previousPos = new List<Vector2>();

    void Start()
    {
        GetColliderLimits(fenceColl);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep"))
        {
            GameObject sheep = collision.gameObject;
            SheepController sheepController = sheep.GetComponent<SheepController>();
            
            if (!sheepController.movingTowardsFence)
            {
                sheepController.movingTowardsFence = true;
                Vector2 fenceDestination = GetRandomFencePos(1.2f);
                previousPos.Add(fenceDestination);
                sheepController.destination = fenceDestination;
            }
        }
    }

    void GetColliderLimits(BoxCollider2D col)
    {
        Vector2 center = col.bounds.center;
        Vector2 extents = col.bounds.extents;

        topLeftCorner = new Vector2(center.x - extents.x, center.y + extents.y);
        bottomRightCorner = new Vector2(center.x + extents.x, center.y - extents.y);
    }

    public Vector2 GetRandomFencePos(float minDistance)
    {
        float xPos = Random.Range(topLeftCorner.x, bottomRightCorner.x);
        float yPos = Random.Range(bottomRightCorner.y, topLeftCorner.y);

        Vector2 vec = new Vector2(xPos, yPos);

        while (!ValidPoint(vec, minDistance))
        {
            xPos = Random.Range(topLeftCorner.x, bottomRightCorner.x);
            yPos = Random.Range(bottomRightCorner.y, topLeftCorner.y);

            vec = new Vector2(xPos, yPos);
        }

        return vec;
    }

    bool ValidPoint(Vector2 vec, float minDistance)
    {
        for (int i = 0; i < previousPos.Count; i++)
        {
            if (Vector2.Distance(vec, previousPos[i]) < minDistance)
            {
                return false;
            }
        }

        return true;
    }
}
