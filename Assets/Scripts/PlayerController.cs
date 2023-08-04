using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private float speed = 5f;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public Vector2 mousePos;

    [Header("Sheep detection:")]
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private LayerMask sheepLayer;
    private Collider2D[] collisions;
    private SheepController[] sheep;
    [SerializeField] private Transform fenceTarget;
    [SerializeField] private GameManager gameManager;

    void Start()
    {
        mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        FollowMousePos();

        HerdSheep();
    }

    void FollowMousePos()
    {
        mousePos = GetMouseWorldPos();

        //follow
        if (canMove) 
        { 
            transform.position = Vector2.Lerp(transform.position, mousePos, speed * Time.deltaTime);
        }
        
        //rotate
        Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        transform.up = dir;
    }

    Vector2 GetMouseWorldPos()
    {
        return mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    void HerdSheep()
    {
        if (DetectSheep())
        {
            sheep = new SheepController[collisions.Length];
            for (int i = 0; i < collisions.Length; i++)
            {
                sheep[i] = collisions[i].gameObject.GetComponent<SheepController>();
                if (!sheep[i].movingTowardsFence) 
                { 
                    sheep[i].canMove = true;
                    sheep[i].destination = fenceTarget.position;
                } 
            }
        }
        else
        {
            if (sheep != null && sheep.Length > 0)
            {
                for (int i = 0; i < sheep.Length; i++)
                {
                    sheep[i].movingTowardsFence = false;
                    sheep[i].destination = gameManager.GetRandomPos(sheep[i].minSheepDistance, sheep[i].gameObject.transform);
                }
            }
            sheep = new SheepController[0]; //reset collision array
        }
    }
     
    bool DetectSheep()
    {
        collisions = Physics2D.OverlapCircleAll(transform.position, detectionRadius, sheepLayer);
        return collisions.Length > 0;
    }

    private void OnDrawGizmos()
    {
        Color idleColor = new Color(0, 1, 1, 0.25f);
        Color detectionColor = new Color(1, 0, 0, 0.25f);
        
        Gizmos.color = DetectSheep() ? detectionColor : idleColor;
        
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }
}
