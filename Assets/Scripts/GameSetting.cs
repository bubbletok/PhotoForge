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
    [SerializeField] GameObject[] pictures;
    [SerializeField] GameObject[] platforms; 
    [SerializeField] GameObject player;
    [SerializeField] public int stage;

    Vector3[] picturesOriginPos;
    Vector3[] platformsOriginPos;
    Vector3 playerOriginPos;
    float[] platformOriginDist;
    GameObject[] platformOriginPic;

    int numOfPictureFrag;
    bool isSafe;
    // Start is called before the first frame update
    void Start()
    {
        isSafe = true;
        numOfPictureFrag = GameObject.FindGameObjectsWithTag("Frag").Length;
        picturesOriginPos = new Vector3[pictures.Length];
        platformsOriginPos = new Vector3[platforms.Length];
        platformOriginDist = new float[platforms.Length];
        platformOriginPic = new GameObject[platforms.Length];

        for (int i=0; i<pictures.Length; i++)
        {
            picturesOriginPos[i] = pictures[i].transform.position;
        }
        for (int i = 0; i < platforms.Length; i++)
        {
            platformsOriginPos[i] = platforms[i].transform.position;
            platformOriginDist[i] = platforms[i].GetComponent<PlatformMoving>().distWithPic;
            platformOriginPic[i] = platforms[i].GetComponent<PlatformMoving>().currentPicture;
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
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].transform.position = platformsOriginPos[i];
                platforms[i].GetComponent<PlatformMoving>().distWithPic = platformOriginDist[i];
                platforms[i].GetComponent<PlatformMoving>().currentPicture = platformOriginPic[i];
            }
            player.transform.position = playerOriginPos;
            numOfPictureFrag = -1;
        }
        checkSafeArea();
        if (!isSafe)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void checkSafeArea()
    {
        float paddingY = 1.25f;
        float paddingX = 0.6f;
        float px1, px2, py1, py2;
        bool overlapX1, overlapX2, overlapY1, overlapY2;
        px1 = player.transform.position.x - player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        px2 = player.transform.position.x + player.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        py1 = player.transform.position.y - player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        py2 = player.transform.position.y + player.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        GameObject picture = null;
        List<GameObject> curPictures = new List<GameObject>();
        for (int i = 0; i < pictures.Length; i++)
        {
            picture = pictures[i];
            float x1, x2, y1, y2;
            x1 = picture.transform.position.x - picture.transform.localScale.x * 1.81f / 2 - paddingX;
            x2 = picture.transform.position.x + picture.transform.localScale.x * 1.81f / 2 + paddingX;
            y1 = picture.transform.position.y - picture.transform.localScale.y / 2 - paddingY;
            y2 = picture.transform.position.y + picture.transform.localScale.y / 2 + paddingY;
            overlapX1 = isOverlap(x1, px1, x2);
            overlapX2 = isOverlap(x1, px2, x2);
            overlapY1 = isOverlap(y1, py1, y2);
            overlapY2 = isOverlap(y1, py2, y2);
            if ((overlapX1 && overlapX2) && (overlapY1 && overlapY2))
            {
                curPictures.Add(picture);
                //break;
            }
            else
            {
                picture = null;
            }
        }
        foreach (GameObject curPicture in curPictures)
        {
            if (curPicture == null) continue;
            float y1, y2;
            y1 = curPicture.transform.position.y - curPicture.transform.localScale.y / 2;
            y2 = curPicture.transform.position.y + curPicture.transform.localScale.y / 2;
            if (isOverlap(y1, py1, y2))
            {
                isSafe = true;
                return;
            }
        }
        isSafe = false;
    }

    bool isOverlap(float min, float pos, float max)
    {
        return min <= pos && pos <= max;
    }
}
