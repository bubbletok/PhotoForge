using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InteractObject : MonoBehaviour
{
    PlayerStatus player;

    public AudioClip potionDrink;
    public AudioClip potionRegen;

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

            if(other.gameObject.GetComponent<AudioSource>() != null)
            {
                gameObject.GetComponent<AudioSource>().clip = other.gameObject.GetComponent<AudioSource>().clip;
                gameObject.GetComponent<AudioSource>().Play();
            }

            other.gameObject.SetActive(false);
        }
        if (other.transform.tag == "EscapeDoor")
        {
            if (other.GetComponent<EscapeDoorSetting>() != null && other.GetComponent<EscapeDoorSetting>().getSmallDoor() && !player.getSmallPlayer())
                return;

            SceneManager.LoadScene("Stages");
        }
        if(other.transform.tag == "Spike")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if(other.transform.tag == "Shrink")
        {
            GetComponent<AudioSource>().clip = potionDrink;
            GetComponent<AudioSource>().Play();
            gameObject.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
            gameObject.GetComponent<Rigidbody2D>().velocity += Vector2.up;
            player.setSizeSmall(true);
            other.gameObject.SetActive(false);
            StartCoroutine(waitToExpand());
        }
    }

    IEnumerator waitToExpand()
    {
        //10�� �� �ٽ� ���� ũ���
        yield return new WaitForSeconds(10f);
        GetComponent<AudioSource>().clip = potionRegen;
        GetComponent<AudioSource>().Play();
        gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        player.setSizeSmall(false);
    }
}
