using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerStatus : MonoBehaviour
{
    //Frag 상호작용 코드 -> InteractObject로 옮김

    int playerFragCount = 0;
    bool isSmall = false;

    public int getFragCount()
    {
        return playerFragCount;
    }

    public void setFragCount(int count)
    {
        playerFragCount = count;
    }
    public bool getSmallPlayer()
    {
        return isSmall;
    }
    public void setSizeSmall(bool size)
    {
        isSmall = size;
    }
}
