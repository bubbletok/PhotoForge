using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SelectMenu : MonoBehaviour
{
    public void tutorial()
    {
        SceneManager.LoadScene("Stage1");
    }
    public void normal()
    {
        //if (GameManager.instance.clearStages[0])
            SceneManager.LoadScene("Stage2");
    }

    public void hard()
    {
        //if (GameManager.instance.clearStages[1])
            SceneManager.LoadScene("Stage3");
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
    
    public void home()
    {
        SceneManager.LoadScene("Stages");
        Time.timeScale = 1f;
    }
    
    public void StartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
    public void exit()
    {
        Application.Quit();
    }
}
