using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{

    //Sets if this Game Manager is in the main menu.
    public bool inMainMenu, inGamePlaying, gameIsPaused;
    
    //Sets the zone number.
    public int zoneNumber;

    //Sets the player's current sanity meter.
    public float sanityMeter = 100f;

    /*DIFFICULTY SELECTION: Sets the difficulty of the game. 
    Easy = A path has to be randomly triggered x3 to spawn a monster and 15s of silence per trigger, 
    Normal = Needs to be triggered 2x and 10s per trigger.
    Hard = Needs to be triggered 1x and 5s per trigger.*/
    [Tooltip("Sets the difficulty of the game! Check code for more info.")]
    public enum SelectedLevelDiffculty
    {Easy, Normal, Hard}

    public SelectedLevelDiffculty selectedLevelDiffculty;

    //Bool to allow/prevent player level selection.
    [Tooltip("Useful bool to allow/disallow player prefs from functioning.")]
    public bool pullSettingsFromPlayerPrefs;

    //Sets if the player has died.
    public bool playerIsAlive;

    //Sets if the player was recently hit with sanity loss.
    public bool lostSanityRecently = false;

    //NewRoundTimeout is how long the player can't move as they learn what zone they're in and the GUI pops up taking up their screen.
    public float newRoundTimeout = 3.5f;

    //The Timer maximum of how often a sound or monster spawn occures after n seconds.
    public float howOftenAnAudioOrMonsterSpawns = 15.0f;

    //The hidden timer of how often a sound or monster spawn occures after n seconds.
    private float attemptTriggerSpawnerOrAudioTimer = 5.0f;

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

    //The EnemyHolderObject that holds any and all monsters for easy location tracking and game management.
    //Input the "EnemyHolderObject" that can be found inside the scene's "PlayableArea" object in here.
    [SerializeField]
    [Tooltip("Drag the coresponding \"EnemyHolderObject\" here.")]
    public GameObject EnemyHolderObject;

    //The possible paths saved per round.
    private List<GameObject> pathArrayList;

    //Grab the player Object in here to interact with the player's scripts.
    [SerializeField]
    [Tooltip("Drag the player object here.")]
    public GameObject playerObject;

    //Grab playable area object.
    [SerializeField]
    [Tooltip("Drag the Playable Area object here.")]
    public GameObject playableAreaGameObject;

    //Gets the player controller script to mmodify the player controllers during specific moments. Automatically set up when Start() runs.
    public PlayerControllerScript playerController;

    //The timer for this round.
    public float roundTimer;

    //The timer for how long it takes for the player to loose more sanity.
    public float sanityInvincibility = 0.5f;

    //The bool to check if the round has started after loading the new rounde objects.
    public bool roundHasStarted = false;

    //The overall timer for this run session (Not including round load times or main menu times).
    public float overallRunTimer;

    //Get the "PlayerNewZoneGUICanvas" that pops up if the player sucessfully goes to a new zone.
    [SerializeField]
    [Tooltip("Drag the \"PlayerNewZoneGUICanvas\" game object here.")]
    GameObject playerNewZoneGUICanvas;

    //Get the "PlayerGameItemsUI" that pops up if the player sucessfully goes to a new zone.
    [SerializeField]
    [Tooltip("Drag the \"PlayerGameItemsUI\" game object here.")]
    GameObject playerGameItemsUI;

    //Sets what monster to jumpscare the player next. Mainly used for Game Over Scene, sometimes when player looses sanity.
    public string nextMonsterJumpscareAtPlayer;

    //Grab the possible jumpscares to load when the player collides with a monster.
    [SerializeField]
    [Tooltip("Drag / add the possible mini jumpscares here.")]
    public List<GameObject> monsterJumpScaresList;

    //An audio source object that gets loaded the player's mic clip. Spawns in Start() Method.
    AudioSource playerMicInput;

    //Microphone device selector. Uses Unity's Microphone.devices class list to find the indexed device to use.
    [Tooltip("This index selects what mic input is being used.")]
    public int microphoneInputIndexSelected = 6;

    //FIXME: Make sure the player can't play from the main menu if no microphone was found!
    //A bool to prevent the player from playing if a microphone wasn't found.
    [Tooltip("A bool to prevent the player from playing if a microphone wasn't found.")]
    public bool compatibleMicAvailable = false;

    //A float variable that saves the RAW SIGNAL strength of the audio that the game is picking up.
    public float playerInGameRawMicSignalStrength;

    //A float variable that saves the loudness of the player converted into DB via mic sensitivity input & DB formulas.
    [Tooltip("This shows the loudness of the player's mic via DB.")]
    public float playerInGameDBLoudness;

    //A float variable that saves sensitivity of the mic input. From 0-1f of change, where 1 is max sensitivity.
    [Tooltip("This float modulates the player's microphone sensitivity for game use via a float. [0-100 is actually 0-1f]")]
    public float playerMicSensitivitySetting = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //Attempts to pulls setting values like mic input, game difficulty, ect. from player Prefs.
        if(pullSettingsFromPlayerPrefs){
            GetPlayerPrefs();
        }
            
        if(!inMainMenu && inGamePlaying){
            //Puts the player in zone 0.
            zoneNumber = 0;
        
            //Grabs the player Controller script for other methods below.
            playerController = playerObject.GetComponent<PlayerControllerScript>();

            //Grabs the MainRoomBuilding to load the level design and spawn monsters via Path Anchors.
            MainRoomBuilding = playableAreaGameObject.transform.Find("MainRoomBuilding").gameObject;
            northPathAnchor = MainRoomBuilding.transform.Find("NorthPathAnchor").gameObject;
            southPathAnchor = MainRoomBuilding.transform.Find("SouthPathAnchor").gameObject;
            eastPathAnchor = MainRoomBuilding.transform.Find("EastPathAnchor").gameObject;
            westPathAnchor = MainRoomBuilding.transform.Find("WestPathAnchor").gameObject;

            StartAndScanMicInstance();

            //Grabs the level holder object to load the level designs.
            LevelDesignLoaderObject = MainRoomBuilding.transform.Find("LevelDesignLoader").gameObject;

            //Sets how often monsters spawn.
            SetMonsterSpawnRates();

            //FIXME: Show the initial zone loading GUI. May want to change this down the line.
            if(playerController != null){
                StartCoroutine(LerpNewZoneNoficicationAndLoading(false));
            }
            LoadNewZone();
        }
        
        if(inMainMenu){
            StartAndScanMicInstance();
        }  
    }

    // Update is called once per frame
    void Update()
    {
        if(inMainMenu){
            //Saves the current DB volume of the mic input that the player recieves.
            playerInGameRawMicSignalStrength = GetMicLoudnessAudio(0,playerMicInput.clip);
            playerInGameDBLoudness = modulatePlayerLoudnessViaMicSensitivity(playerInGameRawMicSignalStrength,playerMicSensitivitySetting);
        }

        if(inGamePlaying && !gameIsPaused){
            //If the new round canva is still active, slowly let it vanish and give the player back control.
            if(playerNewZoneGUICanvas.gameObject.activeSelf == true){
                playerNewZoneGUICanvas.GetNamedChild("BackgroundPanel").gameObject.GetNamedChild("RoundLevelText").GetComponent<TMP_Text>().text = "Zone #" + zoneNumber;
            }

            //While round has been started, count increase game timers.
            if(roundHasStarted){
                roundTimer += Time.deltaTime;
                overallRunTimer += Time.deltaTime;
                attemptTriggerSpawnerOrAudioTimer -= Time.deltaTime;
                TriggerPathAudioOrSpawnAMonster();
                //Saves the current DB volume of the mic input that the player recieves.
                playerInGameRawMicSignalStrength = GetMicLoudnessAudio(0,playerMicInput.clip);
                playerInGameDBLoudness = modulatePlayerLoudnessViaMicSensitivity(playerInGameRawMicSignalStrength,playerMicSensitivitySetting);
            }

            //A counter method to kep track of sanity loss to prevent double-triggering.
            SanityLossedBufferCounter();
        }

    }

    //--------------------------------------------------------------------------------------
    //Game methods that other scripts may call as triggers.

    //Method that pulls from player prefs. More use is settings. Game saves should be saved via something else.
    public void GetPlayerPrefs(){
        microphoneInputIndexSelected = PlayerPrefs.GetInt("micInputSelected",0) + 1;
        playerMicSensitivitySetting = PlayerPrefs.GetFloat("micInputMultiplier",0.5f);
        switch(PlayerPrefs.GetString("difficulty", "normal")){
            case "easy":
                selectedLevelDiffculty = SelectedLevelDiffculty.Easy;
            break;
            case "normal":
                selectedLevelDiffculty = SelectedLevelDiffculty.Normal;
            break;
            case "hard":
                selectedLevelDiffculty = SelectedLevelDiffculty.Hard;
            break;
        }
    }

    //Method that gets triggered when the player goes through a safe path via the "PlayerGoesThroughNextZone" script.
    public void PlayerGoesToNextZone(){
        roundHasStarted = false;
        playerNewZoneGUICanvas.SetActive(true);
        StartCoroutine(LerpNewZoneNoficicationAndLoading(false));
        LoadNewZone();
        playerObject.transform.GetChild(0).gameObject.transform.position = new Vector3(0, 0, 0);
    }

    //FIXME: You need to change the UI of the zone to RED and play a sfx that the player lost some sanity!
    //Method triggered by the "PlayerGoesThroughNextZone" script, when the player went through a bad zone.
    //Player also looses sanity for going through the wrong zone.
    public void PlayerReplaysZone(){
        roundHasStarted = false;
        playerNewZoneGUICanvas.SetActive(true);
        PlayerLosesSanity(25f);

        if(!TestGameOver()){
            StartCoroutine(LerpNewZoneNoficicationAndLoading(true));
            LoadNewZone();
            PlayConfusedSfx();
            playerObject.transform.GetChild(0).gameObject.transform.position = new Vector3(0, 0, 0);
        } 
    }

    //Method can triggered by EnemyAI script when the player collides with a monster!
    public void PlayerReplaysZoneDueToMonster(){
        roundHasStarted = false;
        playerNewZoneGUICanvas.SetActive(true);
        PlayerLosesSanity(25f);

        if(!TestGameOver()){
            StartCoroutine(LerpNewZoneNoficicationAndLoading(true));
            LoadNewZone();
            playerObject.transform.GetChild(0).gameObject.transform.position = new Vector3(0, 0, 0);
            PlayMildJumpscare();
        } 
    }

    //FIXME: This will trigger in the update() frame IF the timer for the last attempt hits 0.
    //If the timer to attempt a trigger hits zero, attempt to trigger a path to make an audio queue.
    public void TriggerPathAudioOrSpawnAMonster(){
        if(attemptTriggerSpawnerOrAudioTimer <= 0){
            attemptTriggerSpawnerOrAudioTimer = howOftenAnAudioOrMonsterSpawns;
            //Pick a random path (North-South-East-West) to attempt to trigger.
            int pathToTriggerSelected = Random.Range(1,5);
            switch(pathToTriggerSelected){
                case 1:
                if(northPathAnchor.transform.GetChild(0).GetComponent<PathTriggerMonsterSFXScript>() != null){
                    northPathAnchor.transform.GetChild(0).GetComponent<PathTriggerMonsterSFXScript>().AttemptTrigger();
                }
                break;
                case 2:
                if(southPathAnchor.transform.GetChild(0).GetComponent<PathTriggerMonsterSFXScript>() != null){
                    southPathAnchor.transform.GetChild(0).GetComponent<PathTriggerMonsterSFXScript>().AttemptTrigger();
                }
                break;
                case 3:
                if(eastPathAnchor.transform.GetChild(0).GetComponent<PathTriggerMonsterSFXScript>() != null){
                    eastPathAnchor.transform.GetChild(0).GetComponent<PathTriggerMonsterSFXScript>().AttemptTrigger();
                }
                break;
                case 4:
                if(westPathAnchor.transform.GetChild(0).GetComponent<PathTriggerMonsterSFXScript>() != null){
                    westPathAnchor.transform.GetChild(0).GetComponent<PathTriggerMonsterSFXScript>().AttemptTrigger();
                }
                break;
            }
        }
    }

    //This method finds all connected mics to this device, and relays that info to the debug.
    //NOTE USE THIS FOR THE SETTINGS MENU AS WELL TO DETERMINE WHAT MICROPHONES ARE TO BE USED BY THE PLAYER!
    //Microphone documentation : https://docs.unity3d.com/ScriptReference/Microphone-devices.html
    public void DisplayAndLoadMicInputs(){
        //As long as the microphone list is >0, show the mics available.
        if(Microphone.devices.Length != 0){
            Debug.Log("Microphones Detected! |\n_____Loading Microphones_____|");
            compatibleMicAvailable = true;
            int microphoneIndex = 0;
            foreach (var device in Microphone.devices)
            {
                microphoneIndex +=1;
                Debug.Log("Microphone " + microphoneIndex + " | Is named : " + device);
            }
        }else{
            Debug.Log("No microphones on this device found!");
            compatibleMicAvailable = false;
        }
    }

    //Start Microphone instance **CALLED IN Start()!
    public void StartAndScanMicInstance(){
        //The a method calls and displays all mic inputs to the debug log, and below lines of code starts mic input recording.
        //Microphone documentation : https://docs.unity3d.com/ScriptReference/Microphone.Start.html
        //FIXME: The "DisplayAndLoadMicInputs" method may be useful for a setting menu that allows mic input changes.
        //DisplayAndLoadMicInputs();
        Debug.Log("Microphone " + microphoneInputIndexSelected + " | SELECTED : " + Microphone.devices[microphoneInputIndexSelected-1]);
        //Instantiates a save for the mic audio and sets the microphone selected via the "microphoneInputIndexSelected" int above.
        playerMicInput = GetComponent<AudioSource>();
        //Starts recording audio via the selected microphone index set. Records a clip for 1 seconds at 64KHz and continues to loop recording session.
        playerMicInput.clip = Microphone.Start(Microphone.devices[microphoneInputIndexSelected-1], true, 1, 64);
    }

    //Gets loudness from the audio recorded.
    //Used forums, documentation, and yt vids to help make our own custom mic input reader for this game.
    //Forums help : https://discussions.unity.com/t/how-do-i-get-the-current-volume-level-amplitude-of-playing-audio-not-the-set-volume-but-how-loud-it-is/162556/2
    //YT help : https://www.youtube.com/watch?v=dzD0qP8viLw
    //Documentation : https://docs.unity3d.com/ScriptReference/AudioClip.GetData.html
    public float GetMicLoudnessAudio(int clipPosition, AudioClip clippedAudio){
        //Take 64 samples of the mic input to justify loudness.
        //Note: "SAMPLES" is a term in audio engineering to determine the "Sound WAVES" or "CYCLES" found, and in this case, how many "waves" to "read".
        //More reading about this can be found here : https://majormixing.com/audio-sample-rate-and-bit-depth-complete-guide/#:~:text=For%20a%20digital%20device%2C%20like,the%20wave%20in%20a%20computer.
        int sampleLength = 64;
        //Start the clip loudness at 0.
        float clipLoudness = 0;
        //Have the start position of the clip check minus the most recent check of sample (To prevent reading the same audio sample over and over).
        //Useful code if clip records are > 1s. -> int startPositionOfClipRead = clipPosition - sampleLength;
        //For now, sample offset is put to 0 because we record samples every second.
        clipPosition = 0;
        //Save the data to be obtained in an array of the sample length desired.
        //Documentation on how to get wave data can be found here: https://docs.unity3d.com/ScriptReference/AudioClip.html
        float[] waveData = new float[sampleLength];
        clippedAudio.GetData(waveData,clipPosition);

        //Reads the data that was input into "waveData" via "GetData" method.
        //Basically reads and adds the amplitude of each wave "Cycle" from 0.00-1.00f in terms of wave data if there was sound recorded in the sample.
        foreach (var sample in waveData) {
				clipLoudness += Mathf.Abs(sample);
			}
        //Gets the raw signal strength from the player 
        /************************************** Usefull Mic debug for RAW signal strength.
        //Debug.Log("Mic input from player | Raw signal strength at | " + clipLoudness + " |");
        ***************************************/
        return clipLoudness;
    }

    //Uses the mic sensitivity input by the player to guage a loudness peaking set.
    //Changes the mic loudness into a usable float co-responding to DB (Decible) values. 
    //The formula and definition for DB calculation can be found here : https://www.britannica.com/science/decibel
    //However, the final formula hard coded did rely on trial and error.
    public float modulatePlayerLoudnessViaMicSensitivity(float inputMicAudio, float inputPlayerSensitivitySetting){
        //Calculate the Amplitude ratio with Mic sensitivity. 
        //Since average sensitivity is at .5, multiply it by 2 to have the standard mic input be multiplied by x1.
        //Sensitivity is <1 because DB is calculated Logarithmically by x10, so .10th of a decimal doubles sound volume input!
        //Thus, the mic sensitivity is x10 - or + more sensitive in DB format depending on the decimal set.
        float rawAudioAmplitudeRatio = inputMicAudio * inputPlayerSensitivitySetting * 2;

        //Forums via unity suggests x20 instead of x10 when comparing amplitude, which is what our code does so far.
        //HOWEVER, we do x120 because we're comparing the DB levels to a player shout or scream! :)
        //DB sound chart can be found here : https://www.commodious.co.uk/knowledge-bank/hazards/noise/measuring-levels
        float DecibleVolume = 120 * Mathf.Log10(rawAudioAmplitudeRatio);
        //If the decible volume is less than 0, set it to average urban silence DB which is ~10DB!
        if(DecibleVolume < 0){
            DecibleVolume = Random.Range(5.00f, 10.00f);
        }
        /************************************** Usefull Mic debug for DB calculation. + DB for specific Sounds Debug.

        Debug.Log("DB Audio from player at | " + DecibleVolume + "DB |");

        if(DecibleVolume > 20 && DecibleVolume < 40){
            Debug.Log("Player is Whispering!");
        }
        if(DecibleVolume > 40 && DecibleVolume < 60){
            Debug.Log("Player is talking!");
        }
        if(DecibleVolume > 60 && DecibleVolume < 80){
            Debug.Log("Player is talking too loud!");
        }
        if(DecibleVolume > 80 && DecibleVolume < 120){
            Debug.Log("Player is Yelling!");
        }
        if(DecibleVolume > 120){
            Debug.Log("Player is SCREAMING!");
        }
        ***************************************/
        return DecibleVolume;
    }

    public void PlayeLosesGame(){

    }

    public void PlayerPausesGame(){

    }

    public void PlayerGoesToMainMenu(){

    }

    //Below is a LERP function to allow the player to have a GUI pop up if they make it to a new zone.
    //Example of LERP documentation can be found here: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#how_to_use_lerp_in_unity
    public IEnumerator LerpNewZoneNoficicationAndLoading(bool playerReDoesAZone){
        float timeElapsed = 0;
        float alphaLevelOfGui;

        //Declare the GUI objects to be changed.
        Image panelImg = playerNewZoneGUICanvas.transform.GetChild(0).GetComponent<Image>();
        TMP_Text textTMP = playerNewZoneGUICanvas.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();

        //Pause player movement for this duration of timeout.
        if(playerController.playerMoveSpeed > 0){
            playerController.playerMoveSpeed = 0;
        }

        //If player gets hit by monster or went down the wrong path, change text color!
        if(playerReDoesAZone){
            textTMP.color = Color.red;
        }

        //Disable the in-game UI (Mic input, SanityMeter, etc.).
        playerGameItemsUI.SetActive(false);

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

        //Re-enable the in-game UI (Mic input, SanityMeter, etc.).
        playerGameItemsUI.SetActive(true);

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

        //Make sure that there are no monsters loaded into the room. If there are, delete them!
        if(EnemyHolderObject.transform.childCount > 0){
            foreach(Transform enemyInZone in EnemyHolderObject.transform){
                Destroy(enemyInZone.gameObject);
            }
        }
        Debug.Log("Clearing enemies in Zone for new generation...");

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

            //Randomly choose either a hippo or dog monster.
                int dogOrHippo = Random.Range(0,2);
                if(dogOrHippo == 1){
                    monsterPath1 = hippoThumperPath;
                }else{
                    monsterPath1 = strayAgressiveDogPath;
                }
            
            //Load in the other basic monsters.
                monsterPath2 = blackSmogMonsterPath;
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

        /*
        //FIXME: Optional switch statement to load specific levels!
        //Use this switch statement to adjust how different zones levels may load via level design.
        switch(zoneNumber){
            case 25:

            break;

            //Load basic zone into the LevelDesignLoaderObject.
            default:
                Instantiate(LevelDesignsList[0],LevelDesignLoaderObject.transform);
            break;
        }
        */

        //Linearly generate the level according to the game manager level designs list filled!
        try{
            Instantiate(LevelDesignsList[zoneNumber],LevelDesignLoaderObject.transform);
        }catch{
            //Output that that level isn't made yet. Rather, load the default 0th level.
            Debug.Log("No Level design made yet for that level! Loading \"Default\" 0th Level design...");
            Instantiate(LevelDesignsList[0],LevelDesignLoaderObject.transform);
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

    //Gets the current battery of the player from the flashlight script attached to them.
    public float GetPlayerFlashlightBatteryHealth(){
        return playerObject.GetComponent<FlashLightScript>().flashlightBatteryTimeLeft;
    }

    //Gets the MAXIMUM battery of the player from the flashlight script attached to them.
    public float GetPlayerFlashlightBatteryHealtMAXIMUM(){
        return playerObject.GetComponent<FlashLightScript>().flashlightFullyChargedBatteryTime;
    }

    public float GetNewRoundTimer(){
        return newRoundTimeout;
    }

    public GameObject GetPlayerNewZonePopupGameObject(){
        return playerNewZoneGUICanvas;
    }

    //How the player looses sanity in the game, triggered by other scripts tied to monsters, or going down the wrong path.
    public void PlayerLosesSanity(float sanityLost){
        if(!lostSanityRecently){
            sanityMeter -= sanityLost;
            lostSanityRecently = true;
        }
    }

    //Makes sure there's no double triggering of sanity meter within the same frame by having a buffer of half a second.
    public void SanityLossedBufferCounter(){
        if(lostSanityRecently){
            sanityInvincibility -= Time.deltaTime;
        }
        if(sanityInvincibility <=0){
            lostSanityRecently = false;
            sanityInvincibility = 0.5f;
        }
    }

    public void SetMonsterSpawnRates(){
        switch(selectedLevelDiffculty.ToString()){
            case "Easy":
                howOftenAnAudioOrMonsterSpawns = 15.0f;
                Debug.Log("Difficulty is set to | EASY | x3 trigger before spawns. | 15s wait per trigger.");
            break;
            case "Normal":
                howOftenAnAudioOrMonsterSpawns = 10.0f;
                Debug.Log("Difficulty is set to | NORMAL | x2 trigger before spawns. | 10s wait per trigger.");
            break;
            case "Hard":
                howOftenAnAudioOrMonsterSpawns = 5.0f;
                Debug.Log("Difficulty is set to | HARD | x1 trigger before spawns. | 5s wait per trigger.");
            break;
            default:
            //Do nothing and use input spawn rate times.
            break;
        }
    }

    //The jumpscare that plays when the player touches a monster but still has some sanity left over.
    //NOTE: THIS IS SET VIA THE ENEMY AI SCRIPT! Possible monsters are : BlackSmogMonster, ShyGuy, AggressiveDog, HippoMonster
    //ALSO, MAKE SURE THiS LIST BELOW CO-RESPONDS TO THE INDEX OF THE MONSTERS IN "monsterJumpScaresList"!
    public void PlayMildJumpscare(){
        //If list has equal or more than 4 jumpscares... do the one called!
        if(monsterJumpScaresList.Count >=4){
            switch(nextMonsterJumpscareAtPlayer){
            case "BlackSmogMonster":
            Instantiate(monsterJumpScaresList[0],playerNewZoneGUICanvas.transform);
            break;
            case "ShyGuy":
            Instantiate(monsterJumpScaresList[1],playerNewZoneGUICanvas.transform);
            break;
            case "AggressiveDog":
            Instantiate(monsterJumpScaresList[2],playerNewZoneGUICanvas.transform);
            break;
            case "HippoMonster":
            Instantiate(monsterJumpScaresList[3],playerNewZoneGUICanvas.transform);
            break;
            default:
            Instantiate(monsterJumpScaresList[4],playerNewZoneGUICanvas.transform);
            break;
            }
        }
    }

    //This method instantiates the confused SFX for when the player goes through a wrong path.
    //NOTE: make sure the last object in monsterJumpScaresList is the SFX!
    public void PlayConfusedSfx(){
        Instantiate(monsterJumpScaresList[monsterJumpScaresList.Count-1],playerObject.transform);
    }

    //Perform game over when the sanity meter reaches below 0. Else return false.
    //FIXME: Right now, it just instantly loads the main menu scene. Change to playing a jumpscare and popping up a new Game Over GUI?
    public bool TestGameOver(){
        if(sanityMeter <= 0){
            //Below is a temp fix for when the player dies, they just go back to the main Menu scene.
            SceneManager.LoadScene("MainMenuScene");
            return true;
        }else{
            return false;
        }

        /* Implement the FULL GAMEOVER jumpscare here. The "nextMonsterJumpscareAtPlayer" is set on the game manager by other scripts interacting with it like "PlayerGoesThroughNextArea.cs"
        switch(nextMonsterJumpscareAtPlayer){
        
            }
        */
    }

}
