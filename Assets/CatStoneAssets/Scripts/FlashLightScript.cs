using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class FlashLightScript : MonoBehaviour
{

    //GameManager Object, useful for pulling specific scripts and objects for use in other scripts. Automatically set in "Start()".
    private GameObject gameManagerinstance;

    //The actual object that emmits light in the game from the flashlight.
    [SerializeField]
    [Tooltip("Drag the \"FlashLightLightSourceObject\" inside the prefab here.")]
    private GameObject flashLightLightSource;

    [SerializeField]
    [Tooltip("Drag and drop the \"FlashLightToggleSFX\" object from the SFX prefabs here.")]
    public GameObject FlashLightToggleSFX;

    [SerializeField]
    [Tooltip("Drag and drop the \"FlashLightBatteryIndicator\" object from the prefabs here.")]
    public GameObject FlashLightBatteryIndicator;

    private CapsuleCollider flashLightCollider;

    [SerializeField]
    [Tooltip("Drag and drop the materials that show what indicator the color the battery should be.")]
    public Material FlashLightBatteryIndicatorGood, FlashLightBatteryIndicatorOkay, FlashLightBatteryIndicatorBad, FlashLightBatteryIndicatorDead;

    //Bool to set what toggle value the light is on.
    [Tooltip("Bool of flashlight toggle value.")]
    public bool flashlightIsON = false;

    //Bool to set toggle value if the light has no more battery in it.
    [Tooltip("Bool of flashlight battery left value.")]
    public bool flashlightHasBatteryLeft;

    [SerializeField]
    [Tooltip("Set the flashlight's settings to how you want it!")]
    public float flashlightRange = 100f, flashlightIntensity = 75f;

    //Sets the inital ammount of time a fully charged flashlight has (seconds).
    public float flashlightFullyChargedBatteryTime = 100;

    //Sets the time the player has left in their flashlight (seconds).
    public float flashlightBatteryTimeLeft;
    
    //The flashlight max intensity for this game/round if it's full battery.
    private float flashlightMaxInitialIntensity;

     //The flashlight max range for this game/round if it's full battery.
    private float flashlightMaxInitialRange;

    //Gets the Input Actions Asset to draw inputs from.
    //Drop the action map "XRI RightHand Interaction"
    [SerializeField]
    [Tooltip("Drag and drop the Input Actions Asset that this script would use.")]
    public InputActionProperty XRIInputActionsAsset;

    //The flashlight raycast to interact with enemies (BlackSmogMonster && ????).
    private RaycastHit lightShinesOnAnEnemy;

    //Enemy Holder Object in the scene that holds all enemies currently in the zone.
    private GameObject enemyHolderObject;

    //Sets what monsters interact with the flashlight to interact with their script.
    //Grab the possible monster prefabs here.
    /*
    NOTE: MAKE SURE SMOG MONSTER IS 0, AND INDEX OF LIST
    */
    [SerializeField]
    [Tooltip("Select, drag, and add what different MONSTERS would interact with the flashlight.")]
    public List<GameObject> monstersReactsToFlashlightList;

    //Start is called before the first frame update.
    void Start()
    {
        //Instantializes the game manager object.
        gameManagerinstance = GameObject.Find("GameManagerObject");

        //Initializes the EnemyHolderObject in the scene to scan for monsters with the flashlight.
        enemyHolderObject = gameManagerinstance.GetComponent<GameManagerScript>().EnemyHolderObject;

        //Make sure that the list of monsters that the flashlight interacts with isn't empty. If so, warn the devs.
        if(monstersReactsToFlashlightList.Count == 0){
            Debug.LogError("LOAD FLASHLIGHT SCRIPT IN PLAYER PREFAB WITH MONSTERS THAT REACT TO LIGHT!");
        }
        
        //Set the flashlight range & intensity via the inputs of this script.
        flashLightLightSource.GetComponent<Light>().range = flashlightRange;
        flashLightLightSource.GetComponent<Light>().intensity = flashlightIntensity;

        //Set the max intensity and range of the flashlight baseed on the input settings.
        flashlightMaxInitialIntensity = flashlightIntensity;
        flashlightMaxInitialRange = flashlightRange;

        //Make sure the flashlight has battery when first starting
        flashlightHasBatteryLeft = true;

        //Start with a fully charged flash light.
        flashlightBatteryTimeLeft = flashlightFullyChargedBatteryTime;

        //Start with the flashlight "Turned on" depending on the toggle button in the editor of this script".
        if(flashlightIsON){
            flashLightLightSource.SetActive(true);
            }else{
                flashLightLightSource.SetActive(false);
            }

        //Make sure the flashlight collider is tied to this flashlight.
        flashLightCollider = this.gameObject.GetComponent<CapsuleCollider>();
    }

    //Update is called once per frame.
    void Update(){   


    //If the flashlight is sucessfully on, draw a raycast line for possible interactions. Moreover, turn on a light sorce object.
    if(flashlightIsON){
        int layerMask = 1 << 2;
        Debug.DrawLine(flashLightLightSource.transform.position, flashLightLightSource.transform.TransformDirection(Vector3.forward*100));
        //Code for raycast examples found here : https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
        if (Physics.Raycast(flashLightLightSource.transform.position, flashLightLightSource.transform.TransformDirection(Vector3.forward*100), out RaycastHit whatDidTheFlashlightHit, flashlightRange*100, ~layerMask))
        
            //Debug line to check what the flashlight raycast is hitting.
            Debug.Log("Flashlight is hitting : " + whatDidTheFlashlightHit.collider.name);
        
            /*FIXME: This Block of code below is not yet done as more monsters are to be added. 
            Essentially, If the ray collided with a monster found in the "monstersReactsToFlashlightList", 
            tell the monster's script to do an action.

            NOTE: MAKE SURE THE INDEX OF THE LIST MATCHES THE INSPECTOR MONSTERS AND SCRIPT BELOW! 
            IF NOT, IT WILL MIX UP THE MONSTER INTERACTIONS AND TRIGGER THE WRONG MONSTER!
            IF A SCRIPT IS MISSING, ADD IT BELOW!

            SO FAR:
            Smogmonster = 0
            ShyGuy      = 1
            ????        = 2...
            ect.

            */

            //Make sure the object the flashlight hits isn't null.
            try {

            //Smog monster special flashlight interaction.
            if(whatDidTheFlashlightHit.collider.name == monstersReactsToFlashlightList[0].name + "(Clone)" || whatDidTheFlashlightHit.collider.name == monstersReactsToFlashlightList[0].name){
                FlashLightMonsterReactionToRaycastLightTest(whatDidTheFlashlightHit);
                }

            //ShyGuy monster special flashlight interaction.
            if(whatDidTheFlashlightHit.collider.name == monstersReactsToFlashlightList[1].name + "(Clone)" || whatDidTheFlashlightHit.collider.name == monstersReactsToFlashlightList[1].name){
                FlashLightMonsterReactionToRaycastLightTest(whatDidTheFlashlightHit);
                }

            }catch (NullReferenceException ex){
                //Debug.Log("Player is shining the flashlight down a hall.");
            }
            
        }
        
    //Gets the XRIInputActionsAsset input map and checks if the specific input is used.
    if(XRIInputActionsAsset.action.WasPressedThisFrame()){
        VariableToggle();
        }

        //Decrese the flashlight time if it's on. If ever the battery reaches zero, have it turn off.
        //Note: I've made it so that if the flashlight has 10seconds left, the intensity will slowly drain from it's max intensity, to 0.

        if(IsTheFlashLightOn()){
            flashlightBatteryTimeLeft -= Time.deltaTime;

            //If Flashlight is above half charged, set material to good.
            if(flashlightBatteryTimeLeft > flashlightFullyChargedBatteryTime/2){
                FlashLightBatteryIndicator.GetComponent<MeshRenderer>().material = FlashLightBatteryIndicatorGood;
            }
            //Else If Flashlight is below half charged but above 1/2th, set material to good.
            else if(flashlightBatteryTimeLeft <= (flashlightFullyChargedBatteryTime/2) && flashlightBatteryTimeLeft > (flashlightFullyChargedBatteryTime/4)){
                FlashLightBatteryIndicator.GetComponent<MeshRenderer>().material = FlashLightBatteryIndicatorOkay;
            }

            //If flashlight has less than 1/4th battery left, begin dimming it, and set the material to bad.
            if(flashlightBatteryTimeLeft <= flashlightFullyChargedBatteryTime/4){
                flashlightIntensity -= Time.deltaTime * flashlightIntensity/4;
                flashlightRange -= Time.deltaTime * flashlightMaxInitialRange/4;

                //Slowly change the light color to black to emphasize the flashlight dying. Proportional to how much time the flashlight has left.
                Color flashLightDyingColor = new Color(flashlightBatteryTimeLeft/flashlightFullyChargedBatteryTime*10f,flashlightBatteryTimeLeft/flashlightFullyChargedBatteryTime*10f,flashlightBatteryTimeLeft/flashlightFullyChargedBatteryTime*10f);
                flashLightLightSource.GetComponent<Light>().color = flashLightDyingColor;
                FlashLightBatteryIndicator.GetComponent<MeshRenderer>().material = FlashLightBatteryIndicatorBad;
            }

            if(flashlightBatteryTimeLeft <=0){
                //Turn Flashlight off.
                flashLightLightSource.SetActive(false);
                flashlightHasBatteryLeft = false;
                flashlightIsON = false;
                flashlightIntensity = flashlightMaxInitialIntensity;
                flashlightRange = flashlightMaxInitialRange;
                //FIXME: Add a short circuit SFX when the battery dies.
                //Since flashlight has no battery left, set the material to dead.
                FlashLightBatteryIndicator.GetComponent<MeshRenderer>().material = FlashLightBatteryIndicatorDead;

            }
        }
    }

    //This is a toggle method uses the controller index trigger input to set the bool of the flashlight state from on-> off, 
    //and furthermore, hide the light emitting object.
    public void VariableToggle(){

        //If the flashlight has battery in it, do the toggle.
        if(flashlightHasBatteryLeft){
            if(flashlightIsON){
                flashlightIsON = false;
                flashLightLightSource.SetActive(false);
            }else{
                flashlightIsON = true;
                flashLightLightSource.SetActive(true);
            }
            //Play flashlight toggle button press.
            Instantiate(FlashLightToggleSFX);
        }else{
            //FIXME: Add a new SFX if the flashlight needs more battery. EX : Instantiate(FlashLightOutOfBatteryToggleSFX);
        }
    }

    //Getters and setters ----------------------------------------------------------------------------------------

    //Getter method used for other scripts to determine if the flashight toggle is on.
    //This will be usful if an enemy is hit by the flashlight if it's on.
    public bool IsTheFlashLightOn(){
        return flashlightIsON;
    }

    //Getter method for how much light (in seconds) the flashlight has left.
    public float FlashLightGetTheTimeLeft(){
        return flashlightBatteryTimeLeft;
    }

    //Setter method to adjust how much light the flashlight has left.
    public void FlashLightSetTimeLeft(float timeToAddToTheFlashlight){
        flashlightBatteryTimeLeft = timeToAddToTheFlashlight;
    }
    
    //Flashlight monster scanner method to check if the interaction was made yet for specific monsters.
    public void FlashLightMonsterReactionToRaycastLightTest(RaycastHit flashlighthit){
        try{flashlighthit.collider.gameObject.GetComponent<BasicAhhEnemyAI>().LightReaction();
        //Debug.Log(flashlighthit.collider.name + " monster got hit by the flashlight!");
                }catch(Exception e){
                    Debug.LogError(e + "\n | This may have happened because the Flashlight Script has specific conditions: "+
                    "\n 1) Monster reacts to light is listed and in the insperctor of this object."+ 
                    "\n 2) Reaction is hard coded in \"FlashLightScript.cs.\""+
                    "\n 3) Un-finished flashlight reacion script triggered.");
                }
    }
}
