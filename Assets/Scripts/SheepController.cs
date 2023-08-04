using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    public float minSheepDistance = 3.25f;

    private Vector2 randomPos;
    private float speed;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public Vector2 destination;
    [HideInInspector] public bool movingTowardsFence = false;
    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip[] clips;
    private bool canPlaySound = false;
    [HideInInspector] public bool playerInteracting = false;
    [HideInInspector] public bool arrived = false;
    [HideInInspector] public bool done = false; //turns true when the sheep is on its merry way to pick a spot in the pen

    void Start()
    {
        randomPos = GameManager.Instance.GetRandomPos(minSheepDistance, transform);
        destination = randomPos;
        
        speed = Random.Range(0.8f, 1.628f);

        StartCoroutine(InitSoundDelay());
    }

    void Update()
    {
        if (canMove && !arrived)
        { 
            RotateToFaceDestination();
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }

        if (!audioSrc.isPlaying && canPlaySound)
        {
            StartCoroutine(PlaySound());
        }

        if (Vector2.Distance(transform.position, destination) <= 0.2f && canMove)
        {
            canMove = false;

            if (!movingTowardsFence && !done)
            {
                randomPos = GameManager.Instance.GetRandomPos(minSheepDistance, transform);
                destination = randomPos;
                canMove = true;
                //StartCoroutine(GetNewRandomPos());
            }
            else if (done && !arrived)
            { 
                GameManager.Instance.sheepInFence++;
                arrived = true;
                Debug.Log(GameManager.Instance.sheepInFence);
            } 
        }
    }

    IEnumerator GetNewRandomPos()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2.5f));

        if (!movingTowardsFence && !done)
        {
            randomPos = GameManager.Instance.GetRandomPos(minSheepDistance, transform);
            destination = randomPos;
            canMove = true;
        }
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
