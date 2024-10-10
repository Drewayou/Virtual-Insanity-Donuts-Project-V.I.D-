using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTriggerScript : MonoBehaviour
{
    //Set what pillar this text GUI should show if needed.
    [Serializable] 
    [Tooltip("Select what case of enemy this path should generate.")]
    public enum SelectedEnemyPath
    {blackSmogMonsterPath, hippoThumperPath, jamiroquaiGraberPath, strayAgressiveDogPath}
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

    //This method is meant for a method to attempt a trigger on the path.
    public void AttemptTrigger(){
        Debug.Log(this.gameObject.transform.parent.name + ": PathWasTriggered!");
    }
}
