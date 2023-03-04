using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPanelController : MonoBehaviour
{
    public static DeathPanelController _instance;
    public GameObject deathPanel;

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
        deathPanel.SetActive(true);
    }

    public void Hide()
    {
        deathPanel.SetActive(false);
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
