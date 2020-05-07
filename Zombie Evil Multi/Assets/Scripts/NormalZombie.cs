using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : MonoBehaviour
{
    [SerializeField]
    public int zombie = 3;
    public AudioClip[] shootClips;
    public AudioClip[] dead;

    public EnemySpawner enemySpawner;

    string lastPlayerHit = "";
    int originalHealth;

    void Awake()
    {
        enemySpawner = GameObject.Find("Connection Manager").GetComponent<EnemySpawner>();
        originalHealth = zombie;
    }

    void Update()
    {
        if (zombie <= 0)
        {
            SoundManager.instance.PlaySoundFX(dead[Random.Range(0, shootClips.Length)]);
            ScoreScript.AddScore(lastPlayerHit, originalHealth);
            Destroy(gameObject);
        }
        print(lastPlayerHit);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            zombie -= 3;
            SoundManager.instance.PlaySoundFX(shootClips[Random.Range(0, shootClips.Length)]);
            lastPlayerHit = other.gameObject.name;
            lastPlayerHit = lastPlayerHit.Replace("'s_Bullet", "");
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "SMGBullet")
        {
            zombie -= 10;
            SoundManager.instance.PlaySoundFX(shootClips[Random.Range(0, shootClips.Length)]);
            lastPlayerHit = other.gameObject.name;
            lastPlayerHit = lastPlayerHit.Replace("'s_Bullet", "");
            Destroy(other.gameObject);
        }
    }
}
