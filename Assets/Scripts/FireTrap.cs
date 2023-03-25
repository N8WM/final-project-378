using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Fire");
        if (col.name == "Cat") {
            GameManager._instance.OnDeath();
        }
    }
}
