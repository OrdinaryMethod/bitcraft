using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interface : MonoBehaviour
{
    private GameMaster gameMaster;
    public TMP_Text lumberText;

    private int lumberCount;

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        lumberCount =  gameMaster.gameMasterData.lumberCount;
        lumberText.text = lumberCount.ToString();
    }
}
