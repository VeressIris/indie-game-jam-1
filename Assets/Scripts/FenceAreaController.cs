using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FenceAreaController : MonoBehaviour
{
    private Vector2 topLeftCorner;
    private Vector2 bottomRightCorner;
    [SerializeField] private BoxCollider2D fenceColl;
    [SerializeField] private GameManager gameManager;
    private List<Vector2> previousPos = new List<Vector2>();
    private Collider2D[] collisions;
    private SheepController[] sheep;
    [SerializeField] private LayerMask sheepLayer;
    [SerializeField] private Vector3 overlapOffset;

    void Start()
    {
        GetColliderLimits(fenceColl);
    }

    void Update()
    {
        MoveSheepToPositions();
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

    bool SheepInFence()
    {
        collisions = Physics2D.OverlapAreaAll(topLeftCorner, bottomRightCorner, sheepLayer);
        return collisions.Length > 0;
    }

    void MoveSheepToPositions()
    {
        if (SheepInFence())
        { 
            sheep = new SheepController[collisions.Length];
            for (int i = 0; i < collisions.Length; i++)
            {
                sheep[i] = collisions[i].gameObject.GetComponent<SheepController>();

                if (!sheep[i].arrivedAtFinalPos && !sheep[i].movingTowardsFinalPos && sheep[i].isUnderPlayerInteraction)
                {
                    sheep[i].isInFence = true;
                    sheep[i].destination = GetRandomFencePos(1.2f);
                    sheep[i].movingTowardsFinalPos = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Color customColor = new Color(0, 1, 1, 0.25f);
        Gizmos.color = customColor;
        Gizmos.DrawCube(transform.position + overlapOffset, gameObject.GetComponent<BoxCollider2D>().size);
    }
}