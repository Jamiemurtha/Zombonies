using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointShoot : MonoBehaviour
{
    public GameObject crosshairs;
    private Vector3 target;

    // Update is called once per frame
    void Update()
    {
        if (crosshairs == null)
        {
            return;
        }
        if (Cursor.visible)
        {
            Cursor.visible = false;
            crosshairs.transform.parent = null;
        }
        target = transform.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        crosshairs.transform.position = new Vector2(target.x, target.y);
    }
}
