using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReceivingController : MonoBehaviour
{
    public float keyItemDistance = 5f;
    public GameObject keyItem;
    public GameObject[] animateReceievers;
    private Animator[] receiverAnimators;
    public GameObject[] destroyAfter;
    private BoxCollider2D bc;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        receiverAnimators = new Animator[animateReceievers.Length];
        for (int i = 0; i < receiverAnimators.Length; i++)
        {
            receiverAnimators[i] = animateReceievers[i].GetComponent<Animator>(); 
        }
    }

    void Update()
    {
        bool allFinished = true;
        for (int i = 0; i < receiverAnimators.Length; i++)
        {
            if (receiverAnimators[i].GetCurrentAnimatorClipInfo(0)[0].clip.name != "Finished")
                allFinished = false;
        }

        if (allFinished)
        { 
            for (int i = 0; i < destroyAfter.Length; i++)
            {
                Destroy(destroyAfter[i]);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (Vector2.Distance(transform.position, keyItem.transform.position) < keyItemDistance)
            {
                Destroy(keyItem);
                for (int i = 0; i < animateReceievers.Length; i++)
                    receiverAnimators[i].SetTrigger("Destroy");
            }
        }
    }

}
