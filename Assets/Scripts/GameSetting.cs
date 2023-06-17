using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    [SerializeField]GameObject escapePicture;
    int numOfPictureFrag;
    // Start is called before the first frame update
    void Start()
    {
        escapePicture.SetActive(false);
       numOfPictureFrag = GameObject.FindGameObjectsWithTag("Frag").Length;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStatus playerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();
        if (playerStatus.getFragCount() == numOfPictureFrag)
        {
            escapePicture.SetActive(true);
        }
    }
}
