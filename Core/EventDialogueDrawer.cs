using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(DialogueController.EventDialogue))]
public class EventDialogueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get the GameEventSO from the DialogueController component
        GameEventSO validEvents = null;
        Object target = property.serializedObject.targetObject;
        if (target is DialogueController dc)
        {
            validEvents = dc.validEvents;
        }

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't indent internal fields
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 2;
        
        Rect requiredEventRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect isDefaultRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
        Rect dialogueLinesRect = new Rect(position.x, position.y + (lineHeight + spacing) * 2, position.width, lineHeight);
        // We need more space for the array, so we'll let Unity draw it normally below

        // 1. Draw Required Event (with Dropdown if possible)
        SerializedProperty eventProp = property.FindPropertyRelative("requiredEvent");

        if (validEvents != null && validEvents.eventNames != null && validEvents.eventNames.Count > 0)
        {
            int index = Mathf.Max(0, validEvents.eventNames.IndexOf(eventProp.stringValue));
            index = EditorGUI.Popup(requiredEventRect, "Required Event", index, validEvents.eventNames.ToArray());
            eventProp.stringValue = validEvents.eventNames[index];
        }
        else
        {
            EditorGUI.PropertyField(requiredEventRect, eventProp, new GUIContent("Required Event"));
        }

        // 2. Draw Is Default
        EditorGUI.PropertyField(isDefaultRect, property.FindPropertyRelative("isDefaultDialogue"));

        // 3. Draw Dialogue Lines (Standard Property Field handles array expansion)
        SerializedProperty linesProp = property.FindPropertyRelative("dialogueLines");
        float linesHeight = EditorGUI.GetPropertyHeight(linesProp, true);
        Rect linesDrawRect = new Rect(position.x, position.y + (lineHeight + spacing) * 2, position.width, linesHeight);
        EditorGUI.PropertyField(linesDrawRect, linesProp, true);

        // 4. Draw Flags To Set On End (List of Dropdowns)
        SerializedProperty flagsProp = property.FindPropertyRelative("flagsToSetOnEnd");
        float flagsHeight = EditorGUI.GetPropertyHeight(flagsProp, true);
        Rect flagsDrawRect = new Rect(position.x, linesDrawRect.y + linesHeight + spacing, position.width, flagsHeight);
        
        // Custom drawing for the list to ensure dropdowns
        EditorGUI.LabelField(new Rect(flagsDrawRect.x, flagsDrawRect.y, flagsDrawRect.width, lineHeight), "Flags To Set On End");
        flagsProp.isExpanded = EditorGUI.Foldout(new Rect(flagsDrawRect.x, flagsDrawRect.y, flagsDrawRect.width, lineHeight), flagsProp.isExpanded, GUIContent.none);

        if (flagsProp.isExpanded)
        {
            EditorGUI.indentLevel++;
            flagsProp.arraySize = EditorGUI.IntField(new Rect(flagsDrawRect.x, flagsDrawRect.y + lineHeight, flagsDrawRect.width, lineHeight), "Size", flagsProp.arraySize);

            for (int i = 0; i < flagsProp.arraySize; i++)
            {
                SerializedProperty flagElement = flagsProp.GetArrayElementAtIndex(i);
                Rect elementRect = new Rect(flagsDrawRect.x, flagsDrawRect.y + (lineHeight + spacing) * (i + 2), flagsDrawRect.width, lineHeight);
                
                if (validEvents != null && validEvents.eventNames != null && validEvents.eventNames.Count > 0)
                {
                    int index = Mathf.Max(0, validEvents.eventNames.IndexOf(flagElement.stringValue));
                    index = EditorGUI.Popup(elementRect, $"Element {i}", index, validEvents.eventNames.ToArray());
                    flagElement.stringValue = validEvents.eventNames[index];
                }
                else
                {
                    EditorGUI.PropertyField(elementRect, flagElement, new GUIContent($"Element {i}"));
                }
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 2;
        
        float height = (lineHeight + spacing) * 2; // Required Event + Is Default

        SerializedProperty linesProp = property.FindPropertyRelative("dialogueLines");
        height += EditorGUI.GetPropertyHeight(linesProp, true) + spacing;

        SerializedProperty flagsProp = property.FindPropertyRelative("flagsToSetOnEnd");
        
        if (flagsProp.isExpanded)
        {
            height += (lineHeight + spacing) * (flagsProp.arraySize + 2); // Header + Size + Elements
        }
        else
        {
            height += lineHeight + spacing; // Just Header
        }

        return height;
    }
}
