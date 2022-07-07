using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using MyBox;

public class Main_Menu_manager : MonoBehaviour
{
    [ReadOnly] public GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void CallSceneSwitcher(int index)
    {
        gameManager.pause_Menu_Manager.SceneSwitcher(index);
    }
}
