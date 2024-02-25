using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    private GameMaster gameMaster;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private List<GameObject> units;

    public List<Transform> spawnPoints;

    [SerializeField] private float resourceCheckRange;
    [SerializeField] private float transparencyRange;
    [SerializeField] private float transparency;


    // Start is called before the first frame update
    void Start()
    {
        units = new List<GameObject>();
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();

        originalColor = spriteRenderer.color;

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

        //all monitoring scripting here
        ResourceCheck();
        BecomeTransparent();

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
            if(dist <= resourceCheckRange)
            {
                if(unit.GetComponent<Unit>().hasResource)
                {
                    unit.GetComponent<Unit>().hasResource = false;
                    gameMaster.gameMasterData.lumberCount++;
                }           
            }
        }
    }

    private void BecomeTransparent()
    {
        foreach (GameObject unit in units)
        {
            float dist = Vector3.Distance(unit.transform.position, transform.position);
            if (dist <= transparencyRange)
            {
                if (spriteRenderer != null)
                {
                    Color color = spriteRenderer.color;

                    // Set the alpha value of the color
                    color.a = transparency;

                    // Assign the modified color back to the SpriteRenderer
                    spriteRenderer.color = color;
                }
            }
            else
            {
                // Assign the modified color back to the SpriteRenderer
                spriteRenderer.color = originalColor;
            }
        } 
    }

    public void SpawnWorm()
    {
        int selectSpawn = Random.Range(0, spawnPoints.Count);
        Debug.Log("spawned at: " + spawnPoints[selectSpawn].name);

        //Instantiate object here
    }

}
