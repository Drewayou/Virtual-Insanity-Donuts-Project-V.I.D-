using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerControllerScript : MonoBehaviour
{

    //Note, this changes the value in the "Move" object of the XR rig. Specifically, the "Dynamic Move Provider" script!
    [Tooltip("Input your desired player move speed here. (Units/second)")]
    public float playerMoveSpeed = 2f;

    //Note, this changes the value in the "Move" object of the XR rig. Specifically, the "Dynamic Move Provider" script!
    [Tooltip("Input your desired player run speed here. (Units/second)")]
    public float playerRunSpeed = 4f;

    //The ammount of time the player has to make the "Running" motion (Up and down with both controllers), to have the player in game run.
    [Tooltip("Input how often the system checks for differences in motion for this script to trigger runnning (seconds).")]
    public float playerRunningMotionCheckTimer = .3f;

    //A seperate counter to check how long since the motion input was recorded.
    private float motiontimeCounter;

    //A final float to have a max timeout the last motion was recorded and to put back the player to walk mode.
    [Tooltip("Input how long until the player goes back to walking speed from doing the running motion (seconds).")]
    public float playerEaseOutFromRunnningTime = .5f;
    
    //The actual timer of the above ease out time.
    [Tooltip("The actual timer to count how much longer the player sprints for (seconds).")]
    public float playerTimerToEaseFromRunning = 0f;

    //The game object used by OpenXR interaction locomotion for movement. Automaticallly paired on this object Start();
    GameObject openXRMoveLocomotionObject;
    //The script tied to the openXR Move object.
    DynamicMoveProvider openXRMoveProviderScript;

    //The game objects of the Left & Right Controller. Used for determining if the player is "Running" Automaticallly paired on this object Start();
    [SerializeField]
    [Tooltip("Drag the stabilized left an right controller objects here.")]
    GameObject openXRLeftControllerStabilized, openXRRightControllerStabilized;

    //Bools that check if the player is moving the left and right controllers in a running motion (up and down) using the past location at playerRunningMotionCheckTimer.
    public bool playerIsInRunningState = false;

    //floats that save the y position of the controllers (up and down) from the past location at playerRunningMotionCheckTimer.
    public float leftHandPositionLastCheck, rightHandPositionLastCheck;

    //floats that constantly check where the hand positions are via normalized vectors.
    public float leftHandPositionConstantCheck, rightHandPositionConstantCheck;

    // Start is called before the first frame update
    void Start()
    {
        //Find the move speed settings of the player, and set it to this input.
        openXRMoveLocomotionObject = GameObject.Find("Move");
        //Find the atached script to the move component.
        openXRMoveProviderScript = openXRMoveLocomotionObject.GetComponent<DynamicMoveProvider>();

        //Sets the player speed according to the input from above.
        openXRMoveProviderScript.moveSpeed = playerMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //If player is making the runing motion, run.
        if(playerIsMakingRunningMotionsWithHands()){
            SetPlayerMovementToRun();
        }else{
            ResumePlayerNormalMovement();
        }
    }

    //This method checks if the player is constantly moving the left and right controller up & down. If they are, allow the player to move at run speed.
    public bool playerIsMakingRunningMotionsWithHands(){

        //This block of code sets the position of the hands depending on timer frequency.
        if(motiontimeCounter < playerRunningMotionCheckTimer){
            //Increase the time counter to repeatedly gather position points of the hands.
            motiontimeCounter += Time.deltaTime;

            //Constantly set the players hand positions to find if they are swapping.
            if(openXRLeftControllerStabilized.transform.localPosition.normalized.y > 0){
                leftHandPositionConstantCheck = 1;
            }else{
                leftHandPositionConstantCheck = -1;
            }
            if(openXRRightControllerStabilized.transform.localPosition.normalized.y > 0){
                rightHandPositionConstantCheck = 1;
            }else{
                rightHandPositionConstantCheck = -1;
            }
        }else{
            //Once the timer hits, gather position points of the hands.
            //Save the normalized vector of the hands to check if it's in a running position.

            Debug.Log("LeftHandYPosition: " + openXRLeftControllerStabilized.transform.localPosition.normalized.y);
            Debug.Log("RighttHandYPosition: " + openXRRightControllerStabilized.transform.localPosition.normalized.y);

            if(openXRLeftControllerStabilized.transform.localPosition.normalized.y > 0){
                leftHandPositionLastCheck = 1;
            }else{
                leftHandPositionLastCheck = -1;
            }
            if(openXRRightControllerStabilized.transform.localPosition.normalized.y > 0){
                rightHandPositionLastCheck = 1;
            }else{
                rightHandPositionLastCheck = -1;
            }
            motiontimeCounter = 0;

            /*Sets a bool if the polarization of hands have changed (One hand has moved up/down the last check)
            if(){
                areHandsMoving = false;
            }else{
                areHandsMoving = true;
            }
            */
        }
        
        //This block sets the running state. If the hands have polar differences and recently changed.
        if(leftHandPositionLastCheck != leftHandPositionConstantCheck || rightHandPositionLastCheck != rightHandPositionConstantCheck){
            playerIsInRunningState = true;
        }else{
            playerIsInRunningState = false;
        }

        //This block of check if the player is in the running state, if they are, continue resetting the ease out timer.
        if(playerIsInRunningState){
            //Increase the ease out counter if the player didn't make a runnning motion for a while.
            playerTimerToEaseFromRunning = 0;
        }else{
            playerTimerToEaseFromRunning += Time.deltaTime;
        }
        
        //If the player ease out timer hits, stop the player from running.
        if(playerTimerToEaseFromRunning < playerEaseOutFromRunnningTime){
            return true;
        }else{
            return false;
        }
    }

    //---------------------------------------------------------------------------------------
    //Getters & Setters for this script.

    //This public method is used by other scripts to pause the player movement upong death, new zone, and others.
    public void PausePlayerMovement(){
        openXRMoveProviderScript.moveSpeed = 0;
    }

    //This public method is used by other scripts to resume the player movement to normal speeds set before runtime by the developers.
    public void ResumePlayerNormalMovement(){
        openXRMoveProviderScript.moveSpeed = playerMoveSpeed;
    }

    //This public method is used by other scripts to set the player movement to running speeds set before runtime by the developers.
    public void SetPlayerMovementToRun(){
        //Can only run if the player controller isn't set to 0 to inhibit the player from moving in the first place.
        if(openXRMoveProviderScript.moveSpeed != 0){
            openXRMoveProviderScript.moveSpeed = playerRunSpeed;
        }
    }
}
