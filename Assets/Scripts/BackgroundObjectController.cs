using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectController : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D bc;

    void Start()
    {
        animator = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Movable"))
            animator.SetTrigger("Disappear");
    }
}
