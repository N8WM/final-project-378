using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController _instance;

    public Vector2 offset = new Vector2(0f, 1f);
    public float smoothTime = 3f;

    void Awake()
    {
        if (_instance != this && _instance != null)
            Destroy(gameObject);
        else
            _instance = this;
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
            PlayerController._instance.transform.position.x,
            PlayerController._instance.transform.position.y,
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
