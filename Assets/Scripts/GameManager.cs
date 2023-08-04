using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level")]
    [SerializeField] private float levelDuration = 10f;
    [SerializeField] private GameObject sheep;
    [SerializeField] private int sheepNumber = 5;
    private List<Vector2> previousSpawns = new List<Vector2>();
    private bool winStatus = false;
    [HideInInspector] public int sheepInFence;

    [Header("Screen Limits")]
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;
    [SerializeField] private Transform bottomLimit;
    [SerializeField] private Transform topLimit;
    [SerializeField] private FenceAreaController fenceController;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject pauseMenu;
    private bool paused = false;
    [SerializeField] private GameObject winScreen;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnSheep();
        
        paused = false;
        SetUI();
    }

    void Update()
    {
        if (sheepInFence == sheepNumber) Win();
        else
        {
            //timer
            if (levelDuration > 0)
            {
                levelDuration -= Time.deltaTime;
                UpdateTimerUI();
            } 
            else
            {
                timerText.text = "Time is out! The Wolf is coming!";
                //do something
            }
            
            if (Input.GetKeyDown(KeyCode.Escape)) PauseResume();
        }
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

    public void PauseResume()
    {
        if (!paused)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            timer.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            timer.SetActive(true);
        }

        paused = !paused;
    }

    void SetUI()
    {
        timer.SetActive(true);
        pauseMenu.SetActive(false);
        winScreen.SetActive(false);
    }

    void Win()
    {
        timer.SetActive(false);
        winScreen.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
