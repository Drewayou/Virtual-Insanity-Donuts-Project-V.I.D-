using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownDescNMoveUIScript : MonoBehaviour
{
    //The duriation for the UI pop-up to appear/disappear via movement.
    [Tooltip("Input the speed in (s) for how fast the UI moves.")]
    public float uiMoveDuration = 2f;

    //The game objects that the player clicks for more info/to start the game.
    [SerializeField]
    [Tooltip("Drag and drop the co-responding gameobjects.")]
    GameObject easyDifficultySelectionPopup, normalDifficultySelectionPopup, hardDifficultySelectionPopup;

    public bool selectedUIIsShowing, UIHasAlreadyMovedUp, playerClickedASetting, playerSelectedEasySetting, playerSelectedNormalSetting, playerSelectedHardSetting;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // The method that gets called to set what difficulty the player has clicked. 
    // (STARTING FROM 0=EASY)
    public void PlayerSelectedADifficulty(int difficultySelected){
        if(!playerClickedASetting){
            switch(difficultySelected){
                case 0:
                    easyDifficultySelectionPopup.SetActive(true);
                    normalDifficultySelectionPopup.SetActive(false);
                    hardDifficultySelectionPopup.SetActive(false);
                    PlayerPrefs.SetString("difficulty", "easy");
                    ScanAndSetDifficultySelectionOfThisUI(difficultySelected);
                    ChooseToMoveUIUpOrNot(selectedUIIsShowing);
                break;
                
                case 1:
                    easyDifficultySelectionPopup.SetActive(false);
                    normalDifficultySelectionPopup.SetActive(true);
                    hardDifficultySelectionPopup.SetActive(false);
                    PlayerPrefs.SetString("difficulty", "normal");
                    ScanAndSetDifficultySelectionOfThisUI(difficultySelected);
                    ChooseToMoveUIUpOrNot(selectedUIIsShowing);
                break;

                case 2:
                    easyDifficultySelectionPopup.SetActive(false);
                    normalDifficultySelectionPopup.SetActive(false);
                    hardDifficultySelectionPopup.SetActive(true);
                    PlayerPrefs.SetString("difficulty", "hard");
                    ScanAndSetDifficultySelectionOfThisUI(difficultySelected);
                    ChooseToMoveUIUpOrNot(selectedUIIsShowing);
                break;
            }
        }
    }

    //A method that checks if another setting was selected, if false, hides the UI.
    public void ChooseToMoveUIUpOrNot(bool UINeedsToGoUpOrNot){
        if(UINeedsToGoUpOrNot && !UIHasAlreadyMovedUp){
            StartCoroutine(MoveUIUp(uiMoveDuration));
            UIHasAlreadyMovedUp = true;
        }
         
    }

    //A method used to clear var for which difficulty was selected. DOES NOT clear player prefs.
    public void ScanAndSetDifficultySelectionOfThisUI(int ChosenUIDifficulty){

        switch(ChosenUIDifficulty){
                case 0:
                    if(playerSelectedEasySetting){
                        playerSelectedEasySetting = false;
                        selectedUIIsShowing = false;
                        StartCoroutine(MoveUIDown(uiMoveDuration));
                        easyDifficultySelectionPopup.SetActive(false);
                    }else{
                        playerSelectedEasySetting = true;
                        playerSelectedNormalSetting = false; 
                        playerSelectedHardSetting = false;
                        selectedUIIsShowing = true;
                    }
                break;
                
                case 1:
                    if(playerSelectedNormalSetting){
                        playerSelectedNormalSetting = false;
                        selectedUIIsShowing = false;
                        StartCoroutine(MoveUIDown(uiMoveDuration));
                        normalDifficultySelectionPopup.SetActive(false);
                    }else{
                        playerSelectedEasySetting = false;
                        playerSelectedNormalSetting = true; 
                        playerSelectedHardSetting = false;
                        selectedUIIsShowing = true;
                    }
                break;

                case 2:
                    if(playerSelectedHardSetting){
                        playerSelectedHardSetting = false;
                        selectedUIIsShowing = false;
                        StartCoroutine(MoveUIDown(uiMoveDuration));
                        hardDifficultySelectionPopup.SetActive(false);
                    }else{
                        playerSelectedEasySetting = false;
                        playerSelectedNormalSetting = false; 
                        playerSelectedHardSetting = true;
                        selectedUIIsShowing = true;
                    }
                break;

                default:
                    if(playerSelectedEasySetting){
                        playerSelectedEasySetting = false;
                        selectedUIIsShowing = false;
                        StartCoroutine(MoveUIDown(uiMoveDuration));
                    }else{
                        playerSelectedEasySetting = true;
                        playerSelectedNormalSetting = false; 
                        playerSelectedHardSetting = false;
                        selectedUIIsShowing = true;
                    }
                break;
            }
    }

    //Move the UI depending on if the player has clicked on the selected difficulty button.
    public void MoveUIUpNEnablePopup(){
        if(!playerClickedASetting){
            playerClickedASetting = true;
            if(!selectedUIIsShowing){
                StartCoroutine(MoveUIUp(uiMoveDuration));
                selectedUIIsShowing = true;
            }else{
                StartCoroutine(MoveUIDown(uiMoveDuration));
                selectedUIIsShowing = false;
            }
        }
    }

    //Example of LERP documentation can be found here: https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/#how_to_use_lerp_in_unity
    IEnumerator MoveUIDown(float duration)
    {
        UIHasAlreadyMovedUp = false;
        float processTime = 0;
        Vector3 startPosition = new Vector3(0f, 2.4f, 2.783f);
        Vector3 endPosition = new Vector3(0f, 1.4f, 2.783f);

        while (processTime < duration)
        {
            this.gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, processTime / duration);
            processTime += Time.deltaTime;
            yield return null;
        }
        this.gameObject.transform.position = endPosition;
        playerClickedASetting = false;
    }

    IEnumerator MoveUIUp(float duration)
    {
        float processTime = 0;
        Vector3 startPosition = new Vector3(0f, 1.4f, 2.783f);
        Vector3 endPosition = new Vector3(0f, 2.4f, 2.783f);

        while (processTime < duration)
        {
            this.gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, processTime / duration);
            processTime += Time.deltaTime;
            yield return null;
        }
        this.gameObject.transform.position = endPosition;
        playerClickedASetting = false;
    }
}
