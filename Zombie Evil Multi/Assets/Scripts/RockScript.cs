using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    public int rock = 30;
    public AudioClip rocksfx;
    void Update()
    {
        if (rock <= 0)
        {
            SoundManager.instance.PlaySoundFX(rocksfx);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            SoundManager.instance.PlaySoundFX(rocksfx);
            rock -= 3;
        }

        if (other.gameObject.tag == "SMGBullet")
        {
            SoundManager.instance.PlaySoundFX(rocksfx);
            rock -= 10;
        }
    }
}
