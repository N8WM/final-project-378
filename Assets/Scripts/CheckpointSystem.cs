using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public bool lvl2 = false;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask checkPoint;

    void FixedUpdate()
    {
        string checkName = CheckpointUpdate().name;
        if(checkName.Equals("LVL2Check")) {
            Debug.Log("Checked");
            lvl2 = true;
        }
    }

    private Object CheckpointUpdate() {
        return Physics2D.OverlapCircle(rb.position, 0.2f, checkPoint);
    }
}
