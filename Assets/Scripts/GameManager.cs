using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level")]
    [SerializeField] private float levelDuration = 10f;
    [SerializeField] private GameObject sheep;
    [SerializeField] private int sheepNumber = 5;

    [Header("Screen Limits")]
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;
    [SerializeField] private Transform bottomLimit;
    [SerializeField] private Transform topLimit;

    private List<Vector2> previousSpawns = new List<Vector2>();
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnSheep();
    }

    void Update()
    {
        //timer
        if (levelDuration > 0) levelDuration -= Time.deltaTime;
        else Debug.Log("The wolf is coming");
        UpdateTimerUI();
    }

    void SpawnSheep()
    {
        for (int i = 0; i < sheepNumber; i++)
        {
            Vector2 randomPos = GetRandomPos(1.25f, previousSpawns);
            previousSpawns.Add(randomPos);
            
            Instantiate(sheep, randomPos, Quaternion.identity);
        }
    }

    //first sheep spawn
    public Vector2 GetRandomPos(float minDistance, List<Vector2> previousSpawns)
    {
        float xPos = Random.Range(leftLimit.position.x, rightLimit.position.x);
        float yPos = Random.Range(bottomLimit.position.y, topLimit.position.y);
        
        Vector2 vec = new Vector2(xPos, yPos);

        while (!ValidSpawn(vec, minDistance, previousSpawns))
        {
            xPos = Random.Range(leftLimit.position.x, rightLimit.position.x);
            yPos = Random.Range(topLimit.position.y, bottomLimit.position.y);

            vec = new Vector2(xPos, yPos);
        }

        return vec;
    }

    bool ValidSpawn(Vector2 vec, float minDistance, List<Vector2> previousPoints)
    {
        for (int i = 0; i < previousPoints.Count; i++)
        {
            if (Vector2.Distance(vec, previousPoints[i]) < minDistance)
            {
                return false;
            }
        }

        return true;
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(levelDuration / 60);
        int seconds = Mathf.FloorToInt(levelDuration % 60);
        
        timerText.text = "Time remaining: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    //everything else
    public Vector2 GetRandomPos(float minDistance, Transform transform)
    {
        float xPos = Random.Range(leftLimit.position.x, rightLimit.position.x);
        float yPos = Random.Range(bottomLimit.position.y, topLimit.position.y);

        Vector2 vec = new Vector2(xPos, yPos);

        while (Vector2.Distance(vec, transform.position) < minDistance)
        {
            xPos = Random.Range(leftLimit.position.x, rightLimit.position.x);
            yPos = Random.Range(topLimit.position.y, bottomLimit.position.y);

            vec = new Vector2(xPos, yPos);
        }

        return vec;
    }

    //random spawn point in fence limits
    public Vector2 GetRandomPos(float minDistance, Vector2 bottomRightCorner, Vector2 topLeftCorner, List<Vector2> previousPos)
    {
        float xPos = Random.Range(topLeftCorner.x, bottomRightCorner.x);
        float yPos = Random.Range(bottomRightCorner.y, topLeftCorner.y);

        Vector2 vec = new Vector2(xPos, yPos);

        while (!ValidSpawn(vec, minDistance, previousPos))
        {
            xPos = Random.Range(topLeftCorner.x, bottomRightCorner.x);
            yPos = Random.Range(bottomRightCorner.y, topLeftCorner.y);

            vec = new Vector2(xPos, yPos);
        }

        return vec;
    }
}
