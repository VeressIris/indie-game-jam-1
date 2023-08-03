using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    bool ValidSpawn(Vector2 vec, float minDistance, List<Vector2> previousSpawns)
    {
        for (int i = 0; i < previousSpawns.Count; i++)
        {
            if (Vector2.Distance(vec, previousSpawns[i]) < minDistance)
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
}
