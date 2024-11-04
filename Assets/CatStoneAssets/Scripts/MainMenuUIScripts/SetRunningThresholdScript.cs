using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetRunningThresholdScript : MonoBehaviour
{

    GameManagerScript thisGameManagerScriptInstance;

    // Start is called before the first frame update
    void Start()
    {
        thisGameManagerScriptInstance = GameObject.Find("GameManagerObject").GetComponent<GameManagerScript>();
        this.gameObject.GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat("runningThreshold",0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRunningThreshold(){
        thisGameManagerScriptInstance.playerController.handRunningThreshold = this.gameObject.GetComponent<Scrollbar>().value;
        PlayerPrefs.SetFloat("runningThreshold", this.gameObject.GetComponent<Scrollbar>().value);
    }
}
