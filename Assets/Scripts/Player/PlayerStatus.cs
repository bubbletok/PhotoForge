using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerStatus : MonoBehaviour
{
    //InteractObject의 자식 클래스(InteractPicture)로 옮기기?
    BoxCollider2D coll;
    bool isInPicture = true;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        isInPicture = false;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(coll.bounds.center, coll.bounds.size, 0f, new Vector2(0, 0), 0f);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.transform.tag == "Picture")
            {
                isInPicture = true;
                break;
            }
        }
    }

    private void LateUpdate()
    {
        if (!isInPicture)
        {
            //죽음 효과 or UI 표시

            //씬 재로드
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.tag == "EscapeDoor")
        {
            print("!@#");
            SceneManager.LoadScene("Stages");
        }
    }

    //InteractObject의 자식 클래스로(InteractFrag) 옮기기?

    int playerFragCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Frag")
        {
            ++playerFragCount;
            other.gameObject.SetActive(false);
            Debug.Log(playerFragCount);
        }
    }
    public int getFragCount()
    {
        return playerFragCount;
    }
}
