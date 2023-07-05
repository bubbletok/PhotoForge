using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarInteraction : MonoBehaviour
{
    [SerializeField] GameObject lockObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Star")
        {
            if (collision.gameObject.GetComponent<AudioSource>() == null)
                gameObject.GetComponent<AudioSource>().Play();

            lockObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Star")
        {
            lockObject.SetActive(true);
        }
    }
}
