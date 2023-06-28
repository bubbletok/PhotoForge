using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] GameObject[] items;
    [SerializeField] float spawnTime = 15f;
    void Update()
    {
        foreach (GameObject item in items)
        {
            if (!item.activeSelf)
            {
                StartCoroutine(waitToSpawn(spawnTime,item));
            }
        }
    }

    IEnumerator waitToSpawn(float time, GameObject item)
    {
        yield return new WaitForSeconds(time);
        item.SetActive(true);
    }
}
