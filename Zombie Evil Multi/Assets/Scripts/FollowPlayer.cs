using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowPlayer : MonoBehaviour
{
    Transform target;

    string targetName;

    void Awake()
    {
        target = gameObject.transform.parent;
    }

    void Update()
    {
        if (targetName != null && GameObject.Find(targetName) == null)
        {
            Destroy(gameObject);
        }

        if (target.name != "Player(Clone)" && targetName == null)
        {
            targetName = target.name;
            transform.parent = null;
        }
        else if (targetName != target.name)
        {
            return;
        }
        transform.position = target.position;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
