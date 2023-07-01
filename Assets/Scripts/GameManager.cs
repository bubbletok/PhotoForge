using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }

            return m_instance;
        }
    }
    static GameManager m_instance;

    public bool[] clearStages;

    private void Awake()
    {
        if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        clearStages = new bool[3];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
