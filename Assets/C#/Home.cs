using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{

    private List<GameObject> units;


    // Start is called before the first frame update
    void Start()
    {
        units = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        UnitCheck();
    }

    private void UnitCheck()
    {
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) 
        {       
                units.Add(unit);           
        }

        ResourceCheck();

        //clear list
        if (units.Count >= GameObject.FindGameObjectsWithTag("Unit").Length)
        {
            units.Clear();
        }
    }

    private void ResourceCheck()
    {
        foreach(GameObject unit in units)
        {
            float dist = Vector3.Distance(unit.transform.position, transform.position);
            if(dist <= 0.75)
            {
                unit.GetComponent<Unit>().hasResource = false;
            }
        }
    }
}
