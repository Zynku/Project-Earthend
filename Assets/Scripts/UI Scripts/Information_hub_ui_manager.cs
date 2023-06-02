using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Information_hub_ui_manager : MonoBehaviour
{
    public bool IHUIActive;

    public VisualElement root;
    public VisualElement parentElement;
    Button leftArrow;
    Button rightArrow;
    List<VisualElement> inventoryslots;

    private void OnValidate()   //Runs at the start of script loading in edit mode
    {
        EditorApplication.playModeStateChanged += DisableUIInEditMode;
    }

    public void DisableUIInEditMode(PlayModeStateChange state)
    {
        try
        {
            UIDocument UIDoc = GetComponent<UIDocument>();
            UIDoc.enabled = false;
            Debug.Log("Disabling IHUI in edit mode");
        }
        catch (MissingReferenceException)
        {
        }

    }

    private void Awake()
    {
        UIDocument UIDoc = GetComponent<UIDocument>();
        UIDoc.enabled = true;
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.SetEnabled(true);
        parentElement = root.Q<VisualElement>("whole-screen");
        leftArrow = root.Q<Button>("page-left-button");
        rightArrow = root.Q<Button>("page-right-button");
        inventoryslots = root.Query<VisualElement>("IHUI-sprite").ToList();

        //Debug.Log($"{inventoryslots.Count} inventory slots counted!");

        leftArrow.clickable.clicked += () => DebugYourLog();
        rightArrow.clickable.clicked -= () => DebugYourLog();
    }

    private void Start()
    {
        parentElement.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (Charinputs.instance.IHUI.WasPressedThisFrame())
        {
            if (!IHUIActive) { parentElement.style.display = DisplayStyle.Flex; }
            else { parentElement.style.display = DisplayStyle.None; }
        }
    }

    public void DebugYourLog()
    {
        Debug.Log("Debugged!");
    }


}
