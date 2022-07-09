using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class welcome_screen : MonoBehaviour
{
    public GameObject welcomeScreen;

    public void SetThisAsInactive()
    {
        welcomeScreen?.SetActive(false);
    }

    public void PauseGame()
    {
        GameManager.instance.PauseGame();
    }

    public void ResumeGame()
    {
        GameManager.instance.ResumeGame();
    }
}
