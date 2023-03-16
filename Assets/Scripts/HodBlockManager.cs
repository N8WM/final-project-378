using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HodBlockManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public float blockSize = 2.5f;
    public float leftmostBlockXPos = -3.27f;
    public int blockCount = 5;
    public float blockHeight = 75f;
    public float maxHeight = 17f;
    private float[] xPositions;

    void Start()
    {
        xPositions = new float[blockCount];
        xPositions[0] = leftmostBlockXPos;
        for (int i = 1; i < blockCount; i++)
            xPositions[i] = xPositions[i - 1] + blockSize;
        StartCoroutine(blockSpawn());
    }

    IEnumerator blockSpawn() {
        while (PlayerController._instance.transform.position.y < maxHeight) {
            float catX = PlayerController._instance.transform.position.x;
            for (int i = 0; i < blockCount; i++)
            {
                if (catX <= xPositions[i] + (blockSize / 2))
                {
                    Instantiate(blockPrefab, new Vector2(xPositions[i], blockHeight), Quaternion.identity);
                    break;
                }
                
            }
            yield return new WaitForSeconds(3);
        }    
    }

}
