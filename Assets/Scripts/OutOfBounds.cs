using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep"))
        {
            GameObject sheep = collision.gameObject;
            if (IsOutOfBounds(sheep))
            {
                Debug.Log("sheep out of bounds");
            
                GameManager.Instance.lostSheep = true;
                sheep.GetComponent<SheepController>().enabled = false;
                Destroy(sheep, 1f);
            }
        }
    }

    bool IsOutOfBounds(GameObject obj)
    {
        Vector2 pos = obj.transform.position;
        //right
        if (transform.position.x < 0 && pos.x < transform.position.x) return true;
        //left
        if (transform.position.x > 0 && pos.x > transform.position.x) return true;
        //top
        if (transform.position.y > 0 && pos.y > transform.position.y) return true;
        //bottom
        if (transform.position.y < 0 && pos.y < transform.position.y) return true;
        
        return false;
    }
}
