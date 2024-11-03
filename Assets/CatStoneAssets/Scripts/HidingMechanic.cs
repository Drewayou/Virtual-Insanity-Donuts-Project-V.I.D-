using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HidingMechanic : MonoBehaviour
{
    public GameObject hideText, stopHideText;
    public GameObject normalPlayer, hidingPlayer;
    public BasicAhhEnemyAI monsterScript;
    public Transform monsterTransform;
    bool interactable, hiding;
    public float loseDistance;
    
    //GameManager Object, useful for pulling specific scripts and objects for use in other scripts. Automatically set in "Start()".
    private GameObject gameManagerinstance;

//Gets the Input Actions Asset to draw inputs from.
    //Drop the action map "XRI RightHand Interaction"
    [SerializeField]
    [Tooltip("Drag and drop the Input Actions Asset that this script would use.")]
    public InputActionProperty XRIInputActionsAsset;


    void Start()
    {
        interactable = false;
        hiding = false;
        //Instantializes the game manager object.
        gameManagerinstance = GameObject.Find("GameManagerObject");
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && hiding == false)
        {
            other.gameObject.transform.position = this.gameObject.transform.position;
            hiding = true;
            interactable = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            //hideText.SetActive(false);
            interactable = false;
        }
    }
    void Update()
    {
        if (interactable == true)
        {
            if (XRIInputActionsAsset.action.WasPressedThisFrame())
            {
                //hideText.SetActive(false);
                //hidingPlayer.SetActive(true);
                Debug.Log("LEMME OUT");
                float distance = Vector3.Distance(monsterTransform.position, normalPlayer.transform.position);
                if (distance > loseDistance)
                {
                    if (monsterScript.chasing == true)
                    {
                        monsterScript.stopChase();
                    }
                }
                //stopHideText.SetActive(true);
                hiding = true;
                //normalPlayer.SetActive(false);
                interactable = false;
            }
        }
        if (hiding == true)
        {
            if (XRIInputActionsAsset.action.WasPressedThisFrame())
            {
                //stopHideText.SetActive(false);
                //normalPlayer.SetActive(true);
                //hidingPlayer.SetActive(false);
                hiding = false;
                gameManagerinstance.GetComponent<GameManagerScript>().playerObject.transform.position = new Vector3(0, 0, 0);

            }
        }
    }
}
