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
    [SerializeField] private GameObject sheepPrefab;
    [SerializeField] private int sheepNumber = 5;
    private List<Vector2> previousSpawns = new List<Vector2>();
    [HideInInspector] public int sheepInFence;
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public bool timeOut = false;
    private SheepController[] sheepControllers;
    [SerializeField] private WolfController wolfController;
    private bool controllersDisabled = false;

    [Header("Limits")]
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;
    [SerializeField] private Transform bottomLimit;
    [SerializeField] private Transform topLimit;
    [SerializeField] private FenceAreaController fenceController;
    [SerializeField] private Transform wolfTransform;
    [SerializeField] private Transform fenceTransform;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject pauseMenu;
    private bool paused = false;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnSheep();
        
        paused = false;
        SetUI();

        sheepControllers = new SheepController[sheepNumber];
        GetSheepControllers();
    }

    void Update()
    {
        if (sheepInFence == sheepNumber) Win();
        else if (gameOver && !timeOut)
        {
            if (wolfController.arrived)
            {
                timer.SetActive(false);
                loseScreen.SetActive(true);
            }

            DisableSheepControllers();
        }
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
                timeOut = true;
                timerText.text = "Time is out! The Wolf is coming!";

                if (wolfController.arrived)
                {
                    timer.SetActive(false);
                    loseScreen.SetActive(true);
                }

                DisableSheepControllers();
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
            
            Instantiate(sheepPrefab, randomPos, Quaternion.identity);
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

    //sheep can't spawn if they're too close together or if they're too close to the wolf
    bool ValidSpawn(Vector2 vec, float minDistance, List<Vector2> previousPoints)
    {
        for (int i = 0; i < previousPoints.Count; i++)
        {
            if (Vector2.Distance(vec, previousPoints[i]) < minDistance ||
                Vector2.Distance(wolfTransform.position, previousPoints[i]) <= 2f)
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

    //general GetRandomPos function
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
        loseScreen.SetActive(false);
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

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex < SceneManager.sceneCount) SceneManager.LoadScene(currentSceneIndex + 1);
        else QuitToMainMenu();
    }

    void GetSheepControllers()
    {
        GameObject[] sheep = GameObject.FindGameObjectsWithTag("Sheep");
        for (int i = 0; i < sheepNumber; i++)
        {
            sheepControllers[i] = sheep[i].GetComponent<SheepController>();
        }
    }

    void DisableSheepControllers()
    {
        if (!controllersDisabled)
        {
            for (int i = 0; i < sheepNumber; i++) sheepControllers[i].enabled = false;
            controllersDisabled = true;
        }
    }
}
