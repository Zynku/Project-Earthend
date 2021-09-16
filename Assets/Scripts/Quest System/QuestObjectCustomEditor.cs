using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[CustomEditor(typeof(QuestObject))]
[CanEditMultipleObjects]
public class QuestObjectCustomEditor : Editor
{
    /*
    //TODO: FIX THIS TO APPLY TO QUESTOBJECT SCRIPT
    public override void OnInspectorGUI()
    {
        
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("optionSource"));
        Generator.OptionSource optionSource = (Generator.OptionSource)serializedObject.FindProperty("optionSource").enumValueIndex;

        switch (optionSource)
        {
            case Generator.OptionSource.External:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("optionList"));
                break;

            case Generator.OptionSource.Mixed:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("optionList"));
                break;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("randomizeOnStart"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("segments"), true);

        SerializedProperty sp = serializedObject.FindProperty("segments");
        for (int x = 0; x < sp.arraySize; x++)
        {

            SerializedProperty subObject = sp.GetArrayElementAtIndex(x);

            Generator.SegmentSeparator segmentSeparator = (Generator.SegmentSeparator)subObject.FindPropertyRelative("useSeparator").enumValueIndex;

            switch (segmentSeparator)
            {

                case Generator.SegmentSeparator.Custom:
                    EditorGUILayout.PropertyField(subObject.FindPropertyRelative("separator"));
                    break;
            }
        }

        serializedObject.ApplyModifiedProperties();
        
    }*/
}
