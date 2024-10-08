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

    //The Timer of how often a sound or monster spawn occures after n seconds.
    public float attemptTriggerSpawnerOrAudioTimer = 5.0f;

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

    //Grab playable area object.
    [SerializeField]
    [Tooltip("Drag the Playable Area object here.")]
    GameObject playableAreaGameObject;

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

        //Grabs the MainRoomBuilding to load the level design and spawn monsters via Path Anchors.
        MainRoomBuilding = playableAreaGameObject.transform.Find("MainRoomBuilding").gameObject;
        northPathAnchor = MainRoomBuilding.transform.Find("NorthPathAnchor").gameObject;
        southPathAnchor = MainRoomBuilding.transform.Find("SouthPathAnchor").gameObject;
        eastPathAnchor = MainRoomBuilding.transform.Find("EastPathAnchor").gameObject;
        westPathAnchor = MainRoomBuilding.transform.Find("WestPathAnchor").gameObject;

        //Grabs the level holder object to load the level designs.
        LevelDesignLoaderObject = MainRoomBuilding.transform.Find("LevelDesignLoader").gameObject;

        //FIXME: Show the initial zone loading GUI. May want to change this down the line.
        if(playerController != null){
            StartCoroutine(LerpNewZoneNoficicationAndLoading());
        }
        LoadNewZone();
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

    //FIXME: This will trigger in the update() frame IF the timer for the last attempt hits 0.
    //If the timer to attempt a trigger hits zero, attempt to trigger a path to make an audio queue.
    public void TriggerPathAudioOrSpawnAMonster(){
        if(attemptTriggerSpawnerOrAudioTimer <= 0){
            
            //Pick a random path (North-South-East-West) to attempt to trigger.
            int pathToTriggerSelected = Random.Range(1,5);
            switch(pathToTriggerSelected){
                case 1:
                northPathAnchor.transform.GetChild(0).GetComponent<PathTriggerScript>().AttemptTrigger();
                break;
                case 2:
                southPathAnchor.transform.GetChild(0);
                break;
                case 3:
                eastPathAnchor.transform.GetChild(0);
                break;
                case 4:
                westPathAnchor.transform.GetChild(0);
                break;
            }
        }
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

        //Declare the GUI objects to be changed.
        Image panelImg = playerNewZoneGUICanvas.transform.GetChild(0).GetComponent<Image>();
        TMP_Text textTMP = playerNewZoneGUICanvas.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();

        //Pause player movement for this duration of timeout.
        if(playerController.playerMoveSpeed > 0){
            playerController.playerMoveSpeed = 0;
        }

        while (timeElapsed < newRoundTimeout)
        {
            alphaLevelOfGui = Mathf.Lerp(1, 0, timeElapsed / newRoundTimeout);
            //Debug.Log("AlphaValueOfGui: " + alphaLevelOfGui);

            //Sets the GUI image & text alpha to the lerp colors.
            var alphaOfPanelLerp = panelImg.color;
            var alphaOfTextLerp = textTMP.color;
            alphaOfPanelLerp.a = alphaLevelOfGui;
            alphaOfTextLerp.a = alphaLevelOfGui;
            panelImg.color = alphaOfPanelLerp;
            textTMP.color = alphaOfTextLerp;

            timeElapsed += Time.deltaTime;

            //Debug.Log("Time until Zone fades: " + newRoundTimeout);

            yield return null;
        }

        //Resume player movement after this duration of timeout.
        playerController.playerMoveSpeed = 2;

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

        //Make sure the level doesn't have any paths loaded.
        if(northPathAnchor.transform.childCount != 0){
            foreach(Transform stuffInNorthPathAnchor in northPathAnchor.transform){
                //Debug.Log("Removing Previously Loaded North Path...");
                Destroy(stuffInNorthPathAnchor.gameObject);
            }    
        }
        if(southPathAnchor.transform.childCount != 0){
            foreach(Transform stuffInSouthPathAnchor in southPathAnchor.transform){
                //Debug.Log("Removing Previously Loaded South Path...");
                Destroy(stuffInSouthPathAnchor.gameObject);
            }    
        }
        if(eastPathAnchor.transform.childCount != 0){
            foreach(Transform stuffInEastPathAnchor in eastPathAnchor.transform){
                //Debug.Log("Removing Previously Loaded East Path...");
                Destroy(stuffInEastPathAnchor.gameObject);
            }    
        }
        if(westPathAnchor.transform.childCount != 0){
            foreach(Transform stuffInNWesthPathAnchor in westPathAnchor.transform){
                //Debug.Log("Removing Previously Loaded West Path...");
                Destroy(stuffInNWesthPathAnchor.gameObject);
            }    
        }

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
        GameObject northPath = Instantiate(pathArrayList[path1Selected], northPathAnchor.transform);
        pathArrayList.RemoveAt(path1Selected);

        //Randomize & instantiate the south path, and remove it from possible path list.
        int path2Selected = Random.Range(0,3);
        GameObject southPath = Instantiate(pathArrayList[path2Selected], southPathAnchor.transform);
        southPath.transform.rotation = Quaternion.Euler(0, 180, 0);
        pathArrayList.RemoveAt(path2Selected);

        //Randomize & instantiate the east path, and remove it from possible path list.
        int path3Selected = Random.Range(0,2);
        GameObject eastPath = Instantiate(pathArrayList[path3Selected], eastPathAnchor.transform);
        eastPath.transform.rotation = Quaternion.Euler(0, 90, 0);
        pathArrayList.RemoveAt(path3Selected);

        //Set & instantiate the west path as whatever was left and make possible path list empty.
        GameObject westPath = Instantiate(pathArrayList[0], westPathAnchor.transform);
        westPath.transform.rotation = Quaternion.Euler(0, 270, 0);
        pathArrayList.RemoveAt(0);
    }

    //FIXME: A method for loading specific Zone prefabs depending on level.
    public void LoadRoomLevelDesign(){

        //Make sure the LevelDesignLoader object doesn't have any level objects loaded.
        if(LevelDesignLoaderObject.transform.childCount == 0){
            Debug.Log("Loading Zone Level " + zoneNumber + "...");
        }else{
            foreach(Transform stuffInSceneLoader in LevelDesignLoaderObject.transform){
                Debug.Log("Removing Previously Loaded Level...");
                Destroy(stuffInSceneLoader.gameObject);
            }
        }
        //Use this switch statement to adjust how different zones levels may load via level design.
        switch(zoneNumber){
            case 25:

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
