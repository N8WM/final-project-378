using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Reset();
    }

    void Reset()
    {
        killWhenFall = GameObject.FindGameObjectsWithTag("KillWhenFall");
        respawnWhenFall = GameObject.FindGameObjectsWithTag("RespawnWhenFall");
        respawnPoints = new Vector3[respawnWhenFall.Length];
        for (int i = 0; i < respawnWhenFall.Length; i++)
            respawnPoints[i] = respawnWhenFall[i].transform.position;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < killWhenFall.Length; i++) {
            if (killWhenFall[i] != null)
                if (killWhenFall[i].transform.position.y < respawnHeight)
                    OnDeath();
            else {
                Reset();
                i = 0;
            }
        }

        for (int i = 0; i < respawnWhenFall.Length; i++) {
            if (respawnWhenFall[i] != null) {
                if (respawnWhenFall[i].transform.position.y < respawnHeight) {
                    respawnWhenFall[i].transform.position = respawnPoints[i];
                    respawnWhenFall[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }
            else {
                Reset();
                i = 0;
            }
        }
    }

    void OnDeath()
    {
        PlayerController._instance.OnDeath();
        DeathPanelController._instance.Show();
        Time.timeScale = 0.1f;
    }

    public void Retry()
    {
        Time.timeScale = 1;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu"); // replace with menu scene name
    }
}
