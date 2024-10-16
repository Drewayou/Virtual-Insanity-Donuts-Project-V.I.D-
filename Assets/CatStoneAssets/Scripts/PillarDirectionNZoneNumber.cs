using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PillarDirectionNZoneNumber : MonoBehaviour
{
    //Get the gamemanager object.
    GameObject gameManagerInstance;

    //Set what pillar this text GUI should show if needed.
    [Serializable] 
    [Tooltip("Select where this text GUI is to show the player the direction they're facing!")]
    public enum SelectedOrdinate
    {North, South, East, West, NoOrdinate}
    public SelectedOrdinate selectedOrdinateSet;

    //Saves what ordinate to show if there is one.
    String ordinateToShow;

    //Zone number input field pulled from the game manager.
    int zoneNumber = 0;

    //Saves what text this GUI should show.
    String textToShow;

    //sets how long until the text GUI's show up after the round has started.
    [Tooltip("How many seconds until these text GUI's show up. This is automatically set by the GameManager object!")]
    public float newRoundTimeout = 3.5f;

    //sets how long until the text GUI's show up after the round has started.
    [Tooltip("How much longer after the timeout for the text GUI's to show up. Set to 0 to instantly pop up the text GUI when the round starts")]
    public float roundStartsGuiShowsUpTimer = 5f;

    //The player PlayerNewZoneGUICanvas when a player goes to a new zone.
    [SerializeField]
    [Tooltip("Drag the \"PlayerNewZoneGUICanvas\" game object here to know when to turn on/OffMeshLink these textUI.")]
    GameObject PlayerNewZoneGUICanvas;

    //Start is called before the first frame update
    void Start()
    {
        //Get the game manager instance to pull game scripts from.
        gameManagerInstance = GameObject.Find("GameManagerObject");

        //Set the zone and ordinates on the GUI when scene is started.
        zoneNumber = gameManagerInstance.GetComponent<GameManagerScript>().GetZoneLevel();
        SetOrdinateSelected();
        textToShow = ordinateToShow + zoneNumber;
        gameObject.GetComponent<TMP_Text>().text = textToShow;

        //Wait for the first generation of zone upon startup.
        StartCoroutine(LerpNewZoneTextGUISInScene());
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

        //Since this only happens when the player enters a new zone, start this lerp to have the numbers fade and re-appear updated.
        StartCoroutine(LerpNewZoneTextGUISInScene());
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

    //Below is a LERP function to allow the zone text UI prefabs in the scene to hide until the new zone canvas is hidden.
    //Example of LERP documentation can be found here: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#how_to_use_lerp_in_unity
    public IEnumerator LerpNewZoneTextGUISInScene(){
        float timeElapsed = 0;
        float alphaLevelOfGui;
        //Debug.Log(timeElapsed);

        //Declare the GUI objects to be changed which is attached to this game object.
        TMP_Text textTMP = gameObject.GetComponent<TMP_Text>();

        Color alphaOfText = new Color(255, 255, 255, 0);
        textTMP.color = alphaOfText;

        //Sets the timer for this lerp to be x2 the length of the game's new round timeout.
        newRoundTimeout = gameManagerInstance.GetComponent<GameManagerScript>().GetNewRoundTimer() * roundStartsGuiShowsUpTimer;

        //Wait for the round to start.
        yield return new WaitForSeconds(newRoundTimeout);
        
        while (timeElapsed < roundStartsGuiShowsUpTimer)
        {
            alphaLevelOfGui = Mathf.Lerp(0, 1, timeElapsed / roundStartsGuiShowsUpTimer);
            //Debug.Log("AlphaValueOfGui: " + alphaLevelOfGui);

            //Sets the Text alpha to the lerp colors.
            alphaOfText = new Color(255, 255, 255, alphaLevelOfGui);
            textTMP.color = alphaOfText;

            timeElapsed += Time.deltaTime;

            //Debug.Log("Time until Text GUI fully reveals itself: " + roundStartsGuiShowsUpTimer);

            yield return null;
        }

        //Resets the alpha of text elements for the next time.
        textTMP.color = Color.white;
    }
}
