using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatformController : MonoBehaviour
{
    [System.Serializable]
    public struct Wave {
        public float amplitude;
        public float period;
        public float speed;
    };
    public Wave xMove;
    public Wave yMove;

    private Vector2 pInitial;
    private Transform originalParent;

    void Start()
    {
        pInitial = transform.position;
    }

    void Update()
    {
        float time = Time.time;
        
        transform.position = new Vector2(
            pInitial.x + (xMove.amplitude * Mathf.Sin(xMove.speed * time * xMove.period)), 
            pInitial.y + (yMove.amplitude * Mathf.Sin(yMove.speed * time * yMove.period))
        );
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("KillWhenFall")) // Player tag
        {
            originalParent = collider.transform.parent;
            collider.transform.parent = this.transform;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("KillWhenFall")) // Player tag
        {
            collider.transform.parent = originalParent;
        }
    }
}
