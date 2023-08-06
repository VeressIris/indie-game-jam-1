using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level")]
    public float levelDuration = 10f;
    [SerializeField] private GameObject sheepPrefab;
    public int sheepNumber = 5;
    private List<Vector2> previousSpawns = new List<Vector2>();
    [HideInInspector] public int sheepInFence;
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public bool timeOut = false;
    private SheepController[] sheepControllers;
    [SerializeField] private WolfController wolfController;
    private bool controllersDisabled = false;
    [SerializeField] private PlayerController playerController;
    [HideInInspector] public bool lostSheep = false;

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
    [HideInInspector] public bool paused = false;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private GameObject optionsMenu;

    [Header("SFX")]
    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip loseSFX;
    [SerializeField] private AudioClip winSFX;
    [SerializeField] private AudioClip halfwaySFX;
    private float halfwayMark;
    private bool winSFXPlayed = false;
    private bool loseSFXPlayed = false;

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

        halfwayMark = levelDuration / 2;
        StartCoroutine(PlayHalfwayMarkSFX());
    }

    void Update()
    {
        if (sheepInFence == sheepNumber) Win();
        else if (gameOver && !timeOut)
        {
            playerController.canMove = false;

            if (wolfController.arrived)
            {
                timer.SetActive(false);
                loseScreen.SetActive(true);
                PlayLoseSFX();
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
                PlayLoseSFX();
                playerController.canMove = false;

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
        if (!gameOver || !timeOut)
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
    }

    void SetUI()
    {
        timer.SetActive(true);
        pauseMenu.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        optionsMenu.SetActive(false);
    }

    void Win()
    {
        //play sfx
        audioSrc.clip = winSFX;
        if (!audioSrc.isPlaying && !winSFXPlayed)
        {
            audioSrc.Play();
            winSFXPlayed = true;
        }

        //visuals
        timer.SetActive(false);
        winScreen.SetActive(true);

        if (lostSheep) winText.text = "You Win!\nYou lost some sheep along the way though.";

        playerController.canMove = false;
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

    IEnumerator PlayHalfwayMarkSFX()
    {
        yield return new WaitForSeconds(halfwayMark);
        audioSrc.volume = 0.62f;
        audioSrc.clip = halfwaySFX;
        audioSrc.Play();
    }

    void PlayLoseSFX()
    {
        audioSrc.volume = 1f;
        audioSrc.clip = loseSFX;
        if (!audioSrc.isPlaying && !loseSFXPlayed) 
        { 
            audioSrc.Play();
            loseSFXPlayed = true;
        }
    }

    public void OpenOptionsMenu()
    {
        if (optionsMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
        else
        {    
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
    }
}
