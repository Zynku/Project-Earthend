using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectGameObject : MonoBehaviour
{
    //This script literally just detects if the player has entered the area defined by this gameObject, and thats it.
    public QuestLogic myQuestObject;
    
    public void Setup(QuestLogic questObject)
    {
        myQuestObject = questObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
