using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelController : MonoBehaviour
{
    public static PausePanelController _instance;
    public GameObject pausePanel;
    Toggle crtEffectToggle;

    void Awake()
    {
        if (_instance != this && _instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        crtEffectToggle = GetComponentInChildren<Toggle>(true);
        Show();
        crtEffectToggle.isOn = GameManager._instance.crtEffectEnabled;
        Hide();
    }

    public void Resume()
    {
        GameManager._instance.Resume();
    }

    public void Show()
    {
        pausePanel.SetActive(true);
    }

    public void Hide()
    {
        pausePanel.SetActive(false);
    }

    public void Retry()
    {
        GameManager._instance.Retry();
        Hide();
    }

    public void Menu()
    {
        GameManager._instance.Menu();
        Hide();
    }

    public void UpdateCRTEffect()
    {
        if (crtEffectToggle == null)
            crtEffectToggle = GetComponentInChildren<Toggle>(true);
        GameManager._instance.SetCRTEffect(crtEffectToggle.isOn);
    }
}
