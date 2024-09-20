using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerSettingsScript : MonoBehaviour
{

    //Note, this changes the value in the "Move" object of the XR rig. Specifically, the "Dynamic Move Provider" script!
    [Tooltip("Input your desired player move speed here.")]
    public float playerMoveSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        //Find the move speed settings of the player, and set it to this input.
        GameObject.Find("Move").GetComponent<DynamicMoveProvider>().moveSpeed = playerMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
