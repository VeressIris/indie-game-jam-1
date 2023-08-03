using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
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
            Vector2 randomPos = GetRandomPos();
            previousSpawns.Add(randomPos);
            Instantiate(sheep, randomPos, Quaternion.identity);
        }
    }

    Vector2 GetRandomPos()
    {
        float xPos = Random.Range(leftLimit.position.x, rightLimit.position.x);
        float yPos = Random.Range(bottomLimit.position.y, topLimit.position.y);
        
        Vector2 vec = new Vector2(xPos, yPos);

        while (!ValidSpawn(vec))
        {
            xPos = Random.Range(leftLimit.position.x, rightLimit.position.x);
            yPos = Random.Range(topLimit.position.y, bottomLimit.position.y);

            vec = new Vector2(xPos, yPos);
        }

        return vec;
    }

    bool ValidSpawn(Vector2 vec)
    {
        for (int i = 0; i < previousSpawns.Count; i++)
        {
            if (Vector2.Distance(vec, previousSpawns[i]) < 1.25f)
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
}
