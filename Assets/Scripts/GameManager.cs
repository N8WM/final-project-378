using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public float respawnHeight = -10f;
    GameObject[] killWhenFall;
    GameObject[] respawnWhenFall;
    Vector3[] respawnPoints;

    void Awake() {
        if (FindObjectsOfType(GetType()).Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    
        _instance = this;
    }

    void Start()
    {
        killWhenFall = GameObject.FindGameObjectsWithTag("KillWhenFall");
        respawnWhenFall = GameObject.FindGameObjectsWithTag("RespawnWhenFall");
        respawnPoints = new Vector3[respawnWhenFall.Length];
        for (int i = 0; i < respawnWhenFall.Length; i++)
            respawnPoints[i] = respawnWhenFall[i].transform.position;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < killWhenFall.Length; i++)
            if (killWhenFall[i].transform.position.y < respawnHeight)
                OnDeath();

        for (int i = 0; i < respawnWhenFall.Length; i++) {
            if (respawnWhenFall[i].transform.position.y < respawnHeight) {
                respawnWhenFall[i].transform.position = respawnPoints[i];
                respawnWhenFall[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

    void OnDeath()
    {
        return;
    }
}
