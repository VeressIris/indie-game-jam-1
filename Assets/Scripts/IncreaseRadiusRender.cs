using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseRadiusRender : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private const float baseRadius = 1.628f;

    void Start()
    {
        transform.localScale = transform.localScale * playerController.detectionRadius / baseRadius;
        transform.localScale += new Vector3(0.25f, 0.25f, 0.25f); //a little extra 
    }
}
