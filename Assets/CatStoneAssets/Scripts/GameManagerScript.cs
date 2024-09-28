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

    //The "PlayableArea" gameobject where the player and ememies run around. Automatically pulled in game.
    private GameObject PlayableAreaGameObject;

    //Grab the possible Level Design prefabs here.
    [SerializeField]
    [Tooltip("Drag / add the possible Level Design prefabs here.")]
    public List<GameObject> LevelDesignsList;

    //The Level design loader object. Automatically pulled in game via "PlayableAreaGameObject".
    [Tooltip("Automatically pulled in game via \"PlayableAreaGameObject\".")]
    private GameObject LevelDesignLoaderObject;

    //Put the possible monsters that can spawn.
    //Input the possible monster prefabs here.
    [SerializeField]
    [Tooltip("Drag the coresponding possible path prefabs here.")]
    GameObject blackSmogMonsterPath, hippoThumperPath, jamiroquaiGraberPath, strayAgressiveDogPath, safePath;

    //The path anchor gameobjects for the in-game area to generate at, using the transform properties of these parent objects. Automatically gets pulled in game.
    private GameObject northPathAnchor, southPathAnchor, eastPathAnchor, westPathAnchor;

    //The MainRoomBuilding object that hosts the ordinal anchors and level design. Automatically gets pulled in game.
    private GameObject MainRoomBuilding;

    //The possible paths saved per round.
    private List<GameObject> pathArrayList;

    //Grab the player Object in here to interact with the player's scripts.
    [SerializeField]
    [Tooltip("Drag the player object here.")]
    GameObject playerObject;

    //Gets the player controller script to mmodify the player controllers during specific moments. Automatically set up when Start() runs.
    private PlayerControllerScript playerController;

    //The timer for this round.
    public float roundTimer;

    //The bool to check if the round has started after loading the new rounde objects.
    public bool roundHasStarted = false;

    //The overall timer for this run session (Not including round load times or main menu times).
    public float overallRunTimer;

    //Get the "PlayerNewZoneGUICanvas" that pops up if the player sucessfully goes to a new zone.
    [SerializeField]
    [Tooltip("Drag the \"PlayerNewZoneGUICanvas\" game object here.")]
    GameObject playerNewZoneGUICanvas;

    // Start is called before the first frame update
    void Start()
    {
        //Grabs the player Controller script for other methods below.
        playerController = playerObject.GetComponent<PlayerControllerScript>();

        //Grabs the play area.
        PlayableAreaGameObject = GameObject.Find("PlayableArea");

        //Grabs the MainRoomBuilding to load the level design and spawn monsters via Path Anchors.
        MainRoomBuilding = PlayableAreaGameObject.transform.Find("MainRoomBuilding").gameObject;
        northPathAnchor = MainRoomBuilding.transform.Find("NorthPathAnchor").gameObject;
        southPathAnchor = MainRoomBuilding.transform.Find("SouthPathAnchor").gameObject;
        eastPathAnchor = MainRoomBuilding.transform.Find("EastPathAnchor").gameObject;
        westPathAnchor = MainRoomBuilding.transform.Find("WestPathAnchor").gameObject;

        //Grabs the level holder object to load the level designs.
        LevelDesignLoaderObject = MainRoomBuilding.transform.Find("LevelDesignLoader").gameObject;

        //FIXME: Show the initial zone loading GUI. May want to change this down the line.
        StartCoroutine(LerpNewZoneNoficicationAndLoading());
    }

    // Update is called once per frame
    void Update()
    {
        //If the new round canva is still active, slowly let it vanish and give the player back control.
        if(playerNewZoneGUICanvas.gameObject.activeSelf == true){
            playerNewZoneGUICanvas.GetNamedChild("BackgroundPanel").gameObject.GetNamedChild("RoundLevelText").GetComponent<TMP_Text>().text = "Zone #" + zoneNumber;
        }

        if(roundHasStarted){
            roundTimer += Time.deltaTime;
            overallRunTimer += Time.deltaTime;
        }
    }

    //--------------------------------------------------------------------------------------
    //Game methods that other scripts may call as triggers.

    public void PlayerGoesToNextZone(){
        roundHasStarted = false;
        StartCoroutine(LerpNewZoneNoficicationAndLoading());
        LoadNewZone();
    }

    public void PlayeLosesGame(){

    }

    public void PlayerPausesGame(){

    }

    public void PlayerGoesToMainMenu(){

    }

    //Below is a LERP function to allow the player to have a GUI pop up if they make it to a new zone.
    //Example of LERP documentation can be found here: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#how_to_use_lerp_in_unity
    public IEnumerator LerpNewZoneNoficicationAndLoading(){
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

        roundHasStarted = true;
    }

    //A method for loading specific monster prefabs depending on level.
    public void LoadMonsterRandomization(){

        //Temp gameobject variables to save which monster paths to generate for this zone.
        GameObject monsterPath1;
        GameObject monsterPath2;
        GameObject monsterPath3;
        GameObject safePath4;

        //FIXME: Use this switch statement to adjust how different zones levels may load via level design.
        switch(zoneNumber){
            case 25:

            break;

            //Set basic possible paths in zone via saving it to the pathArrayList.
            default:
                monsterPath1 = blackSmogMonsterPath;
                monsterPath2 = hippoThumperPath;
                monsterPath3 = jamiroquaiGraberPath;
                safePath4 = safePath;
                pathArrayList = new List<GameObject>{monsterPath1, monsterPath2, monsterPath3, safePath4};
            break;
        }

        //FIXME: Randomize & instantiate the north path, and remove it from possible path list.
        int path1Selected = Random.Range(0,4);
        Instantiate(pathArrayList[path1Selected], northPathAnchor.transform);
        pathArrayList.RemoveAt(path1Selected);

        //Randomize & instantiate the south path, and remove it from possible path list.
        int path2Selected = Random.Range(0,3);
        Instantiate(pathArrayList[path2Selected], southPathAnchor.transform);
        pathArrayList.RemoveAt(path2Selected);

        //Randomize & instantiate the east path, and remove it from possible path list.
        int path3Selected = Random.Range(0,2);
        Instantiate(pathArrayList[path3Selected], eastPathAnchor.transform);
        pathArrayList.RemoveAt(path3Selected);

        //Set & instantiate the west path as whatever was left and make possible path list empty.
        Instantiate(pathArrayList[0], westPathAnchor.transform);
        pathArrayList.RemoveAt(0);
    }

    //FIXME: A method for loading specific Zone prefabs depending on level.
    public void LoadRoomLevelDesign(){

        //Make sure the LevelDesignLoader object doesn't have any level loaded.
        if(LevelDesignLoaderObject.transform.childCount == 0){
            Debug.Log("Loading Level");
        }else{
            Destroy(LevelDesignLoaderObject.transform.GetChild(0));
        }
        //Use this switch statement to adjust how different zones levels may load via level design.
        switch(zoneNumber){
            case 25:

            break;

            case 2:
            Instantiate(LevelDesignsList[1],LevelDesignLoaderObject.transform);
            break;

            //Load basic zone into the LevelDesignLoaderObject.
            default:
                Instantiate(LevelDesignsList[0],LevelDesignLoaderObject.transform);
            break;
        }
    }

    //---------------------------------------------------------------------------------------
    //Loads the new zone for the player.
    public void LoadNewZone(){
        LoadRoomLevelDesign();
        LoadMonsterRandomization();
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
