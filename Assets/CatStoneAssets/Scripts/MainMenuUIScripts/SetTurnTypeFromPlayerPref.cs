using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class SetTurnTypeFromPlayerPref : MonoBehaviour
{
    public ActionBasedSnapTurnProvider snapTurn;
    public ActionBasedContinuousTurnProvider continuousTurn;

    // Start is called before the first frame update
    void Start()
    {
        ApplyPlayerPref();
    }

    //Applies the player pref when this bject is spawned in.
    public void ApplyPlayerPref()
    {
        if(PlayerPrefs.HasKey("turnType"))
        {
            int value = PlayerPrefs.GetInt("turnType");
            if(value == 0)
            {
                snapTurn.leftHandSnapTurnAction.action.Enable();
                snapTurn.rightHandSnapTurnAction.action.Enable();
                continuousTurn.leftHandTurnAction.action.Disable();
                continuousTurn.rightHandTurnAction.action.Disable();
            }
            else if(value == 1)
            {
                snapTurn.leftHandSnapTurnAction.action.Disable();
                snapTurn.rightHandSnapTurnAction.action.Disable();
                continuousTurn.leftHandTurnAction.action.Enable();
                continuousTurn.rightHandTurnAction.action.Enable();
            }
        }else{
            PlayerPrefs.SetInt("turnType",0);
            ApplyPlayerPref();
        }
    }

    //Saves the player pref and sets the player on the scene to the new updated pref.
    public void SetPlayerPref(){
        
        if(this.gameObject.GetComponent<TMP_Dropdown>().value == 0){
            PlayerPrefs.SetInt("turnType",0);
        }else{
            PlayerPrefs.SetInt("turnType",1);
        }

        //Immediately applys the player pref.
        ApplyPlayerPref();
    }
}
