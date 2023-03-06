using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectController : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D bc;
    public GameObject[] objectsToDelete;

    void Start()
    {
        animator = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Movable"))
        {
            animator.SetTrigger("Disappear");
            for (int i = 0; i < objectsToDelete.Length; i++)
                Destroy(objectsToDelete[i]);
        }
            
    }
}
