using UnityEngine;

[CreateAssetMenu(fileName = "UnitCardData", menuName = "Custom/Create UnitCardData")]
public class UnitCardData : ScriptableObject
{
    public UnitListEnums.UnitType unitType;
    public int unitTypeCount;
    public Sprite unityTypeImg;
}