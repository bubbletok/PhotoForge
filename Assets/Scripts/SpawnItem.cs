using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] GameObject[] items;
    [SerializeField] float spawnTime = 10f;
    bool[] spawnings;

    private void Start()
    {
        spawnings = new bool[items.Length];
    }
    void Update()
    {
        for(int i=0; i<items.Length;i++)
        {
            GameObject item = items[i];
            if (!item.activeSelf && !spawnings[i])
            {
                spawnings[i] = true;
                StartCoroutine(waitToSpawn(spawnTime, item, spawnings[i]));
            }
        }
    }

    IEnumerator waitToSpawn(float time, GameObject item, bool spawning)
    {
        yield return new WaitForSeconds(time);
        item.SetActive(true);
        spawning = false;
    }
}
