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
    public float detectionRadius = 8f;
    [SerializeField] private LayerMask sheepLayer;
    private Collider2D[] collisions;
    private SheepController[] sheep;
    [SerializeField] private Transform fenceTarget;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SpriteRenderer detectionRadiusSR;
    private Color idleDetColor = new Color(1, 0, 0, 0.14f);
    private Color detectedColor = new Color(0, 1, 0, 0.08f);

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
        if (canMove) 
        { 
            transform.position = Vector2.Lerp(transform.position, mousePos, speed * Time.deltaTime);
        }
    }

    Vector2 GetMouseWorldPos()
    {
        return mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    void HerdSheep()
    {
        if (DetectSheep())
        {
            detectionRadiusSR.color = detectedColor;

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
            detectionRadiusSR.color = idleDetColor;

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
