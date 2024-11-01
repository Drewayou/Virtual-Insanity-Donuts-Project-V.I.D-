using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class MicSettingScript : MonoBehaviour
{
    GameManagerScript thisGameManagerScriptInstance;

    [SerializeField]
    [Tooltip("Drag and drop the dropdown object for the selectable microphones.")]
    TMP_Dropdown micOptionsDropdown;

    [SerializeField]
    [Tooltip("Drag and drop the ScrollBar object for the selectable mic input multipler.")]
    Scrollbar micInputMultiplier;

    [SerializeField]
    [Tooltip("Drag and drop the TMP object for what the multipler is set to.")]
    TMP_Text micInputMultiplierText;

    //Sets the input multiplier for the mic. Default is 0.5 and every .1 change amplifies the db by x10!
    public float setMicInputVolume;

    // Start is called before the first frame update
    void Start()
    {
        thisGameManagerScriptInstance = GameObject.Find("GameManagerObject").GetComponent<GameManagerScript>();

        micOptionsDropdown.options.Clear();

            int microphoneIndex = 0;
            foreach (var device in Microphone.devices)
            {
                micOptionsDropdown.options.Add (new TMP_Dropdown.OptionData(){text="Microphone " + (microphoneIndex + 1) + " | Is named : " + device});
                microphoneIndex +=1;
            }
        
        micOptionsDropdown.value = thisGameManagerScriptInstance.microphoneInputIndexSelected -1;

        micInputMultiplier.value = thisGameManagerScriptInstance.playerMicSensitivitySetting;

        micInputMultiplierText.text = " Mic Input = " + thisGameManagerScriptInstance.playerMicSensitivitySetting;
    }

    // Update is called once per frame
    void Update()
    {
        micInputMultiplierText.text = " Mic Input = " + micInputMultiplier.value.ToString("F1");
    }

    // Used by the mic selection menu to change the mic input raw data multiplier.
    public void UpdateMicInputSensitivity(){
        PlayerPrefs.SetFloat("micInputMultiplier", micInputMultiplier.value);
        thisGameManagerScriptInstance.playerMicSensitivitySetting = micInputMultiplier.value;
    }

    // Used by the mic selection menu to change the mic input.
    public void UpdateMicSelection(){
        PlayerPrefs.SetInt("micInputSelected", micOptionsDropdown.value);
        thisGameManagerScriptInstance.microphoneInputIndexSelected = micOptionsDropdown.value + 1;
        thisGameManagerScriptInstance.StartAndScanMicInstance();
    }

    // Used by the mic selection menu to assure player prefs saves.
    public void SavePlayerPrefChanges(){
        PlayerPrefs.Save();
    }
}
