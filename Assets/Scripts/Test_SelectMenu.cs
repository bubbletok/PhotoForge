using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Test_SelectMenu : MonoBehaviour
{
    public void tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void normal()
    {
        SceneManager.LoadScene("Normal");
    }

    public void hard()
    {
        SceneManager.LoadScene("Hard");
    }

    public void back()
    {
        SceneManager.LoadScene("Stages_test");
    }
}
