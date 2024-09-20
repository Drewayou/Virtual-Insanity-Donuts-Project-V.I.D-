using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class FogEmiterScript : MonoBehaviour
{

    //Fog color objects.
    [SerializeField]
    [Tooltip("Drop in the fog colors in here.")]
    private GameObject WhiteFogObject, GreyFogObject, BlackFogObject, RedFogObject;

    [Serializable] 
    public enum SelectedFogColor
    {White = 0, Grey = 1, Black = 2, Red = 3, NoFog = 4}

    public SelectedFogColor selectedFogSet;

    public enum SelectedFogHeaviness
    {Light = 0, Heavy = 1, FullSmoke = 2}

    public SelectedFogHeaviness selectedFogHeaviness;

    //Private variables to set the fog to the selected conditions.
    private int selectedFogSetIndex; 

    private Vector3 fogPosition;

    // Start is called before the first frame update.
    void Start()
    {
        //Set fogs depending on OBJECT hierarchy.
        WhiteFogObject = gameObject.transform.GetChild(0).gameObject;
        GreyFogObject = gameObject.transform.GetChild(1).gameObject;
        BlackFogObject = gameObject.transform.GetChild(2).gameObject;
        RedFogObject = gameObject.transform.GetChild(3).gameObject;
        
    }

    // Update is called once per frame.
    void Update()
    {
        SetFogParameters();
    }

    //Helper Method to clean up code.
    public void SetFogParameters(){
        SetFogColorSelected();
        SetFogHeaviness();
    }

    //Sets the fog heavy index and how heavy the fog is.
    public void SetFogHeaviness(){
        
        //If No fog wasn't selected, set the fog thickness.
        if(selectedFogSetIndex!=4){
            switch(selectedFogHeaviness){
                case SelectedFogHeaviness.Light:
                fogPosition = new Vector3(0f,-3.38f,0f);
                break;
                case SelectedFogHeaviness.Heavy:
                fogPosition = new Vector3(0f,0.23f,0f);
                break;
                case SelectedFogHeaviness.FullSmoke:
                fogPosition = new Vector3(0f,2.18f,0f);
                break;
            }
            gameObject.transform.GetChild(selectedFogSetIndex).transform.position = fogPosition;
        }
    }

    public void SetFogColorSelected(){
        //Activate only the co-respondinng fog selected.
        switch (selectedFogSet) {
            case SelectedFogColor.White:

            WhiteFogObject.SetActive(true);
            GreyFogObject.SetActive(false);
            BlackFogObject.SetActive(false);
            RedFogObject.SetActive(false);

            selectedFogSetIndex = 0;

            break;
            
            case SelectedFogColor.Grey:

            WhiteFogObject.SetActive(false);
            GreyFogObject.SetActive(true);
            BlackFogObject.SetActive(false);
            RedFogObject.SetActive(false);

            selectedFogSetIndex = 1;

            break;

            case SelectedFogColor.Black:

            WhiteFogObject.SetActive(false);
            GreyFogObject.SetActive(false);
            BlackFogObject.SetActive(true);
            RedFogObject.SetActive(false);

            selectedFogSetIndex = 2;

            break;

            case SelectedFogColor.Red:

            WhiteFogObject.SetActive(false);
            GreyFogObject.SetActive(false);
            BlackFogObject.SetActive(false);
            RedFogObject.SetActive(true);

            selectedFogSetIndex = 3;

            break;

            case SelectedFogColor.NoFog:

            WhiteFogObject.SetActive(false);
            GreyFogObject.SetActive(false);
            BlackFogObject.SetActive(false);
            RedFogObject.SetActive(false);

            selectedFogSetIndex = 4;
            break;
        }
    }
}
