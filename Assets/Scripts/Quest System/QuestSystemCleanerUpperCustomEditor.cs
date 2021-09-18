using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestSystemCleanerUpper))]

public class QuestSystemCleanerUpperCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuestSystemCleanerUpper myScript = (QuestSystemCleanerUpper)target;
        if (GUILayout.Button("Toggle Quest Object Sprites"))
        {
            myScript.ToggleQuestObjectSprites();
        }
    }
}
