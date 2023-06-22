using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

public class OnPlatform : MonoBehaviour
{
    GameObject platform;
    PictureMovement pic;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            pic = collision.gameObject.GetComponentInParent<PictureMovement>();
            pic.setPlayer(gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            pic = collision.gameObject.GetComponentInParent<PictureMovement>();
            pic.setPlayer(null);
        }
    }
}
