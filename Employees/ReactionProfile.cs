using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HorrorProject/Reaction Profile")]
public class ReactionProfile : ScriptableObject
{
    [Header("Animation Triggers")]
    public string idleAnimation = "Idle";
    public string nervousAnimation = "Nervous_Idle";
    public string panicAnimation = "Panic_Run";
    
    [Header("Thresholds")]
    public float nervousThreshold = 30f;
    public float panicThreshold = 80f;

    // You can expand this to include lists of sounds, or specific tasks they refuse to do
}
