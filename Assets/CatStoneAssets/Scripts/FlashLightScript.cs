using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    [Tooltip("Set the flashlight's settings to how you want it!")]
    public float flashlightRange = 100f, flashlightIntensity = 1f;

    //The flashlight raycast to interact with enemies (BlackSmogMonster && ????).
    private RaycastHit lightShinesOnAnEnemy;

    //FIXME: The monsters that interact with the flashlight.
    //Currently, the flashlight checks the whole scene for these monsters. Aim to have ONE gameobject to hold the monsters in, 
    //instead of seperate monster gameobjects.
    private GameObject blackSmogMonster;

    //Start is called before the first frame update.
    void Start()
    {
        //Set the flashlight range and intensity via the inputs of this script.
        FlashLightLightSource.GetComponent<Light>().range = flashlightRange;
        FlashLightLightSource.GetComponent<Light>().intensity = flashlightIntensity;

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
    void Update()
    {
        VariableToggle();
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

    //This is a toggle method uses the controller index trigger input to set the bool of the flashlight state from on-> off, 
    //and furthermore, hide the light emitting object.
    public void VariableToggle(){
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)) {
            if(flashlightIsON){
                flashlightIsON = false;
                FlashLightLightSource.SetActive(false);
            }else{
                flashlightIsON = true;
                FlashLightLightSource.SetActive(true);
            }
            //Play flashlight toggle button press.
            Instantiate(FlashLightToggleSFX);
        }
    }

    //Getter method used for other scripts to determine if the flashight toggle is on.
    //This will be usful if an enemy is hit by the flashlight if it's on.
    public bool IsTheFlashLightOn(){
        return flashlightIsON;
    }
}
