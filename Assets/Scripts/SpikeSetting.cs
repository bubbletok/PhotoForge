using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSetting : MonoBehaviour
{
    [SerializeField] GameObject picture;
    Vector3 distance;
    // Start is called before the first frame update
    void Start()
    {
        distance = transform.position - picture.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = picture.transform.position + distance;
    }
}
