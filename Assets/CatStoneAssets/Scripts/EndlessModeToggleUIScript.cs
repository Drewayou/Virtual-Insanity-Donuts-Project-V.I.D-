using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessModeToggleUIScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag and drop the scarier scares buttons here!")]
    GameObject EndlessModeOFF, EndlessModeON;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitForLoading());
    }

    //Waits a couple seconds to get player prefs...
    public IEnumerator waitForLoading(){
        yield return new WaitForSeconds(1);
        if(PlayerPrefs.GetInt("endlessMode",0) == 0){
            EndlessModeOFF.SetActive(true);
            EndlessModeON.SetActive(false);
        }else{
            EndlessModeOFF.SetActive(false);
            EndlessModeON.SetActive(true);
        }
    }
}

