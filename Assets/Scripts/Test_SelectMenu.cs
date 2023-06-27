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
    public void stage1()
    {
        SceneManager.LoadScene("Normal1");
    }
    public void back()
    {
        SceneManager.LoadScene("Stages_test");
    }
}
