using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DialogueController))]
public class DialogueControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueController controller = (DialogueController)target;

        // Draw the default inspector for everything except the dialogues list
        serializedObject.Update();
        
        DrawPropertiesExcluding(serializedObject, "dialogues");

        // Custom drawer for the Dialogues List
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dialogue Events Map", EditorStyles.boldLabel);
        
        SerializedProperty dialoguesProp = serializedObject.FindProperty("dialogues");

        if (controller.validEvents == null)
        {
            EditorGUILayout.HelpBox("Assign a GameEventSO to 'Valid Events' to select events from a dropdown.", MessageType.Warning);
            EditorGUILayout.PropertyField(dialoguesProp, true); // Fallback to standard list
        }
        else
        {
            // Iterate through list
            for (int i = 0; i < dialoguesProp.arraySize; i++)
            {
                SerializedProperty item = dialoguesProp.GetArrayElementAtIndex(i);
                SerializedProperty eventProp = item.FindPropertyRelative("requiredEvent");
                SerializedProperty isDefaultProp = item.FindPropertyRelative("isDefaultDialogue");
                SerializedProperty linesProp = item.FindPropertyRelative("dialogueLines");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Header
                EditorGUILayout.LabelField($"Dialogue Set {i + 1}", EditorStyles.boldLabel);

                // Default Toggle
                EditorGUILayout.PropertyField(isDefaultProp);

                if (!isDefaultProp.boolValue)
                {
                    // DROPDOWN LOGIC
                    int currentIndex = controller.validEvents.eventNames.IndexOf(eventProp.stringValue);
                    if (currentIndex == -1) currentIndex = 0;

                    // Convert list to array for Popup
                    string[] options = controller.validEvents.eventNames.ToArray();
                    
                    if (options.Length > 0)
                    {
                        int newIndex = EditorGUILayout.Popup("Required Event", currentIndex, options);
                        eventProp.stringValue = options[newIndex];
                    }
                    else
                    {
                         EditorGUILayout.LabelField("No events found in GameEventSO!");
                    }
                }

                // Dialogue Lines
                EditorGUILayout.PropertyField(linesProp, true);

                // Remove Button
                if (GUILayout.Button("Remove Dialogue Set"))
                {
                    dialoguesProp.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add New Dialogue Set"))
            {
                dialoguesProp.InsertArrayElementAtIndex(dialoguesProp.arraySize);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
