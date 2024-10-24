using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathTriggerMonsterSFXScript : MonoBehaviour
{
    //Note: This script IS NOT meant to be interacting with the player when they walk through the paths. That's another script.
    //This script is for how the game manager spawns monsters and triggers monster sfx sounds.

    //GameManager Object
    GameObject gameManagerinstance;

    //Set what pillar this text GUI should show if needed.
    [Tooltip("Select what case of enemy this path should generate.")]
    public enum SelectedEnemyPath
    {SAFEPATH, blackSmogMonsterPath, hippoThumperPath, jamiroquaiGraberPath, strayAgressiveDogPath}
    public SelectedEnemyPath selectedEnemy;

    //An int variable to count how many times this path needs to trigger before spawning a monster.
    public int triggersTillAMonsterSpawns;

    //Sets what monster prefab should spawn when the trigger is used.
    [SerializeField] 
    [Tooltip("Select and drag what monster would generate on this path.")]
    public GameObject selectedMonsterToSpawn;

    //A private bool that functions like a lock to check if a monster is already in the zone and thus, will NOT spawn.
    private bool monsterAlreadyInZone = false;

    //Sets what monster audio prefab should spawn when the trigger is used.
    //Grab the possible SFX prefabs here.
    [SerializeField]
    [Tooltip("Select and drag and add what different AUDIO SFX would generate on this path.")]
    public List<GameObject> audioSFXListToPlay;

    //The actual random audio object that gets instantiated and plays selected from the audio input above. Set via methods below.
    private GameObject selectedAudioSFXToPlay;

    //The EnemyHolderObject that holds any and all monsters for easy location tracking and game management.
    [Tooltip("This gets set via the GameManagerObject!")]
    private GameObject EnemyHolderObject;

    // Start is called before the first frame update
    void Start()
    {
        //Find the game manager object in the scene.
        gameManagerinstance = GameObject.Find("GameManagerObject");

        //Pulls the enemy holder object reference from the game manager.
        EnemyHolderObject = gameManagerinstance.GetComponent<GameManagerScript>().EnemyHolderObject;

        //Find the level difficulty selected for how often a path has to trigger before spawning a monster.
        triggersTillAMonsterSpawns = GetDifficultyToSetTriggerTimes();

        //If the audio list is empty, alert the devs.
        if(audioSFXListToPlay.Count == 0){
            Debug.LogError("Audio list for " + this.gameObject.name + " is empty! Drag audio for this path trigger prefab!");
        }
    }

    //This method uses a switch statement to pull data from the game manager instance and solve how many times this path needs to trigger
    //before spawning a monster.
    public int GetDifficultyToSetTriggerTimes(){
        switch(gameManagerinstance.GetComponent<GameManagerScript>().selectedLevelDiffculty.ToString()){
            case "Easy":
                return 3;
            case "Normal":
                return 2;
            case "Hard":
                return 1;
            default:
                return 1;
        }
    }

    //This method is meant for an attempt a trigger on the path by the game manager object, to spawn sounds and monsters.
    public void AttemptTrigger(){

        Debug.Log(this.gameObject.name + " Has ben triggered!");
        triggersTillAMonsterSpawns -= 1;

        //Randomly selects which audio to play from the list if there's triggers left.
        selectedAudioSFXToPlay = audioSFXListToPlay[Random.Range(0,audioSFXListToPlay.Count)];

        //Set this check to false in the start
        monsterAlreadyInZone = false;

        //Check if the monster this path wants to spawn is already present in the zone via the "EnemyHolderObject" in the scene.
        //If even ONE instance of the monster desired to spawn is already in the zone, DO NOT spawn it (Set "monsterAlreadyInZone" to TRUE).
        foreach(Transform monsterCheck in EnemyHolderObject.transform){
            if (monsterCheck.gameObject.name == selectedMonsterToSpawn.name + "(Clone)"){
                monsterAlreadyInZone = true;
            }
        }
        

        //FIXME: If this path has it's trigger counter "triggersTillAMonsterSpawns"<=0, spawn the monster tied to this object. Else, play the audio tied to this object.
        if(triggersTillAMonsterSpawns <= 0 && selectedEnemy != SelectedEnemyPath.SAFEPATH && !monsterAlreadyInZone){
            if(EnemyHolderObject.transform.childCount<5){
                //Spawns monster -100 meters away from this path object.
                Instantiate(selectedMonsterToSpawn, this.gameObject.transform.forward*100, Quaternion.identity, EnemyHolderObject.transform);

                //Tells the game manager if it sucessfully spawns a monster.
                gameManagerinstance.GetComponent<GameManagerScript>().newMonsterSpawned = true;
            }
        }else{
            //Spawn the audio on this trigger path ordinate, closer to the player but still on the ordinate path.
            Instantiate(selectedAudioSFXToPlay,this.gameObject.transform.forward*50, Quaternion.identity, this.gameObject.transform);
        }
    }
}
