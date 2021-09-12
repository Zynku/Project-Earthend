using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestGiver))]

public class QuestEventIDBuilder : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuestGiver myScript = (QuestGiver)target;
        if (GUILayout.Button("Build Event IDs"))
        {
            myScript.GenerateQuestEventID();
        }
    }
}
