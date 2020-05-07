using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Weapon currentWeapon;
    public GameObject bullet;
    private Rigidbody2D myBody;
    public float speed;
    private float nextTimeOfFire = 0;
    private Vector2 moveVelocity;

    Vector2 oldPosition;
    Vector2 currentPosition;
    Quaternion oldRotation;
    Quaternion currentRotation;
    public bool canControl = false;

    Text gameOverText;

    bool isDead = false;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = currentWeapon.currentWeaponSpr;
        gameOverText = GameObject.Find("Game Over Canvas").GetComponentInChildren<Text>();
    }

    void Start()
    {
        oldPosition = transform.position;
        currentPosition = oldPosition;
        oldRotation = transform.rotation;
        currentRotation = oldRotation;
    }

    private void Update() {
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isDead = false;
                gameOverText.text = "";
                ScoreScript.ResetScore(gameObject.name);
                gameObject.transform.position = new Vector2(-15.8f, -17.7f);
            }
        }

        if (!canControl)
        {
            return;
        }

        Rotation();

        if (currentPosition != oldPosition)
        {
            ConnectionManager.instance.GetComponent<ConnectionManager>().Move(transform.position);
            oldPosition = currentPosition;
        }

        if (currentRotation != oldRotation)
        {
            ConnectionManager.instance.GetComponent<ConnectionManager>().Rotate(transform.rotation);
            oldRotation = currentRotation;
        }

        if (Input.GetMouseButton(0))
        {
            ConnectionManager.instance.GetComponent<ConnectionManager>().Shoot();
        }
    }

    void FixedUpdate()
    {
        if (!canControl)
        {
            return;
        }

        Movement();
    }

    public void ShootWeapon(GameObject ply)
    {
        if (Time.time >= nextTimeOfFire)
        {
            currentWeapon.Shoot(ply);
            nextTimeOfFire = Time.time + 1 / currentWeapon.fireRate;
        }
    }

    void Rotation()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10 * Time.deltaTime);
        currentRotation = transform.rotation;
    }

    void Movement()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * speed;
        myBody.MovePosition(myBody.position + moveVelocity * Time.fixedDeltaTime);
        currentPosition = transform.position;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" && canControl)
        {
            gameObject.transform.position = new Vector2(1000, 1000);
            gameOverText.text = "Game Over\nTotal Score: " + ScoreScript.currentScore + "\nPress [R] to restart";
            isDead = true;
        }
    }
}
