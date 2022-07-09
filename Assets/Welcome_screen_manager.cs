using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Welcome_screen_manager : MonoBehaviour
{
    public GameObject welcomeScreen;

    public void ShowWelcomeScreen()
    {
        welcomeScreen.SetActive(true);
    }
}
