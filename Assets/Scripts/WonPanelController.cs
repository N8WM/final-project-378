using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonPanelController : MonoBehaviour
{
    public static WonPanelController _instance;
    public GameObject wonPanel;

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
        wonPanel.SetActive(true);
    }

    public void Hide()
    {
        wonPanel.SetActive(false);
    }

    public void Next()
    {
        GameManager._instance.NextLevel();
        Hide();
    }

    public void Menu()
    {
        GameManager._instance.Menu();
        Hide();
    }
}
