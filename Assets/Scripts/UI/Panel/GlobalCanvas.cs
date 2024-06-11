using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalCanvas : MonoBehaviour
{
    private static GlobalCanvas m_instance;
    
    public static GlobalCanvas Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject canvas = GameObject.Instantiate<GameObject>(Resources.Load("Prefabs/UI/Panel/GlobalCanvas") as GameObject);
                m_instance = canvas.GetComponent<GlobalCanvas>();
            }
            return m_instance;
        }
    }

    public void Awake()
    {
        m_instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
