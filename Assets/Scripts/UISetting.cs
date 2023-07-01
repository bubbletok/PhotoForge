using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UISetting : MonoBehaviour
{
    [SerializeField] GameObject setting;
    [SerializeField] Text stageText;
    public void openSetting()
    {
        setting.SetActive(true);
        stageText.text = SceneManager.GetActiveScene().name;
        Time.timeScale = 0f;
    }
    public void closeSetting()
    {
        setting.SetActive(false);
        Time.timeScale = 1f;
    }
}
