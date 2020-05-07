using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerafollow : MonoBehaviour
{
    private Transform playerPos;

    public string currentPlayer = "";

    // Update is called once per frame
    void Update()
    {
        
        if (playerPos == null)
        {
            if (GameObject.Find(currentPlayer) == null)
            {
                return;
            }
            playerPos = GameObject.Find(currentPlayer).transform;
            return;
        }
        transform.position = new Vector3(playerPos.position.x, playerPos.position.y, transform.position.z);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10f, 10f), Mathf.Clamp(transform.position.y, -15f, 15f), transform.position.z);
    }
}
