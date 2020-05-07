using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement2 : MonoBehaviour
{

    List<Transform> player = new List<Transform>();
    Transform closestPlayer;

    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < player.Count; i++)
        {
            closestPlayer = player[i];
            float difference1 = Mathf.Abs(transform.position.x - player[i].position.x) + Mathf.Abs(transform.position.y - player[i].position.y);
            float difference2 = Mathf.Abs(transform.position.x - closestPlayer.position.x) + Mathf.Abs(transform.position.y - closestPlayer.position.y);

            if (difference1 < difference2)
            {
                closestPlayer = player[i];
            }
        }

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    void Awake()
    {
        player.Add(GameObject.Find("Player").GetComponent<Transform>());
    }

}
