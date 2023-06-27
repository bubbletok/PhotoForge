using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
            //사진 조각 얻으면
            //사진 조각 갯수 + 1
            int fragCount = player.getFragCount();
            player.setFragCount(fragCount + 1);
            other.gameObject.SetActive(false);
        }
        if (other.transform.tag == "EscapeDoor")
        {
            //작은 문이라면, 플레이어가 축소되었는지 확인
            if (other.GetComponent<EscapeDoorSetting>().getSmallDoor() && !player.getSmallPlayer())
                return;

            SceneManager.LoadScene("Stages_test");
        }
        if(other.transform.tag == "Spike")
        {
            //사망
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if(other.transform.tag == "Shrink")
        {
            //크기 축소
            gameObject.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
            //먹을 때 잠깐 위로 뜨게해서 점프 안되는거 방지
            gameObject.GetComponent<Rigidbody2D>().velocity += Vector2.up;
            other.gameObject.SetActive(false);
            StartCoroutine(waitToExpand());
        }
    }

    IEnumerator waitToExpand()
    {
        //10초 후 다시 원래 크기로
        yield return new WaitForSeconds(10f);
        gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
    }
}
