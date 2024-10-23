using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGoesThroughNextArea : MonoBehaviour
{

    //This script managers all cases when they player goes through a chosen path.
    //GameManager Object
    GameObject gameManagerinstance;

    //Save this object's box collider unto here.
    BoxCollider thisCollider;

    //Saves he monster that's tied to this path if it exists. Used for jumpscare purposes by Game manager script.
    PathTriggerMonsterSFXScript thisPathsMonsterScript;

    //Canvas GUI object - Player New Round Canva object.
    [Tooltip("This Automatically connects when this object is loaded unto the scene.")]
    public GameObject newZoneNotificationCanvaObject;

    //Sets and saves if this script is tied to a bad path or a safe path.
    [Tooltip("Set to check if this path, that this script is attached to, is safe or bad.")]
    public bool thePathIsSafe = true;

    //Start is called before the first frame update.
    void Start()
    {
        //Find the game manager object in the scene.
        gameManagerinstance = GameObject.Find("GameManagerObject");

        //Sets this player object's colliders.
        thisCollider = this.gameObject.GetComponent<BoxCollider>();

        //Sets what monster that's attached to this object by the PathTriggerMonsterSFXScript.
        thisPathsMonsterScript = this.gameObject.GetComponent<PathTriggerMonsterSFXScript>();

        //Find the Neww Zone GUI tied to the player object.
        newZoneNotificationCanvaObject = gameManagerinstance.GetComponent<GameManagerScript>().GetPlayerNewZonePopupGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Used the methods that colliders have built-in. Docs can be found here : https://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html
    void OnTriggerEnter(Collider colliderThatTouchesThisTrigger){
        Debug.Log("TriggerActive!");
        if(thePathIsSafe){
            if(colliderThatTouchesThisTrigger.gameObject.tag == "Player"){

                //Set the player back to 0,0,0.
                colliderThatTouchesThisTrigger.gameObject.transform.position = new Vector3(0, 0, 0);

                //Call Game Manager Object and it's component script's methods to increase the zone level by 1.
                gameManagerinstance.GetComponent<GameManagerScript>().SetZoneLevel(gameManagerinstance.GetComponent<GameManagerScript>().GetZoneLevel() + 1);

                //Re-activate the new zone GUI object to phase the player to a new zone. Moreover, calls the game manager to trigger what needs to be done to go to the next zone.
                newZoneNotificationCanvaObject.SetActive(true);
                gameManagerinstance.GetComponent<GameManagerScript>().PlayerGoesToNextZone();
            }
        }else{

            //FIXME: edit the switch statement that sets the GAMEMANAGERSCRIPT game over jump scare.
            switch(thisPathsMonsterScript.selectedEnemy.ToString()){
                case "blackSmogMonsterPath":
                        gameManagerinstance.GetComponent<GameManagerScript>().nextMonsterJumpscareAtPlayer = "BlackSmogMonster";
                break;
                
                case "hippoThumperPath":
                        gameManagerinstance.GetComponent<GameManagerScript>().nextMonsterJumpscareAtPlayer = "HippoThumper";
                break;
                
                case "jamiroquaiGraberPath":
                        gameManagerinstance.GetComponent<GameManagerScript>().nextMonsterJumpscareAtPlayer = "ShyGuy";
                break;

                case "strayAgressiveDogPath":
                        gameManagerinstance.GetComponent<GameManagerScript>().nextMonsterJumpscareAtPlayer = "StrayAgressiveDogPath";
                break;
            };
            //FIXME: Right now the player always looses 25 sanity. This can be changed hard coded here.
            gameManagerinstance.GetComponent<GameManagerScript>().PlayerLosesSanity(25f);
            
                //Set the player back to 0,0,0.
                colliderThatTouchesThisTrigger.gameObject.transform.position = new Vector3(0, 0, 0);

                //FIXME: You need to go into the GameManagerScript, and add a PlayerReplaysZone() to change the GUI and sfx!
                //Re-activate the new zone GUI object to tell the player they're in the same zone. 
                //Moreover, calls the game manager to trigger what needs to be done tosignal the player went in a wrong path.
                newZoneNotificationCanvaObject.SetActive(true);
                gameManagerinstance.GetComponent<GameManagerScript>().PlayerReplaysZone();
        }
    }
}

