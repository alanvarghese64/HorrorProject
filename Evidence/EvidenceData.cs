using UnityEngine;

[CreateAssetMenu(fileName = "NewEvidence", menuName = "HorrorProject/Evidence Data")]
public class EvidenceData : ScriptableObject
{
    public string evidenceName;
    public EvidenceType type;
    [Range(0, 50)] public float convictionPower;
    public Sprite evidenceIcon;
}
