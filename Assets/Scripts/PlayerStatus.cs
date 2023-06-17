using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private int playerFlagCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Flag")
        {
            ++playerFlagCount;
            other.gameObject.SetActive(false);
            Debug.Log(playerFlagCount);
        }
    }

}
