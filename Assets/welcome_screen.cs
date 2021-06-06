using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class welcome_screen : MonoBehaviour
{
    public GameObject welcomeScreen;

    public void SetThisAsInactive()
    {
        if (welcomeScreen != null)
        {
            welcomeScreen.SetActive(false);
            Destroy(welcomeScreen);
        }
    }

    public void PauseGame()
    {
        gamemanager.instance.PauseGame();
    }

    public void ResumeGame()
    {
        gamemanager.instance.ResumeGame();
    }
}
