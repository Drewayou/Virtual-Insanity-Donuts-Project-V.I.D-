using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThresholdChecker : MonoBehaviour
{

    GameManagerScript thisGameManagerScriptInstance;

    [SerializeField]
    [Tooltip("Drag the UI Boxes to show the hand positiions and running state of the player.")]
    GameObject CheckBoxRunningChecked, CheckBoxLeftABOVEThreshold, CheckBoxRightBELOWThreshold, CheckBoxCrouching;

    // Start is called before the first frame update
    void Start()
    {
        thisGameManagerScriptInstance = GameObject.Find("GameManagerObject").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    // The methods in this update simply allow the player to see if their hands are in the right positions and if they are running.
    void Update()
    {
        if(thisGameManagerScriptInstance.playerController.playerIsInRunningState){
            CheckBoxRunningChecked.SetActive(true);
        }else{
            CheckBoxRunningChecked.SetActive(false);
        }

        if(thisGameManagerScriptInstance.playerController.leftHandPositionConstantCheck > thisGameManagerScriptInstance.playerController.handRunningThreshold){
            CheckBoxLeftABOVEThreshold.SetActive(true);
        }else{
            CheckBoxLeftABOVEThreshold.SetActive(false);
        }

        if(thisGameManagerScriptInstance.playerController.rightHandPositionConstantCheck < thisGameManagerScriptInstance.playerController.handRunningThreshold){
            CheckBoxRightBELOWThreshold.SetActive(true);
        }else{
            CheckBoxRightBELOWThreshold.SetActive(false);
        }

        if(thisGameManagerScriptInstance.playerController.playerIsCrouching){
            CheckBoxCrouching.SetActive(true);
        }else{
            CheckBoxCrouching.SetActive(false);
        }
    }

}
