using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 offset = new Vector2(0f, 1f);
    public float smoothTime = 3f;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager._camera = this;
    }

    void Start()
    {
        transform.position = GetTargetPosition();
    }

    void LateUpdate()
    {
        transform.position = LerpCamPos();
    }

    Vector3 GetTargetPosition()
    {
        return new Vector3(
            gameManager._player.transform.position.x,
            gameManager._player.transform.position.y,
            transform.position.z
        ) + new Vector3(offset.x, offset.y, 0);
    }

    Vector3 LerpCamPos()
    {
        Vector3 targetPos = GetTargetPosition();
        return Vector3.Lerp(
            transform.position,
            targetPos,
            (
                Time.deltaTime *
                Mathf.Pow(Vector3.Distance(transform.position, targetPos), 4) /
                smoothTime
            )
        );
    }
}
