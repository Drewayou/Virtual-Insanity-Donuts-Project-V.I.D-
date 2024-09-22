using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGoesThroughNextArea : MonoBehaviour
{

    //GameManager Object
    GameObject gameManagerinstance;

    //Save this object's box collider unto here.
    BoxCollider thisCollider;

    //Canvas GUI object - Player New Round Canva object.
    [SerializeField]
    [Tooltip("Drag the \"PlayerNewZoneGUICanvas\" here.")]
    GameObject newZoneNotificationCanvaObject;

    // Start is called before the first frame update.
    void Start()
    {
        //Find the game manager object in the scene.
        gameManagerinstance = GameObject.Find("GameManagerObject");

        //Sets this player object's colliders.
        thisCollider = this.gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Used the methods that colliders have built-in. Docs can be found here : https://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html
    void OnTriggerEnter(Collider colliderThatTouchesThisTrigger){
        if(colliderThatTouchesThisTrigger.tag == "Player"){}

        //Call Game Manager IObject and it's component script's methods to increase the zone level by 1.
        gameManagerinstance.GetComponent<GameManagerScript>().SetZoneLevel(gameManagerinstance.GetComponent<GameManagerScript>().GetZoneLevel() + 1);
            
            //Set the player back to 0,0,0.
            colliderThatTouchesThisTrigger.gameObject.transform.position = new Vector3(0, 0, 0);

            //Re-activate the new zone GUI object to phase the player to a new zone. Moreover, calls the game manager to trigger what needs to be done to go to the next zone.
            newZoneNotificationCanvaObject.SetActive(true);
            gameManagerinstance.GetComponent<GameManagerScript>().PlayerGoesToNextZone();
        }
}

