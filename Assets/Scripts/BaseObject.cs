using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    protected GameObject pictureBelongTo;

    public GameObject getPicture()
    {
        return pictureBelongTo;
    }
}
