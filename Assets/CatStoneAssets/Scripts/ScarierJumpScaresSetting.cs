using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarierJumpScaresSetting : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag and drop the scarier scares buttons here!")]
    GameObject ScarierScaresWarning, ScarierScaresSelected;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitForLoading());
    }

    //Waits a couple seconds to get player prefs...
    public IEnumerator waitForLoading(){
        yield return new WaitForSeconds(1);
        if(PlayerPrefs.GetInt("scarierJumpscares",0) == 0){
            ScarierScaresWarning.SetActive(true);
            ScarierScaresSelected.SetActive(false);
        }else{
            ScarierScaresSelected.SetActive(true);
            ScarierScaresWarning.SetActive(false);
        }
    }
}
