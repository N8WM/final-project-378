using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorTarget doorTarget = null;
    private float targetUIAlpha = 0f;
    private float targetIconAlpha = 0f;
    private float currentUIAlpha = 0f;
    private float currentIconAlpha = 0f;
    private TextMeshProUGUI doorName;
    private SpriteRenderer doorShadow, enterIcon;
    private bool isPlayerInTrigger = false;
    private bool lockedCache;

    void Start()
    {
        doorShadow = transform.Find("Aesthetic").gameObject.GetComponent<SpriteRenderer>();
        doorName = transform.Find("UI/Name").gameObject.GetComponent<TextMeshProUGUI>();
        enterIcon = transform.Find("UI/Enter Icon").gameObject.GetComponent<SpriteRenderer>();

        lockedCache = !doorTarget.unlockedInLevel;

        if (doorTarget != null)
        {
            targetUIAlpha = 0f;
            targetIconAlpha = 0f;
            doorName.text = doorTarget.doorTitle;
            if (lockedCache && doorTarget.locked)
                doorShadow.color = new Color(0,0,0,0);
            else
                doorShadow.color = doorTarget.doorColor;
            UpdateColors();
        }
        else Debug.LogError("DoorController: No DoorTarget assigned to door!");
    }

    void UpdateColors()
    {
        currentUIAlpha = Mathf.Lerp(currentUIAlpha, targetUIAlpha, Time.deltaTime * 5f);
        currentIconAlpha = Mathf.Lerp(currentIconAlpha, targetIconAlpha, Time.deltaTime * 5f);
        doorName.color = new Color(
            doorTarget.doorColor.r,
            doorTarget.doorColor.g,
            doorTarget.doorColor.b,
            doorTarget.doorColor.a * (
                (doorTarget.locked && lockedCache) ? 0 :
                doorTarget.keepTitleVisible ? 1 :
                currentUIAlpha
            )
        );
        enterIcon.color = new Color(
            doorTarget.doorColor.r * 0.7f,
            doorTarget.doorColor.g * 0.7f,
            doorTarget.doorColor.b * 0.7f,
            currentIconAlpha
        );
    }

    void Update()
    {
        if (isPlayerInTrigger) targetUIAlpha = 1f;
        else targetUIAlpha = 0f;
        if (PlayerController._instance.isMoving ||
            !isPlayerInTrigger ||
            (doorTarget.locked && lockedCache)) targetIconAlpha = 0f;
        else targetIconAlpha = 1f;
        UpdateColors();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (doorTarget != null)
            {
                PlayerController._instance.SetTargetDoor(doorTarget);
                isPlayerInTrigger = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (doorTarget != null)
            {
                PlayerController._instance.SetTargetDoor(null);
                isPlayerInTrigger = false;
            }
        }
    }
}
