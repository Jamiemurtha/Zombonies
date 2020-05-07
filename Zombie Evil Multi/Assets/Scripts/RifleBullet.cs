using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleBullet : MonoBehaviour
{
    private float speed = 15;
    public GameObject bul;
    public Vector2 dir;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1);
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, dir, speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Rock")
        {
            Destroy(bul);
        }

        if (other.gameObject.tag == "Wood")
        {
            Destroy(bul);
        }
    }
}
