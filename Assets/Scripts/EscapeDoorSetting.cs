using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeDoorSetting : MonoBehaviour
{
    [SerializeField] bool smallDoor;
    
    public bool getSmallDoor()
    {
        return smallDoor;
    }
}
