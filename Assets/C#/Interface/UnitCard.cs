using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    public UnitCardData unitData;

    public TMP_Text unitTitle;
    public TMP_Text unitCount;

    private void Update()
    {
        if(unitData != null)
        {
            //Set Card art
            GetComponent<Image>().sprite = unitData.unityTypeImg;
            unitTitle.text = unitData.unitType.ToString();
            unitCount.text = unitData.unitTypeCount.ToString();
        }
    }  
}
