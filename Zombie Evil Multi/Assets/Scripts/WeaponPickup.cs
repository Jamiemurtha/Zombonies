using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Weapon weapon;
    public AudioClip pick;

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Player")
        {
            target.GetComponent<Player>().currentWeapon = weapon;
            target.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = weapon.currentWeaponSpr;
            SoundManager.instance.PlaySoundFX(pick);
            Destroy(gameObject);
        }
    }
}
