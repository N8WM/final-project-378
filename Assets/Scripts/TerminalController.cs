using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalController : MonoBehaviour
{
    Transform leftT, rightT;
    LineRenderer lr;
    BoxCollider2D bc;
    Color color;
    [SerializeField] LayerMask enableMask;

    float effectSpeed = 1f;
    float countdown_start = 0.1f;
    float countdown = 0f;
    bool on = false;
    bool updateVal = false;

    void Start()
    {
        leftT = transform.Find("LT");
        rightT = transform.Find("RT");
        lr = GetComponent<LineRenderer>();
        bc = GetComponent<BoxCollider2D>();
        color = lr.material.color;
        lr.positionCount = 2;
    }

    void FixedUpdate()
    {
        on = countdown > 0;

        if (on && !updateVal) {
            Enable();
            updateVal = true;
        } else if (!on && updateVal) {
            Disable();
            updateVal = false;
        }

        countdown = Mathf.Max(0, countdown - Time.fixedDeltaTime);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer & enableMask.value) > 0)
            countdown = countdown_start;
    }

    void Enable()
    {
        lr.SetPosition(0, leftT.position);
        lr.SetPosition(1, rightT.position);
        bc.size = new Vector2(Vector2.Distance(leftT.position, rightT.position), 1f);
        bc.offset = new Vector2(Mathf.Abs(rightT.position.x - leftT.position.x) / 2f, 0);
        StartCoroutine(OnEffect());
    }

    void Disable()
    {
        StartCoroutine(OffEffect());
    }

    IEnumerator OnEffect()
    {
        lr.enabled = true;
        bc.enabled = true;
        float t = 0;
        while (t < effectSpeed)
        {
            if (!on)
                break;
            t += Time.deltaTime;
            lr.material.color = Color.Lerp(lr.material.color, color, t);
            yield return null;
        }
    }

    IEnumerator OffEffect()
    {
        bc.enabled = false;
        float t = 0;
        while (t < effectSpeed)
        {
            if (on)
                break;
            t += Time.deltaTime;
            lr.material.color = Color.Lerp(lr.material.color, color * new Vector4(1,1,1,0), t);
            yield return null;
        }
        if (!on) lr.enabled = false;
    }

    void OnDrawGizmos()
    {
        leftT = transform.Find("LT");
        rightT = transform.Find("RT");
        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftT.position + Vector3.up, rightT.position + Vector3.up);
        Gizmos.DrawLine(leftT.position + Vector3.down, rightT.position + Vector3.down);
    }
}
