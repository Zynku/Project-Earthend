using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Information_hub_ui_manager : MonoBehaviour
{
    public VisualElement root;
    public VisualElement parentElement;
    Button leftArrow;
    Button rightArrow;
    List<VisualElement> inventoryslots;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.SetEnabled(true);
        parentElement = root.Q<VisualElement>("whole-screen");
        leftArrow = root.Q<Button>("page-left-button");
        rightArrow = root.Q<Button>("page-right-button");
        inventoryslots = root.Query<VisualElement>("inventory-sprite").ToList();

        //Debug.Log($"{inventoryslots.Count} inventory slots counted!");

        leftArrow.clickable.clicked += () => DebugYourLog();
        rightArrow.clickable.clicked -= () => DebugYourLog();
    }

    private void Start()
    {
        parentElement.style.display = DisplayStyle.None;
    }

    public void DebugYourLog()
    {
        Debug.Log("Debugged!");
    }


}
