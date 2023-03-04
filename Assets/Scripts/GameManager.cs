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
    bool wonState = false;
    bool deathState = false;

    void Awake() {
        if (GameObject.FindObjectsOfType<GameManager>().Length > 1 && GameManager._instance != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    
        _instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Reset()
    {
        killWhenFall = GameObject.FindGameObjectsWithTag("KillWhenFall");
        respawnWhenFall = GameObject.FindGameObjectsWithTag("RespawnWhenFall");
        respawnPoints = new Vector3[respawnWhenFall.Length];
        for (int i = 0; i < respawnWhenFall.Length; i++)
            respawnPoints[i] = respawnWhenFall[i].transform.position;
        wonState = false;
        deathState = false;
    }

    void FixedUpdate()
    {
        if (wonState || deathState) return;
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Reset();
    }

    public void OnDeath()
    {
        if (wonState) return;
        deathState = true;
        PlayerController._instance.OnDeath();
        DeathPanelController._instance.Show();
        Time.timeScale = 0.1f;
    }

    public void WonLevel()
    {
        // PlayerController._instance.WonLevel();
        if (deathState) return;
        wonState = true;
        WonPanelController._instance.Show();
        Time.timeScale = 0.1f;
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        Scene currentScene = SceneManager.GetActiveScene();
         // this will be replaced with a level selection screen
        if (currentScene.buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(currentScene.buildIndex + 1);
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
        SceneManager.LoadScene("Scenes/Menu"); // replace with menu scene name
    }
}
