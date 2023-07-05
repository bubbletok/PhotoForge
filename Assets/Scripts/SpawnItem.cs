using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] GameObject[] items;
    [SerializeField] float spawnTime = 10f;
    [SerializeField] bool[] spawnings;

    private void Start()
    {
        spawnings = new bool[items.Length];
        spawnTime = 10f;
    }
    void Update()
    {
        for (int i = 0; i < items.Length; i++)
        {
            GameObject item = items[i];
            if (!item.activeSelf && !spawnings[i])
            {
                spawnings[i] = true;
                StartCoroutine(waitToSpawn(spawnTime, item, i));
            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (!spawnings[i])
            {
                items[i].SetActive(true);
            }
        }
    }

    IEnumerator waitToSpawn(float time, GameObject item, int idx)
    {
        yield return new WaitForSeconds(time);
        item.SetActive(true);
        spawnings[idx] = false;
    }
}