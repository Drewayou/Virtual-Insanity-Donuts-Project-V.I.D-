using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerFlashlightPowerMeterTextGenerator : MonoBehaviour
{

    //Gets the game manager instance.
    GameObject gameManagerInstance;

    //Gets this object's text.
    TMP_Text thisTextObject; 

    // Start is called before the first frame update
    void Start()
    {
        //Get the game manager instance from the scene to pull game scripts from.
        gameManagerInstance = GameObject.Find("GameManagerObject");

        //Gets the text this object the script is attached to has.
        thisTextObject = this.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        thisTextObject.text = "Flashlight Battery : " + (gameManagerInstance.GetComponent<GameManagerScript>().GetPlayerFlashlightBatteryHealth()/gameManagerInstance.GetComponent<GameManagerScript>().GetPlayerFlashlightBatteryHealtMAXIMUM() * 100).ToString("F1") + "%";
    }
}
