using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractableScript : MonoBehaviour
{

    GameManagerScript thisGameManagerScriptInstance;

    Collider thisBatteryCollider;

    [SerializeField]
    [Tooltip("Drag and drop the battery charge SFX here.")]
    GameObject batteryRechargeSFX;

    // Start is called before the first frame update
    void Start()
    {
        thisGameManagerScriptInstance = GameObject.Find("GameManagerObject").GetComponent<GameManagerScript>();
        thisBatteryCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //The trigger for the battery to be merged with the flashlight.
    void OnTriggerEnter(Collider colliderThatTouchesThisTrigger){
        Debug.Log("Collision Detected!");
            if(colliderThatTouchesThisTrigger.gameObject.name == "FlashlightPrefab"){
                Debug.Log("A Battery and Flashlight Collided!");
                //Refill with a fully charged flash light.
                thisGameManagerScriptInstance.SetFlashLightBattery(thisGameManagerScriptInstance.GetPlayerFlashlightBatteryHealthMAXIMUM());
                Instantiate(batteryRechargeSFX, thisGameManagerScriptInstance.playerObject.transform);

                //Delete the battery because it's used.
                Destroy(this.gameObject);
            }
    }
}
