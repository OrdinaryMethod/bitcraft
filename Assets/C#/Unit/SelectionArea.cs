using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionArea : MonoBehaviour
{
    [SerializeField] private GameObject selectedArea;

    public bool isSelected;

    private void Awake()
    {
        isSelected = false;
    }

    private void Update()
    {
        if(isSelected)
        {
            selectedArea.SetActive(true);
            isSelected = false;
        }
        else
        {
            selectedArea.SetActive(false);
        }
    }
}
