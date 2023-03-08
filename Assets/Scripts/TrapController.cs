using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    private Animator animator;
    private CircleCollider2D cc;
    private bool wasSuccessful = false;
    private bool hasFailed = false;
    private GameObject caughtObject;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Mouth_Closing")
        {
            if (wasSuccessful && !hasFailed)
            {
                GameManager._instance.OnDeath();
                wasSuccessful = false;
            }
            else if (hasFailed)
            {
                Destroy(caughtObject);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        animator.SetTrigger("TriggerTrap");
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            wasSuccessful = true;
        else
        {
            hasFailed = true;
            caughtObject = other.gameObject;
        }
            
    }
}
