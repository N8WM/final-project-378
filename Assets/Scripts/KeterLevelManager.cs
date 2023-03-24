using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeterLevelManager : MonoBehaviour
{
    public GameObject player;
    public GameObject crown;
    private SpriteRenderer sr;
    private ItemController ic;
    private float crownInitY;
    private Vector2 lastPosition;

    void Start()
    {
        sr = player.GetComponent<SpriteRenderer>();
        ic = crown.GetComponent<ItemController>();
        crownInitY = crown.transform.position.y;
        lastPosition = crown.transform.position;
    }

    void Update()
    {
        if (ic.GetWasObtained() && player.transform.position.y > crownInitY)
        {
            float alphaSub = 0.04f * (player.transform.position.y - lastPosition.y);
            sr.color = new Color(1f, 1f, 1f, sr.color.a - alphaSub);
            lastPosition = player.transform.position;
        }
    }
}
