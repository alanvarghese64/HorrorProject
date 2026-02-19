using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameEventList", menuName = "Horror/Game Event List")]
public class GameEventSO : ScriptableObject
{
    [Header("Define All Game Events Here")]
    [Tooltip("List all event names here. These will appear in dropdowns.")]
    public List<string> eventNames = new List<string>();
}
