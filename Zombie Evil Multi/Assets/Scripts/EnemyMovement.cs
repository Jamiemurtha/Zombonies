using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    ConnectionManager connectionManager;

    Transform closestPlayer;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        connectionManager = ConnectionManager.instance.GetComponent<ConnectionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (closestPlayer == null)
        {
            return;
        }
        Vector2 direction = closestPlayer.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 85;
        direction.Normalize();
        movement = direction;
    }

    private void FixedUpdate()
    {
        moveCharacter(movement);

        float closestDifference = Mathf.Infinity;

        for (int i = 0; i < connectionManager.playerList.Count; i++)
        {
            float currentDifference = 0;
            currentDifference += Mathf.Abs(transform.position.x - connectionManager.playerList[i].transform.position.x);
            currentDifference += Mathf.Abs(transform.position.y - connectionManager.playerList[i].transform.position.y);
            if (currentDifference < closestDifference)
            {
                closestPlayer = connectionManager.playerList[i].transform;
                closestDifference = currentDifference;
            }
        }
    }

    void moveCharacter(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
    }


}
