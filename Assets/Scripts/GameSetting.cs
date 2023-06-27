using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    [SerializeField] GameObject escapePicture;
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] pictures;
    int numOfPictureFrag;
    bool isOverlapped;
    // Start is called before the first frame update
    void Start()
    {
        isOverlapped = true;
       escapePicture.SetActive(false);
       numOfPictureFrag = GameObject.FindGameObjectsWithTag("Frag").Length;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus.getFragCount() == numOfPictureFrag)
        {
            escapePicture.SetActive(true);
        }
        checkPicutreArea();
        if (!isOverlapped)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void checkPicutreArea()
    {
        float px1, py1, px2, py2;
        px1 = player.transform.position.x - player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        px2 = player.transform.position.x + player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        py1 = player.transform.position.y + player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        py2 = player.transform.position.y - player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        foreach (GameObject picture in pictures)
        {
            if (picture == null) continue;
            float x1, y1, x2, y2;
            x1 = picture.transform.position.x - picture.transform.localScale.x/2;
            x2 = picture.transform.position.x + picture.transform.localScale.x/2;
            y1 = picture.transform.position.y + picture.transform.localScale.y/2;
            y2 = picture.transform.position.y - picture.transform.localScale.y/2;
            /*            if(isOverlap(x1,px1,x2) && isOverlap(x1,px2,x2) && isOverlap(y2,py1,y1) && isOverlap(y2, py2, y1))
                        {
                            isOverlapped = true;
                            return;
                        }*/
            if (isOverlap(y2, py2, y1))
            {
                isOverlapped = true;
                return;
            }
        }
        isOverlapped = false;
    }

    bool isOverlap(float min, float pos, float max)
    {
        return min <= pos && pos <= max;
    }
}
