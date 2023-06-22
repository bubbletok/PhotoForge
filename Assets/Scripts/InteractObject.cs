using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractObject : MonoBehaviour
{
    PlayerStatus player;
    private void Start()
    {
        player = gameObject.GetComponent<PlayerStatus>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Frag")
        {
            int fragCount = player.getFragCount();
            player.setFragCount(fragCount + 1);
            other.gameObject.SetActive(false);
        }
        if (other.transform.tag == "EscapeDoor")
        {
            if (other.GetComponent<EscapeDoorSetting>().getSmallDoor() && !player.getSmallPlayer())
                return;

            SceneManager.LoadScene("Stages");
        }
        if(other.transform.tag == "Spike")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if(other.transform.tag == "Shrink")
        {
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            other.gameObject.SetActive(false);
        }
    }
}
