using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using MyBox;


public class Charinputs : MonoBehaviour
{
    public PlayerInput inputs;
    Charcontrol charcontrol;
    GameManager gamemanager;
    InfoHubManager infohub;

    [ReadOnly] public InputAction move;
    [ReadOnly] public InputAction interact;
    [ReadOnly] public InputAction dodge;

    public const string moveInput = "Move";
    public const string interactInput = "Interact";
    public const string dodgeInput = "Dodge";

    private void Awake()
    { 
        inputs = GetComponent<PlayerInput>();

        move = inputs.actions[moveInput];
        interact = inputs.actions[interactInput];
        dodge = inputs.actions[dodgeInput];
    }


    private void Start()
    {
        gamemanager = GameManager.instance;
        charcontrol = GetComponent<Charcontrol>();
    }


    public void Update()
    {
        Debug.Log($"Move value X is {move.ReadValue<Vector2>().x}.");
        Debug.Log($"Interact key value is {interact.ReadValue<float>()}.");
        
    }

    public IEnumerator DisableInputs(float duration)
    {
        yield return new WaitForSeconds(duration);

    }

    // P Pauses the game. Called from Gamemanager Update()
    // O Unpauses the game. Called from Gamemanager Update()
    // J Resets health. Called from Charhealth Update()
    // H Adds 20 health. Called from Charhealth Update()
    // K Takes max damage from health. Called from Charhealth Update()
    // K Also resets enemy health values to max. Called from EnemyHealth Update()
    // L Takes 20 damage from health. Called from Charhealth Update()
    // esc Pauses and unpauses the game. Called from Pause_and_Scene_manager Update()
    // Tab opens and closes the Info Hub. Called from Gamemanager Update()

    //Keyboard.current.aKey.wasPressedThisFrame;        //Returns a bool checking to see if this particular key was pressed
    //inputs.onActionTriggered +=                   //Is called when any button is pressed from this action map
}
