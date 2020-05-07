using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodScript : MonoBehaviour
{
    public int wood = 30;
    public AudioClip woodsfx;
    void Update()
    {
        if (wood <= 0)
        {
            SoundManager.instance.PlaySoundFX(woodsfx);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            SoundManager.instance.PlaySoundFX(woodsfx);
            wood -= 3;
        }

        if (other.gameObject.tag == "SMGBullet")
        {
            SoundManager.instance.PlaySoundFX(woodsfx);
            wood -= 10;
        }
    }
}
