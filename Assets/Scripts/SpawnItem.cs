using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] float spawnTime = 15f;

    bool spawning;

    void Update()
    {
        if (!spawning)
        {
            StartCoroutine(waitToSpawn(spawnTime));
            spawning = true;
        }
    }

    IEnumerator waitToSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        item.SetActive(true);
        spawning = false;
    }
}
