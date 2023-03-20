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

    public GameObject[] animationObjects;
    public GameObject[] moveObjects;
    public float playerDistanceForAnimation;

    private bool playedAestheticAnimations = false;
    private bool objectsHaveMoved = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        PlayAestheticAnimations();
        MoveAfterAnimation();

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

    void PlayAestheticAnimations() 
    {
        if (!playedAestheticAnimations && playerDistanceForAnimation >= Vector2.Distance(
            PlayerController._instance.transform.position,
            transform.position
        ))
        {
            for (int i = 0; i < animationObjects.Length; i++)
            {
                Animator anim = animationObjects[i].GetComponent<Animator>();
                anim.SetTrigger("InRange");
            }
            playedAestheticAnimations = true;
        }
    }

    void MoveAfterAnimation()
    {
        if (!objectsHaveMoved && playedAestheticAnimations)
        {
            for (int i = 0; i < moveObjects.Length; i++)
            {
                moveObjects[i].GetComponent<BackgroundObjectController>().ActivateEffect();
            }
            objectsHaveMoved = true;
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
