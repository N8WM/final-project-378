using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanelController : MonoBehaviour
{
    public static PausePanelController _instance;
    public GameObject pausePanel;

    void Awake()
    {
        if (_instance != this && _instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        Show();
        Hide();
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
}
