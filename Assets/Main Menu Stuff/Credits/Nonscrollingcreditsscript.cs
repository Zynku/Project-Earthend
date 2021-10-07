using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nonscrollingcreditsscript : MonoBehaviour
{
    public bool finishedShowing;
    Creditsscript creditsscript;


    private void Start()
    {
        creditsscript = GetComponentInParent<Creditsscript>();
    }

    public void FinishedShowing()
    {
        finishedShowing = true;
    }
}
