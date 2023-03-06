using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public Vector2 offset;
    public float smoothTime = 8f;
    private BoxCollider2D bc;
    private bool wasObtained = false;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            wasObtained = true;
    }

    void LateUpdate()
    {
        if (wasObtained)
            transform.position = LerpPos();
    }

    Vector3 GetTargetPosition()
    {
        return new Vector3(
            PlayerController._instance.transform.position.x,
            PlayerController._instance.transform.position.y,
            transform.position.z
        ) + new Vector3(offset.x, offset.y, 0);
    }

    Vector3 LerpPos()
    {
        Vector3 targetPos = GetTargetPosition();
        return Vector3.Lerp(
            transform.position,
            targetPos,
            (
                Time.deltaTime *
                Mathf.Pow(Vector3.Distance(transform.position, targetPos), 2) / smoothTime
            )
        );
    }
}
