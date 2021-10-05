using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charattacks : MonoBehaviour
{
    public string currentState;
    Charcontrol charcontrol;
    Charanimation charanimation;


    // Start is called before the first frame update
    void Start()
    {
        charcontrol = GetComponent<Charcontrol>();
        charanimation = GetComponent<Charanimation>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState = charcontrol.currentState.ToString();
        switch (charcontrol.currentState)
        {
            case Charcontrol.State.COMBAT_Idle:
                break;
        }
    }

    public void Combat_Idle()
    {
        charcontrol.currentState = Charcontrol.State.COMBAT_Idle;
        charcontrol.airJumpsHas = charcontrol.airJumps;
        charcontrol.rolled = false;

        charcontrol.canFlipXDir();
    }

    public void Combat_Running()
    {
        charcontrol.rb2d.velocity = new Vector2(charcontrol.runSpeed * Input.GetAxis("Horizontal"), charcontrol.rb2d.velocity.y);

        charcontrol.rolled = false;
        charcontrol.canFlipXDir();
    }
}
