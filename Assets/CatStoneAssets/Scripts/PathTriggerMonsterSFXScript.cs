using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTriggerMonsterSFXScript : MonoBehaviour
{
    //Note: This script IS NOT meant to be interacting with the player when they walk through the paths. That's another script.
    //This script is for how the game manager spawns monsters and triggers monster sfx sounds.
    //Set what pillar this text GUI should show if needed.
    [Serializable] 
    [Tooltip("Select what case of enemy this path should generate.")]
    public enum SelectedEnemyPath
    {SAFEPATH, blackSmogMonsterPath, hippoThumperPath, jamiroquaiGraberPath, strayAgressiveDogPath}
    public SelectedEnemyPath selectedEnemy;

    //Sets what monster prefab should spawn when the trigger is used.
    [SerializeField] 
    [Tooltip("Select and drag what monster would generate on this path.")]
    public GameObject selectedMonsterToSpawn;

    //Sets what monster audio prefab should spawn when the trigger is used.
    [SerializeField] 
    [Tooltip("Select and drag what AUDIO would generate on this path.")]
    public GameObject selectedMonsterAudio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This method is meant for an attempt a trigger on the path by the game manager object, to spawn sounds and monsters.
    public void AttemptTrigger(){
        Debug.Log(this.gameObject.transform.parent.name + ": PathWasTriggered!");
    }
}
