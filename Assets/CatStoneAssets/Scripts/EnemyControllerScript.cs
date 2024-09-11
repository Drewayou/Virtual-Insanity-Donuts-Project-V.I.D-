using System.Collections;
using System.Collections.Generic;
using Assets.OVR.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerScript : MonoBehaviour
{

    [Tooltip ("What object to follow, in this case the player")]
    [SerializeField]
    public GameObject Target;

    //This is the variable that stores the player's last location found when spotted.
    private Vector3 playerLastSeenPosition;

    //This is the Navemesh agent component attached to this enemy object.
    private NavMeshAgent thisNavMeshAgent;

    //Monster's initial speed
    public float thisMonstersOriginalSpeed;

    //Monster's initial acceleration
    public float thisMonstersOriginalAcceleration;

    //Get starting position of the enemy.
    private Vector3 thisEnemyStartingPosition;

    // Start is called before the first frame update
    void Start()
    {
        //Get the position of this enemy's spawn.
        thisEnemyStartingPosition = gameObject.transform.position;

        //Set up player target gameObject, find their object instance in the scene hierarchy.
        Target = GameObject.Find("PlayerObject");

        //Declare the Navmesh agent connected to THIS enemy game object.
        thisNavMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        
        //FIXME: Make a method to set up this enemy settings if there are any.
        //setUpEnemySettings();
    }

    // Update is called once per frame
    void Update()
    {
        //FIXME: This line basically consistently tracks, and follows the player. Change as needed.
        thisNavMeshAgent.SetDestination(playerLastSeenPosition);
    }

    //Get the last SEEN position of the player (Useful for player hide-seek mechanics).
    public Vector3 GetPlayerLastSeenLocation(){
        return playerLastSeenPosition = new Vector3(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z);
    }

    //SET the last SEEN position of the player (Useful for player hide-seek mechanics).
    public void SetPlayerLastSeenLocation(Vector3 newSuspectedPlayerLocation){
        playerLastSeenPosition = newSuspectedPlayerLocation;
    }

    //Sets the monster SPEED. (This is good for jumpscares.)
    public void ChangeMonsterSpeed(float newMonsterSpeed){
        thisNavMeshAgent.speed = newMonsterSpeed;
    }

    //RESETS the monster SPEED. Acording to initial input. (This is good AFTER jumpscares.)
    public void ResetMonsterSpeed(){
        thisNavMeshAgent.speed = thisMonstersOriginalSpeed;
    }

    //Sets the monster ACCELERATION. (This is good for jumpscares, on how FAST the monster speeds up.)
    public void ChangeMonsterAcceleration(float newMonsterAcceleration){
        thisNavMeshAgent.acceleration = newMonsterAcceleration;
    }


    //RESETS the monster ACCELERATION. Acording to initial input. (This is good AFTER jumpscares.)
    public void ResetMonsterAcceleration(){
        thisNavMeshAgent.acceleration = thisMonstersOriginalAcceleration;
    }


    //Getter for the enemy's starting position (useful for other scripts).
    public Vector3 GetEnemyStartingPosition(){
        return thisEnemyStartingPosition;
    }

    //Pause the enemy movement.
    public void PauseEnemyMovement(){
        thisNavMeshAgent.isStopped = true;
    }

    //Continue the enemy movement.
    public void ContinueEnemyMovement(){
        thisNavMeshAgent.isStopped = false;
    }
}
