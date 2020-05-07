using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon" , menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public Sprite currentWeaponSpr;

    public GameObject bulletPrefab;
    public float fireRate = 1;
    public int damage = 20;

    public AudioClip[] shootClips;

    public void Shoot(GameObject player)
    {
        var bullet = Instantiate(bulletPrefab, player.transform.GetChild(0).GetChild(0).transform.position, Quaternion.identity);

        RifleBullet b = bullet.GetComponent<RifleBullet>();
        b.dir = player.transform.GetChild(0).GetChild(0).GetChild(0).transform.position;
        b.name = player.name + "'s_Bullet";
        if (b.transform.childCount > 0)
        {
            b.transform.GetChild(0).name = b.name;
            b.transform.GetChild(1).name = b.name;
            b.transform.GetChild(2).name = b.name;
        }
        SoundManager.instance.PlaySoundFX(shootClips[Random.Range(0, shootClips.Length)]);
    }
}
