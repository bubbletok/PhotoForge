using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    [SerializeField] GameObject[] finalObjects;
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] pictures;
    [SerializeField] GameObject curPicture;
    Vector3[] picturesOriginPos;
    Vector3 playerOriginPos;
    int numOfPictureFrag;
    bool isSafe;
    // Start is called before the first frame update
    void Start()
    {
        isSafe = true;
        numOfPictureFrag = GameObject.FindGameObjectsWithTag("Frag").Length;
        picturesOriginPos = new Vector3[pictures.Length];
        for(int i=0; i<pictures.Length; i++)
        {
            picturesOriginPos[i] = pictures[i].transform.position;
        }
        playerOriginPos = player.transform.position;
        for (int i = 0; i < finalObjects.Length; i++)
        {
            finalObjects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus.getFragCount() == numOfPictureFrag)
        {
            foreach (GameObject finalObject in finalObjects)
                finalObject.SetActive(true);
            for (int i=0; i<pictures.Length; i++)
            {
                pictures[i].transform.position = picturesOriginPos[i];
            }
            player.transform.position = playerOriginPos;
            numOfPictureFrag = -1;
        }
        checkSafeArea();
        /*        if(curPicture == null)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }*/
        //print(isSafe);
        if (!isSafe)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void checkSafeArea()
    {
        curPicture = null;
        float px1, px2, py1, py2;
        px1 = player.transform.position.x - player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        px2 = player.transform.position.x + player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        py1 = player.transform.position.y - player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        py2 = player.transform.position.y + player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        for(int i=0; i<pictures.Length; i++)
        {
            GameObject picture = pictures[i];
            //if (picture == null) continue;
            float x1, x2, y1, y2;
            x1 = picture.transform.position.x - picture.transform.localScale.x / 2;
            x2 = picture.transform.position.x + picture.transform.localScale.x / 2;
            y1 = picture.transform.position.y - picture.transform.localScale.y / 2;
            y2 = picture.transform.position.y + picture.transform.localScale.y / 2;
            //print(picture.name);
            //print(x1 + " " + x2 + " " + y1 + " " + y2);
            /*            bool overlapX1, overlapX2, overlapY1, overlapY2;
                        overlapX1 = isOverlap(x1, px1, x2);
                        overlapX2 = isOverlap(x1, px2, x2);
                        overlapY1 = isOverlap(y1, py1, y2);
                        overlapY2 = isOverlap(y1, py2, y2);
                        //print(picture.name + " " + overlapY1);
                        if (overlapX1 && overlapX2 && overlapY2 && overlapY1)
                        {
                            curPicture = picture;
                        }
                        if (curPicture != null && overlapY1)
                        {
                            isSafe = true;
                            return;
                        }*/
            /*            else
                        {
                            isSafe = false;
                        }*/
            if (isOverlap(y1, py1, y2))
            {
                isSafe = true;
                return;
            }
        }
        if(isSafe) return;
        isSafe = false;
    }

    bool isOverlap(float min, float pos, float max)
    {
        return min <= pos && pos <= max;
    }
}
