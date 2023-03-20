using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectController : MonoBehaviour
{
    public enum WhatDo { MOVE, DELETE };

    private Animator animator;
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    public WhatDo howToAffect;
    public float maxY;
    private bool isActivated = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isActivated)
        {
            switch (howToAffect)
            {
                case WhatDo.MOVE:
                    rb.MovePosition(rb.position + (new Vector2(0, 0.35f) * Time.fixedDeltaTime));
                    if (rb.position.y >= maxY)
                        isActivated = false;
                    break;
                case WhatDo.DELETE:
                    Destroy(gameObject);
                    isActivated = false;
                    break;
            }
        }
    }

    public void ActivateEffect()
    {
        isActivated = true;
    }
}
