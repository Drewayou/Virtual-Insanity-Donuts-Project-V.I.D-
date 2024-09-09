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

    // Start is called before the first frame update
    void Start()
    {
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

    //Set the last SEEN position of the player (Useful for player hide-seek mechanics)
    public Vector3 getPlayerLastSeenLocation(){
        return playerLastSeenPosition = new Vector3(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z);
    }
}
