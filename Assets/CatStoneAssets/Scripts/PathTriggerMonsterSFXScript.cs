using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTriggerMonsterSFXScript : MonoBehaviour
{
    //Note: This script IS NOT meant to be interacting with the player when they walk through the paths. That's another script.
    //This script is for how the game manager spawns monsters and triggers monster sfx sounds.

    //GameManager Object
    GameObject gameManagerinstance;

    //Set what pillar this text GUI should show if needed.
    [Serializable] 
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

    //Sets what monster audio prefab should spawn when the trigger is used.
    [SerializeField] 
    [Tooltip("Select and drag what AUDIO would generate on this path.")]
    public GameObject selectedMonsterAudio;

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

        //FIXME: If this path has it's trigger counter "triggersTillAMonsterSpawns"<=0, spawn the monster tied to this object. Else, play the audio tied to this object.
        if(triggersTillAMonsterSpawns <= 0){
            if(EnemyHolderObject.transform.childCount<5){
                //Spawns monster -100 meters away from this path object.
                Instantiate(selectedMonsterToSpawn, this.gameObject.transform.forward*100, Quaternion.identity, EnemyHolderObject.transform);

                //Tells the game manager if it sucessfully spawns a monster.
                gameManagerinstance.GetComponent<GameManagerScript>().newMonsterSpawned = true;
            }
        }else{
            //Spawn the audio on this trigger path ordinate, closer to the player but still on the ordinate path.
            Instantiate(selectedMonsterAudio,this.gameObject.transform.forward*25, Quaternion.identity, this.gameObject.transform);
        }
    }
}
