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
            sheepController.movingTowardsFence = true;
            //sheepController.destination = gameManager.GetRandomPos(sheepController.minSheepDistance, bottomRightCorner, topLeftCorner, previousPos);
            //previousPos.Add(sheepController.destination);
            sheepController.destination = gameManager.GetRandomPos(sheepController.minSheepDistance, sheep.transform);
        }
    }

    void GetColliderLimits(BoxCollider2D col)
    {
        Vector2 center = col.bounds.center;
        Vector2 extents = col.bounds.extents;

        topLeftCorner = new Vector2(center.x - extents.x, center.y + extents.y);
        bottomRightCorner = new Vector2(center.x + extents.x, center.y - extents.y);
    }
}
