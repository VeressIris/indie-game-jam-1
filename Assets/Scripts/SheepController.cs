using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    public float minSheepDistance = 3.25f;

    [Header("Movement:")]
    private Vector2 randomPos;
    private float speed;
    public Vector2 destination;
    [HideInInspector] public bool isUnderPlayerInteraction = false;
    private bool canMove = true;
    [HideInInspector] public bool isInFence = false;
    [HideInInspector] public bool arrivedAtFinalPos = false;
    [HideInInspector] public bool movingTowardsFinalPos = false;

    private bool[] wolfChaseChance = { true, false, false, true, false };
    private Transform wolf;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip[] clips;
    private bool canPlaySound = false;

    void Start()
    {
        //randomly decide if the sheep should follow the wolf
        if (wolfChaseChance[Random.Range(0, 5)])
        {
            wolf = GameObject.FindWithTag("Wolf").transform;
            destination = wolf.position;
            Debug.Log("following wolf", gameObject);
        }
        else
        {
            randomPos = GameManager.Instance.GetRandomPos(minSheepDistance, transform);
            destination = randomPos;
        }

        speed = Random.Range(0.85f, 1.7f);

        StartCoroutine(InitSoundDelay());
    }

    void Update()
    {
        //sound
        if (!audioSrc.isPlaying && canPlaySound)
        {
            StartCoroutine(PlaySound());
        }

        //movement
        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            RotateToFaceDestination();
        }
        //arrived at destination
        if (Vector2.Distance(transform.position, destination) <= 0.2f && canMove)
        {
            if (!isUnderPlayerInteraction) StartCoroutine(GetNewRandomPos());
            else if (isInFence)
            {
                canMove = false;
                arrivedAtFinalPos = true;
            }
            else if (isUnderPlayerInteraction)
            {
                StopCoroutine(GetNewRandomPos());
                canMove = true;
            }
        }
    }

    IEnumerator GetNewRandomPos()
    {
        yield return new WaitForSeconds(Random.Range(0.8f, 2.25f));

        randomPos = GameManager.Instance.GetRandomPos(minSheepDistance, transform);
        destination = randomPos;
        canMove = true;
    }

    void RotateToFaceDestination()
    {
        Vector2 dir = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        transform.up = dir;
    }

    IEnumerator PlaySound()
    {
        audioSrc.clip = clips[Random.Range(0, 2)];
        audioSrc.volume = Random.Range(0.15f, 0.42f);
        audioSrc.Play();
        canPlaySound = false;

        yield return new WaitForSeconds(Random.Range(4f, 6f));

        canPlaySound = true;
    }

    IEnumerator InitSoundDelay()
    {
        yield return new WaitForSeconds(Random.Range(0.85f, 1.5f));

        audioSrc.clip = clips[Random.Range(0, 2)];
        audioSrc.volume = Random.Range(0.15f, 0.42f);
        audioSrc.Play();

        canPlaySound = true;
    }
}