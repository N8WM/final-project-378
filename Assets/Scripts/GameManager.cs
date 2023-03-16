using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public AudioSource[] backgroundMusic;
    public float respawnHeight = -10f;
    GameObject[] killWhenFall;
    GameObject[] respawnWhenFall;
    Vector3[] respawnPoints;
    bool deathState = false;
    int musicPlaying = -1;
    public DoorTarget[] winUnlocks { get {
        if (currentLevelDoor == null) return new DoorTarget[] {};
        return currentLevelDoor.destinations;
    }}
    public DoorTarget currentLevelDoor;
    public DoorTarget[] doors = {};

    void Awake() {
        if (GameObject.FindObjectsOfType<GameManager>().Length > 1 && GameManager._instance != this) {
            GameManager._instance.currentLevelDoor = currentLevelDoor;
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        backgroundMusic = GetComponents<AudioSource>();
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Menu" || sceneName == "Credits") {
            backgroundMusic[0].Play();
            musicPlaying = 0;
        }
        else {
            backgroundMusic[1].Play();
            musicPlaying = 1;
        }
    }

    void Reset()
    {
        killWhenFall = GameObject.FindGameObjectsWithTag("KillWhenFall");
        respawnWhenFall = GameObject.FindGameObjectsWithTag("RespawnWhenFall");
        respawnPoints = new Vector3[respawnWhenFall.Length];
        for (int i = 0; i < respawnWhenFall.Length; i++)
            respawnPoints[i] = respawnWhenFall[i].transform.position;
        deathState = false;
    }

    void FixedUpdate()
    {
        if (deathState) return;
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
        Time.timeScale = 1;
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (musicPlaying == -1) return;
        if (sceneName == "Menu" || sceneName == "Credits") {
            if (musicPlaying == 1) {
                backgroundMusic[1].Stop();
                backgroundMusic[0].Play();
                musicPlaying = 0;
            }
        } else {
            if (musicPlaying == 0) {
                backgroundMusic[0].Stop();
                backgroundMusic[1].Play();
                musicPlaying = 1;
            }
        }
    }

    public void OnDeath()
    {
        deathState = true;
        PlayerController._instance.OnDeath();
        DeathPanelController._instance.Show();
        Time.timeScale = 0.1f;
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 0;
        SceneManager.LoadScene(levelName);
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

    public void ResetLevels()
    {
        foreach (DoorTarget door in doors) {
            door.locked = door.startLocked;
            door.levelCompleted = false;
        }
    }
}
