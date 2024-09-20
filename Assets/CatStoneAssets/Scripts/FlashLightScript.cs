using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class FlashLightScript : MonoBehaviour
{
    //The actual object that emmits light in the game from the flashlight.
    [SerializeField]
    [Tooltip("Drag the \"FlashLightLightSourceObject\" inside the prefab here.")]
    private GameObject FlashLightLightSource;

    [SerializeField]
    [Tooltip("Drag and drop the \"FlashLightToggleSFX\" object from the SFX prefabs here.")]
    public GameObject FlashLightToggleSFX;

    //Bool to set what toggle value the light is on.
    [Tooltip("Bool of flashlight toggle value.")]
    public bool flashlightIsON = false;

    //Bool to set toggle value if the light has no more battery in it.
    [Tooltip("Bool of flashlight battery left value.")]
    public bool flashlightHasBatteryLeft;

    [SerializeField]
    [Tooltip("Set the flashlight's settings to how you want it!")]
    public float flashlightRange = 100f, flashlightIntensity = 75f;

    //Sets the time the player has left in their flashlight (seconds).
    public float flashlightBatteryTimeLeft = 100;

    //The flashlight max intensity for this game/round if it's full battery.
    public float flashlightMaxInitialIntensity;

    //Gets the Input Actions Asset to draw inputs from.
    //Drop the action map "XRI RightHand Interaction"
    [SerializeField]
    [Tooltip("Drag and drop the Input Actions Asset that this script would use.")]
    public InputActionProperty XRIInputActionsAsset;

    //The flashlight raycast to interact with enemies (BlackSmogMonster && ????).
    private RaycastHit lightShinesOnAnEnemy;

    //FIXME: The monsters that interact with the flashlight.
    //Currently, the flashlight checks the whole scene for these monsters. Aim to have ONE gameobject to hold the monsters in, 
    //instead of seperate monster gameobjects.
    private GameObject blackSmogMonster;

    //Start is called before the first frame update.
    void Start()
    {
        //Set the flashlight range & intensity via the inputs of this script.
        FlashLightLightSource.GetComponent<Light>().range = flashlightRange;
        FlashLightLightSource.GetComponent<Light>().intensity = flashlightIntensity;

        //Set the max intensity of the flashlight baseed on the input settings.
        flashlightMaxInitialIntensity = flashlightIntensity;

        //Make sure the flashlight has battery when first starting
        flashlightHasBatteryLeft = true;

        //Start with the flashlight "Turned on" depending on the toggle button in the editor of this script".
        if(flashlightIsON){
            FlashLightLightSource.SetActive(true);
            }else{
                FlashLightLightSource.SetActive(false);
            }
        
        //FIXME: Currently, the flashlight checks the whole scene for this monster. Aim to have ONE gameobject to hold the monsters in to search it.
        //This script first checks if it's in the scene, then assigns it to a temp variable.
        if(GameObject.Find("BlackSmogMonster") != null){
            Debug.Log("Smog monster in scene detected.");
            blackSmogMonster = GameObject.Find("BlackSmogMonster");
        }
    }

    //Update is called once per frame.
    void Update(){   

    //Gets the XRIInputActionsAsset input map and checks if the specific input is used.
    if(XRIInputActionsAsset.action.WasPressedThisFrame()){
        VariableToggle();

        //If the flashlight is sucessfully on, draw a raycast line for possible interactions. Moreover, turn on a light sorce object.
        if(flashlightIsON){
            Debug.DrawLine(FlashLightLightSource.transform.position, FlashLightLightSource.transform.TransformDirection(Vector3.forward*100));
            //Code for raycast examples found here : https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
            if (Physics.Raycast(FlashLightLightSource.transform.position, FlashLightLightSource.transform.TransformDirection(Vector3.forward*100), out RaycastHit whatDidTheFlashlightHit, flashlightRange*100))
            {
                //If the ray collided with the smog monster, tell it's script to do an action.
                if(whatDidTheFlashlightHit.collider.name == "BlackSmogMonster"){
                    Debug.Log("Smog monster Felt Scared!");
                    blackSmogMonster.GetComponent<BlackSmogMonsterScript>().BlackSmogGotScared();
                    }
                }
            }
        }

        //Decrese the flashlight time if it's on. If ever the battery reaches zero, have it turn off.
        //Note: I've made it so that if the flashlight has 10seconds left, the intensity will slowly drain from it's max intensity, to 0.

        if(IsTheFlashLightOn()){
            flashlightBatteryTimeLeft -= Time.deltaTime;
            if(flashlightBatteryTimeLeft <=10){
                LerpFlashlightIntensityWhenBatteryDies();
            }
            if(flashlightBatteryTimeLeft <=0){
                flashlightHasBatteryLeft = false;
                flashlightIsON = false;
                flashlightIntensity = flashlightMaxInitialIntensity;
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
                FlashLightLightSource.SetActive(false);
            }else{
                flashlightIsON = true;
                FlashLightLightSource.SetActive(true);
            }
            //Play flashlight toggle button press.
            Instantiate(FlashLightToggleSFX);
        }else{
            //FIXME: Add a new SFX if the flashlight needs more battery. EX : Instantiate(FlashLightOutOfBatteryToggleSFX);
        }
    }

    //Lerp method used for making the flashlight loose it's power.
    //Used the lerp method found here : https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#how_to_use_lerp_in_unity
    IEnumerator LerpFlashlightIntensityWhenBatteryDies(){
        float TimeUntilBatteryDies = 0;
                while(TimeUntilBatteryDies <= 10){
                    flashlightIntensity = Mathf.Lerp(flashlightMaxInitialIntensity, 0, TimeUntilBatteryDies / 9);
                    TimeUntilBatteryDies += Time.deltaTime;
                yield return null;
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
}
