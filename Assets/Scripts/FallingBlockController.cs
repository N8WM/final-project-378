using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockController : MonoBehaviour
{
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    public float gravity;
    private int moveableLayer;
    private int groundLayer;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        moveableLayer = LayerMask.NameToLayer("Movable");
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    void FixedUpdate()
    {
        // if (gameObject.layer == moveableLayer)
        //     rb.MovePosition(
        //         rb.position + 
        //         (new Vector2(0, -10) * Time.fixedDeltaTime) + 
        //         (0.5f * new Vector2(0, -gravity) * Mathf.Pow(Time.fixedDeltaTime, 2f))
        //     );
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == groundLayer)
        {
            gameObject.layer = groundLayer;
            bc.isTrigger = false;
            bc.offset = new Vector2(0, 0);
            bc.size = new Vector2(0.99f, 0.99f);
            StartCoroutine(Freeze());
        }
            
    }

    IEnumerator Freeze()
    {
        yield return new WaitForSeconds(0.5f);
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (gameObject.layer == moveableLayer)
        { 
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") && PlayerController._instance.GetIsGrounded())
                GameManager._instance.OnDeath();
        }
        
    }
}
