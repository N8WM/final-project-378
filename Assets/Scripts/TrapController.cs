using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    private Animator animator;
    private CircleCollider2D cc;
    private bool wasSuccessful = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (wasSuccessful && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Mouth_Closing")
        {
            GameManager._instance.OnDeath();
            wasSuccessful = false;
        }
            
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        animator.SetTrigger("TriggerTrap");
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            wasSuccessful = true;
        
    }
}
