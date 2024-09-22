using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    //Sets the zone number.
    public int zoneNumber = 0;

    //Sets the player's current sanity meter;
    public float sanityMeter = 100;

    //Sets if the player has died.
    public bool playerIsAlive;

    //NewRoundTimeout is how long the player can't move as they learn what zone they're in and the GUI pops up taking up their screen.
    public float newRoundTimeout = 3.5f;

    //Put the possible monsters that can spawn.
    //Input the possible monster prefabs here.
    [SerializeField]
    [Tooltip("Drag the possible path prefabs here.")]
    GameObject blackSmogMonsterPath, hippoThumperPath, monster3Path, monster4Path, monster5Path, safePath;

    //Grab the player Object in here to interact with the player's scripts.
    [SerializeField]
    [Tooltip("Drag the player object here.")]
    GameObject playerObject;

    //Gets the player controller script to mmodify the player controllers during specific moments. Automatically set up when Start() runs.
    private PlayerControllerScript playerController;


    //Get the "PlayerNewZoneGUICanvas" that pops up if the player sucessfully goes to a new zone.
    [SerializeField]
    [Tooltip("Drag the \"PlayerNewZoneGUICanvas\" game object here.")]
    GameObject playerNewZoneGUICanvas;

    // Start is called before the first frame update
    void Start()
    {
        //Grabs the player Controller script for other methods below.
        playerController = playerObject.GetComponent<PlayerControllerScript>();

        //FIXME: Show the initial zone loading GUI. May want to change this down the line.
        StartCoroutine(LerpNewZoneNotificationGUI());
    }

    // Update is called once per frame
    void Update()
    {
        //If the new round canva is still active, slowly let it vanish and give the player back control.
        if(playerNewZoneGUICanvas.gameObject.activeSelf == true){
            playerNewZoneGUICanvas.GetNamedChild("BackgroundPanel").gameObject.GetNamedChild("RoundLevelText").GetComponent<TMP_Text>().text = "Zone #" + zoneNumber;
        }
    }

    //--------------------------------------------------------------------------------------
    //Game methods that other scripts may call as triggers.

    public void PlayerGoesToNextZone(){
        StartCoroutine(LerpNewZoneNotificationGUI());
    }

    public void PlayeLosesGame(){

    }

    public void PlayerPausesGame(){

    }

    public void PlayerGoesToMainMenu(){

    }

    //Below is a LERP function to allow the player to have a GUI pop up if they make it to a new zone.
    //Example of LERP documentation can be found here: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#how_to_use_lerp_in_unity
    public IEnumerator LerpNewZoneNotificationGUI(){
        float timeElapsed = 0;
        float alphaLevelOfGui;
        Debug.Log(timeElapsed);

        //Declare the GUI objects to be changed.
        Image panelImg = playerNewZoneGUICanvas.transform.GetChild(0).GetComponent<Image>();
        TMP_Text textTMP = playerNewZoneGUICanvas.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();

        //Pause player movement for this duration of timeout.
        playerController.PausePlayerMovement();

        while (timeElapsed < newRoundTimeout)
        {
            alphaLevelOfGui = Mathf.Lerp(1, 0, timeElapsed / newRoundTimeout);
            Debug.Log("AlphaValueOfGui: " + alphaLevelOfGui);

            //Sets the GUI image & text alpha to the lerp colors.
            var alphaOfPanelLerp = panelImg.color;
            var alphaOfTextLerp = textTMP.color;
            alphaOfPanelLerp.a = alphaLevelOfGui;
            alphaOfTextLerp.a = alphaLevelOfGui;
            panelImg.color = alphaOfPanelLerp;
            textTMP.color = alphaOfTextLerp;

            timeElapsed += Time.deltaTime;

            Debug.Log("Time until Zone fades: " + newRoundTimeout);

            yield return null;
        }

        //Resume player movement after this duration of timeout.
        playerController.ResumePlayerNormalMovement();

        //Resets the alpha and color of both GUI elements for the next time.
        panelImg.color = Color.black;
        textTMP.color = Color.white;

        var alphaOfPanel = panelImg.color;
        var alphaOfText = textTMP.color;
        alphaOfPanel.a = 1;
        alphaOfText.a = 1;

        panelImg.color = alphaOfPanel;
        textTMP.color = alphaOfText;

        playerNewZoneGUICanvas.SetActive(false);
    }

    //---------------------------------------------------------------------------------------
    //Getters & Setters for this script.

    //Get the zone level.
    public int GetZoneLevel(){
        return zoneNumber;
    }

    //Set the zone level.
    public void SetZoneLevel(int zoneNewNumber){
        zoneNumber = zoneNewNumber;
    }

    //Get the sanity meter.
    public float GetSanityMeterValue(){
        return sanityMeter;
    }

    //Set the sanity meter.
    public void SetSanityMeterValue(int newSanityMeterInput){
        zoneNumber = newSanityMeterInput;
    }

    //Set the flashlight battery left in seconds.
    public void SetFlashLightBattery(float newFlashlightTimeLeft){
        playerObject.GetComponent<FlashLightScript>().FlashLightSetTimeLeft(newFlashlightTimeLeft);
    }

    public float GetNewRoundTimer(){
        return newRoundTimeout;
    }

    public GameObject GetPlayerNewZonePopupGameObject(){
        return playerNewZoneGUICanvas;
    }

}
