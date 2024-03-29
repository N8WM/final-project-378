using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public AudioSource[] backgroundMusic;
    public float respawnHeight = -10f;
    public bool isPaused { get; private set; } = false;
    public DoorTarget spawnAtDoor { get; private set; } = null;
    public bool GameIsWon { get; private set; } = false;
    GameObject[] killWhenFall;
    GameObject[] respawnWhenFall;
    Vector3[] respawnPoints;
    Animator sceneTransitionAnim;
    bool deathState = false;
    int musicPlaying = -1;
    float originalVolume;
    public bool crtEffectEnabled { get; private set; } = true;
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
        sceneTransitionAnim = GetComponentInChildren<Animator>();
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Menu" || sceneName == "Credits") {
            backgroundMusic[0].Play();
            musicPlaying = 0;
        }
        else {
            backgroundMusic[1].Play();
            musicPlaying = 1;
        }
        originalVolume = backgroundMusic[0].volume;
        SetCRTEffect(crtEffectEnabled);
    }

    IEnumerator FadeTrack()
    {
        float timeToFade = 1f;
        float timeElaped = 0f;
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Menu" || sceneName == "Credits") {
            if (musicPlaying == 1) {
                musicPlaying = 0;
                backgroundMusic[0].Play();
                while (timeElaped < timeToFade) {

                    backgroundMusic[1].volume = Mathf.Lerp(originalVolume, 0, timeElaped / timeToFade);
                    backgroundMusic[0].volume = Mathf.Lerp(0, originalVolume, timeElaped / timeToFade);
                    timeElaped += Time.deltaTime;
                    yield return null;
                }
                backgroundMusic[1].Stop();
                backgroundMusic[1].volume = originalVolume;
                backgroundMusic[0].volume = originalVolume;
            }
        } else {
            if (musicPlaying == 0) {
                musicPlaying = 1;
                backgroundMusic[1].Play();
                while (timeElaped < timeToFade) {

                    backgroundMusic[0].volume = Mathf.Lerp(originalVolume, 0, timeElaped / timeToFade);
                    backgroundMusic[1].volume = Mathf.Lerp(0, originalVolume, timeElaped / timeToFade);
                    timeElaped += Time.deltaTime;
                    yield return null;
                }
                backgroundMusic[0].Stop();
                backgroundMusic[0].volume = originalVolume;
                backgroundMusic[1].volume = originalVolume;
            }
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
                Rigidbody2D rb = respawnWhenFall[i].GetComponent<Rigidbody2D>();
                respawnWhenFall[i].transform.position = respawnPoints[i];
                respawnWhenFall[i].transform.rotation = Quaternion.identity;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Reset();
        Time.timeScale = 1;
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (musicPlaying == -1) return;
        StartCoroutine(FadeTrack());

        if (GameIsWon) return;
        int levelCount = 0;
        int wonCount = 0;
        foreach (DoorTarget door in doors) {
            if (door.startLocked) {
                levelCount++;
                if (door.levelCompleted)
                    wonCount++;
            }
        }

        if (levelCount == wonCount) {
            GameWon();
        }
    }

    public void SetCRTEffect(bool enabled)
    {
        crtEffectEnabled = enabled;
        Volume[] volumes = FindObjectsOfType<Volume>();
        foreach (Volume volume in volumes)
            if (volume.profile.TryGet(out LensDistortion ld))
                ld.active = crtEffectEnabled;
    }

    public void OnDeath()
    {
        deathState = true;
        PlayerController._instance.OnDeath();
        Time.timeScale = 0.1f;
        Retry();
    }

    public void FadeToLevel(string levelName, bool setSpawnPos = false)
    {
        if (setSpawnPos)
            spawnAtDoor = currentLevelDoor;
        else
            spawnAtDoor = null;
        sceneTransitionAnim.SetBool("fadeOut", true);
        sceneTransitionAnim.speed = 1f;
        StartCoroutine(LoadLevelFadeIn(levelName));
    }

    IEnumerator LoadLevelFadeIn(string levelName)
    {
        yield return new WaitForSecondsRealtime(1f);
        LoadLevel(levelName);
        sceneTransitionAnim.speed = 1f;
        sceneTransitionAnim.SetBool("fadeOut", false);
    }

    void LoadLevel(string levelName)
    {
        Time.timeScale = 0;
        SceneManager.LoadScene(levelName);
    }

    public void Retry()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        FadeToLevel(currentScene.name);
    }

    public void Menu()
    {
        Time.timeScale = 0;
        FadeToLevel("Scenes/Menu", true);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
        PausePanelController._instance.Show();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        isPaused = false;
        PausePanelController._instance.Hide();
    }

    public void ResetLevels()
    {
        GameIsWon = false;
        foreach (DoorTarget door in doors) {
            door.locked = door.startLocked;
            door.levelCompleted = false;
        }
    }

    void GameWon()
    {
        // TODO: Show game won screen
        GameIsWon = true;
        print("Game won!");
    }
}
