using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// need collider in gameobject
public class Kbd_KeyDrag : MonoBehaviour
{
    float distance = 5;
    void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        this.transform.position = objPosition;
    }
}
