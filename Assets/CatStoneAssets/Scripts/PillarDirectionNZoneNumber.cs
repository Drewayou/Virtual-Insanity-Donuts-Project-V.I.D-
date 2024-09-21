using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PillarDirectionNZoneNumber : MonoBehaviour
{
    //Set what pillar this text GUI should show if needed.
    [Serializable] 
    public enum SelectedOrdinate
    {North, South, East, West, NoOrdinate}
    public SelectedOrdinate selectedOrdinateSet;

    //Saves what ordinate to show if there is one.
    String ordinateToShow;

    //Zone number input field pulled from the game manager.
    int zoneNumber;

    //Saves what text this GUI should show.
    String textToShow;

    //Start is called before the first frame update
    void Start()
    {
        //Set the zone and ordinates on the GUI when scene is started.
        zoneNumber = GameObject.Find("GameManagerObject").GetComponent<GameManagerScript>().GetZoneLevel();
        SetOrdinateSelected();
        textToShow = ordinateToShow + zoneNumber;
        gameObject.GetComponent<TMP_Text>().text = textToShow;
    }

    //Update is called once per frame.
    void Update()
    {
        //If the zone level does not match to the game manager, update it.
        if(zoneNumber != GameObject.Find("GameManagerObject").GetComponent<GameManagerScript>().GetZoneLevel()){
            UpdateZoneNOrdinateTextGUIS();
        }
    }

    //Public method that other scripts may use to update this text GUI.
    public void UpdateZoneNOrdinateTextGUIS(){
        zoneNumber = GameObject.Find("GameManagerObject").GetComponent<GameManagerScript>().GetZoneLevel();
        textToShow = ordinateToShow + zoneNumber;
        gameObject.GetComponent<TMP_Text>().text = textToShow;
    }

    public void SetOrdinateSelected(){
        switch(selectedOrdinateSet){
            case SelectedOrdinate.North:
                ordinateToShow = "N";
            break;

            case SelectedOrdinate.South:
                ordinateToShow = "S";
            break;

            case SelectedOrdinate.East:
                ordinateToShow = "E";
            break;

            case SelectedOrdinate.West:
                ordinateToShow = "W";
            break;
        }
        
    }
}
