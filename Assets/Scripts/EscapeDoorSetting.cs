using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeDoorSetting : MonoBehaviour
{
    [SerializeField] bool smallDoor;
    // Start is called before the first frame update
    
    public bool getSmallDoor()
    {
        return smallDoor;
    }
}
