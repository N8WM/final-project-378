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
    public GameObject[] turnOnObjects;
    public GameObject[] turnOffObjects;
    private BoxCollider2D bc;
    private bool hasTriggered = false;

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

        if (hasTriggered && allFinished)
        { 
            for (int i = 0; i < destroyAfter.Length; i++)
            {
                Destroy(destroyAfter[i]);
            }

            for (int i = 0; i < turnOnObjects.Length; i++)
            {
                if (!turnOnObjects[i].activeInHierarchy)
                    turnOnObjects[i].SetActive(true);
            }

            for (int i = 0; i < turnOffObjects.Length; i++)
            {
                if (turnOffObjects[i].activeInHierarchy)
                    turnOffObjects[i].SetActive(false);
            }
            hasTriggered = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (Vector2.Distance(transform.position, keyItem.transform.position) < keyItemDistance)
            {
                for (int i = 0; i < animateReceievers.Length; i++)
                    receiverAnimators[i].SetTrigger("Destroy");
                hasTriggered = true;
            }
        }
    }

}
