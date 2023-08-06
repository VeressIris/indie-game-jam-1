using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep"))
        {
            GameObject sheep = collision.gameObject;
            if (IsOutOfBounds(sheep))
            {
                Debug.Log("sheep out of bounds", sheep);

                gameManager.lostSheep = true;
                gameManager.sheepNumber--;
                SheepController sheepController = sheep.GetComponent<SheepController>();
                sheepController.enabled = false;
            }
        }
    }

    bool IsOutOfBounds(GameObject obj)
    {
        Vector2 pos = obj.transform.position;
        //left
        if (transform.position.x < 0 && pos.x < transform.position.x) return true;
        //right
        if (transform.position.x > 0 && pos.x > transform.position.x) return true;
        //top
        if (transform.position.y > 0 && pos.y > transform.position.y) return true;
        //bottom
        if (transform.position.y < 0 && pos.y < transform.position.y) return true;
        
        return false;
    }
}
