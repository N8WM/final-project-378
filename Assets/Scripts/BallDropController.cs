using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDropController : MonoBehaviour
{
    private CircleCollider2D cc;
    private Rigidbody2D rb;

    void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
