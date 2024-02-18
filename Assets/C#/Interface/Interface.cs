using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnitListEnums;

public class Interface : MonoBehaviour
{
    private GameMaster gameMaster;

    [SerializeField] private GameObject unitPanel;
    [SerializeField] private GameObject homePanel;

    private bool homeSelected;

    //Display
    public Button clearUnits;
    public TMP_Text lumberText;

    private int lumberCount;

    public List<GameObject> unitCards;

    //Controls
    public Button clearUnitBtn;

    //Data
    [SerializeField] private UnitCardData homeData;
    [SerializeField] private UnitCardData ghoulData;

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

        ManageHome();


        UnitCards();

    }

    private void UnitCards()
    {
        //Assign unit cards ---
        foreach (GameObject unitCard in unitCards)
        {
            if (unitCard.GetComponent<UnitCard>().unitData != null)
            {
                unitCard.SetActive(true);
            }
            else
            {
                unitCard.SetActive(false);
            }
        }

        if (gameMaster.selectedUnitList.Count <= 0)
        {
            foreach (GameObject unitCard in unitCards)
            {
                unitCard.SetActive(false);
                clearUnits.gameObject.SetActive(false);
            }
        }
        else
        {
            clearUnits.gameObject.SetActive(true);
        }

        foreach (Unit u in gameMaster.selectedUnitList)
        {
            switch (u.unitType)
            {
                case UnitType.Ghoul: //Ghoul

                    if (unitCards.Exists(card => card.GetComponent<UnitCard>().unitTitle.text == UnitType.Ghoul.ToString()))
                    {
                        //do count logic here
                        ghoulData.unitTypeCount = gameMaster.selectedUnitList.FindAll(unit => unit.GetComponent<Unit>().unitType.ToString() == UnitType.Ghoul.ToString()).Count;


                        return;

                    }
                    else
                    {
                        foreach (GameObject card in unitCards)
                        {
                            UnitCard unitCard = card.GetComponent<UnitCard>();
                            if (unitCards.FindAll(card => card.GetComponent<UnitCard>().unitData != null && card.GetComponent<UnitCard>().unitTitle.text == UnitType.Ghoul.ToString()).Count < 1)
                            {
                                Debug.Log("Assigned card");
                                unitCard.unitData = ghoulData;
                                return;
                            }
                        }

                    }
                    break;
                case UnitType.Home: //Home
                    if (unitCards.Exists(card => card.GetComponent<UnitCard>().unitTitle.text == UnitType.Home.ToString()))
                    {
                        //do count logic here
                        homeData.unitTypeCount = gameMaster.selectedUnitList.FindAll(unit => unit.GetComponent<Unit>().unitType.ToString() == UnitType.Home.ToString()).Count;


                        return;

                    }
                    else
                    {
                        foreach (GameObject card in unitCards)
                        {
                            UnitCard unitCard = card.GetComponent<UnitCard>();
                            Debug.Log("Assigned card");
                            if (unitCards.FindAll(card => card.GetComponent<UnitCard>().unitData != null && card.GetComponent<UnitCard>().unitTitle.text == UnitType.Home.ToString()).Count < 1)
                            {
                                Debug.Log("Assigned card");
                                unitCard.unitData = homeData;
                                return;
                            }
                        }

                    }
                    break;
            }
        }
        //---

        clearUnitBtn.onClick.AddListener(ClearUnitCards);
    }

    private void ManageHome()
    {
        if (gameMaster && gameMaster.target && gameMaster.target.name == "Home") //Home - priority 1
        {
            Debug.Log("home selected");

            gameMaster.selectedUnitList.Clear();

            homePanel.SetActive(true);
        }
        else
        {
            Debug.Log("home deselected");
            homeSelected = false;
            homePanel.SetActive(false);
        }
    }

    public void ClearUnitCards()
    {
        Debug.Log("Cleared");
        gameMaster.selectedUnitList.Clear();
    }



    void FixedUpdate()
    {

      

    }
}
