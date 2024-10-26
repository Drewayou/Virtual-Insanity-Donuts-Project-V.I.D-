using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSoundMeterGenerator : MonoBehaviour
{

    //Gets the game manager instance.
    GameObject gameManagerInstance;

    //Gets this object's text.
    [SerializeField]
    [Tooltip("Drag and drop the \"SoundMeterText\" object here.")]
    TMP_Text thisTextObject; 

    //Gets the sound meter fill bar.
    [SerializeField]
    [Tooltip("Drag and drop the \"SoundMeterFilled\" object here.")]
    Image soundMeterFillBar; 

    // Start is called before the first frame update
    void Start()
    {
        //Get the game manager instance from the scene to pull game scripts from.
        gameManagerInstance = GameObject.Find("GameManagerObject");

    }

    // Update is called once per frame
    void Update()
    {
        thisTextObject.text = "Sound(In-GameDB) : " + gameManagerInstance.GetComponent<GameManagerScript>().playerInGameDBLoudness.ToString("F1") + "DB";
        soundMeterFillBar.fillAmount = gameManagerInstance.GetComponent<GameManagerScript>().playerInGameDBLoudness / 85f;
    }
}
