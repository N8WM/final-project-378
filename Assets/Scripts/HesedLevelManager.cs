using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(PlayerInput))]

public class HesedLevelManager : MonoBehaviour
{
    [Header("Main")]
    public GameObject player;

    [Header("Hand Trap")]
    public GameObject handTrap;
    public float trapYDifference; // -1.6f
    private bool caughtInTrap = false;
    private bool triggeredHand = false;

    [Header("Ring")]
    public GameObject ring;
    public GameObject ringTrigger;
    public Vector2[] ringPositions;
    private bool enteredSelectionProcess = false;
    private int positionIndex = 0;

    [Header("Arrows")]
    public GameObject arrowIcons;
    public Vector2 arrowPosition;

    [Header("Platforms")]
    public GameObject platformsToDelete;
    public GameObject bridgePlatform;
    public Vector2 bridgePosition;
    public GameObject door;
    public Vector2 doorPosition;

    public void Move(InputValue movementValue)
    {
        if (enteredSelectionProcess)
        {
            float val = movementValue.Get<float>();
            if (val < 0)
            {
                positionIndex = Modulo((positionIndex - 1), ringPositions.Length);
                ring.transform.position = ringPositions[positionIndex];   
            }
            else if (val > 0)
            {
                positionIndex = Modulo((positionIndex + 1), ringPositions.Length);
                ring.transform.position = ringPositions[positionIndex];
            }
        }
    }

    public void Select(InputValue selectValue)
    {
        if (enteredSelectionProcess)
        {
            enteredSelectionProcess = false;
            Destroy(arrowIcons);
            if (positionIndex == 2) // ring finger
            {
                Destroy(platformsToDelete);
                PlayerController._instance.inputDisabled = false;
                bridgePlatform.transform.position = bridgePosition;
                door.transform.position = doorPosition;
                Destroy(gameObject);
            } 
            else
            {
                handTrap.transform.position = new Vector2(
                    player.transform.position.x,
                    handTrap.transform.position.y
                );
                
            }
        }
    }

    void Update()
    {
        if (ringTrigger == null && !enteredSelectionProcess)
        {
            enteredSelectionProcess = true;
            ring.transform.position = ringPositions[positionIndex];
            PlayerController._instance.inputDisabled = true;
            arrowIcons.transform.position = arrowPosition;
        }
    }

    void FixedUpdate()
    {
        if (triggeredHand)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Rigidbody2D trapRb = handTrap.GetComponent<Rigidbody2D>();

            playerRb.velocity = new Vector2(0f, 0f);
            trapRb.position = Vector3.Lerp(
                trapRb.position,
                playerRb.position,
                (
                    Time.deltaTime * Vector3.Distance(trapRb.position, playerRb.position)
                )
            );
            
            if (trapRb.position.y - playerRb.position.y >= trapYDifference)
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
        else if (player.transform.position.x >= handTrap.transform.position.x)
        {
            triggeredHand = true;
            PlayerController._instance.inputDisabled = true;
        }
    }

    int Modulo(int a, int b) 
    {
        a %= b;
        return (a < 0) ? a + b : a;
    }

}
