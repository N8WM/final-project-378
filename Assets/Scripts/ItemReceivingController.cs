using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReceivingController : MonoBehaviour
{
    public GameObject keyItem;
    public GameObject[] ItemReceievers;
    public float keyItemDistance = 5f;
    private BoxCollider2D bc;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (Vector2.Distance(transform.position, keyItem.transform.position) < keyItemDistance)
            {
                // Play destroy animation
                Destroy(keyItem);
                for (int i = 0; i < ItemReceievers.Length; i++)
                    Destroy(ItemReceievers[i]);
                Destroy(gameObject);
            }
        }
    }

}
