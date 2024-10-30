using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.InputSystem;
using UnityEngine;

public class MenuUISelector : MonoBehaviour
{

    //Gets the Input Actions Asset to draw inputs from.
    //Drop the action map "XRI RightHand Interaction"
    [SerializeField]
    [Tooltip("Drag and drop the Input Actions Asset that this script would use.")]
    public InputActionProperty XRIInputActionsAsset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Gets the XRIInputActionsAsset input map and checks if the specific input is used.
        if(XRIInputActionsAsset.action.WasPressedThisFrame()){
            
        }
    }
}
    
