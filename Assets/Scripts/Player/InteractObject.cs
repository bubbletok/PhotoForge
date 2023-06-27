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
            //���� ���� ������
            //���� ���� ���� + 1
            int fragCount = player.getFragCount();
            player.setFragCount(fragCount + 1);
            other.gameObject.SetActive(false);
        }
        if (other.transform.tag == "EscapeDoor")
        {
            //���� ���̶��, �÷��̾ ��ҵǾ����� Ȯ��
            if (other.GetComponent<EscapeDoorSetting>().getSmallDoor() && !player.getSmallPlayer())
                return;

            SceneManager.LoadScene("Stages_test");
        }
        if(other.transform.tag == "Spike")
        {
            //���
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if(other.transform.tag == "Shrink")
        {
            //ũ�� ���
            gameObject.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
            //���� �� ��� ���� �߰��ؼ� ���� �ȵǴ°� ����
            gameObject.GetComponent<Rigidbody2D>().velocity += Vector2.up;
            other.gameObject.SetActive(false);
            StartCoroutine(waitToExpand());
        }
    }

    IEnumerator waitToExpand()
    {
        //10�� �� �ٽ� ���� ũ���
        yield return new WaitForSeconds(10f);
        gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
    }
}
