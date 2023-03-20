using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class HesedLevelManager : MonoBehaviour
{
    public GameObject player;
    public GameObject[] handTraps;
    public float trapYDifference; // -1.6
    private int trapNumber = -1;
    private bool caughtInTrap = false;

    void Start()
    {
        // Component[] components = player.GetComponents(typeof(Component));
        // foreach(Component component in components)
        // {
        //     Debug.Log(component.ToString());
        // }

    }

    void FixedUpdate()
    {
        if (trapNumber > -1)
        {
            TriggerRisingTraps(handTraps[trapNumber]);
        }
        else
        {
            for (int i = 0; i < handTraps.Length; i++)
                if (player.transform.position.x >= handTraps[i].transform.position.x)
                {
                    trapNumber = i;
                    player.GetComponent<PlayerInput>().DeactivateInput();
                }
        }
        
    }

    void TriggerRisingTraps(GameObject trap)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        Rigidbody2D trapRb = trap.GetComponent<Rigidbody2D>();

        playerRb.velocity = new Vector2(0f, 0f);
        trapRb.position = Vector3.Lerp(
            trapRb.position,
            playerRb.position,
            (
                Time.deltaTime * Vector3.Distance(trapRb.position, playerRb.position)
            )
        );
        
        

        if (trapRb.position.y - playerRb.position.y >= trapYDifference)
        //     trapRb.MovePosition(trapRb.position + (new Vector2(0, 6f) * Time.fixedDeltaTime));
        // else
        {
            if (caughtInTrap)
                trapRb.position = new Vector2(playerRb.position.x, playerRb.position.y + trapYDifference);
            else
            {
                GameManager._instance.OnDeath();
                caughtInTrap = true;
            }
        }
    }
}
