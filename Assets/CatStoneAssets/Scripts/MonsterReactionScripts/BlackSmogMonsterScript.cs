using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlackSmogMonsterScript : MonoBehaviour
{

    //Get this enemy controller script.
    EnemyControllerScript thisEnemyController;

    SphereCollider thisEnemyBody;

    //The Game object for the player.
    GameObject player;

    //Declare audio sources of this BlackSmogEnemy
    [SerializeField]
    [Tooltip("Put the possible audio sounds of the smog monster here.")]
    public GameObject audio1Groan, audio2Scared;

    //How long this monster gets banished for.
    public float monsterBanishTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        //Find the player gameobject.
        player = GameObject.Find("PlayerObject");
        
        //Get this enemy controller attached to this game object.
        thisEnemyController = this.gameObject.GetComponent<EnemyControllerScript>();

        //Play the first audio.
        audio1Groan.GetComponent<AudioSource>().Play(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //A method to play the black smog's audios and send it back to it's spawn point.
    public void BlackSmogGotScared(){

        //Play the second audio (The one where the fog is scared).
        audio2Scared.GetComponent<AudioSource>().Play(0);

        //Have monster "hide" from the light
        StartCoroutine(MonsterShysFromLight());

    }

    //This Coroutine lets the monster "hide" for a bit before vanishing.
    IEnumerator MonsterShysFromLight(){
        //Play a sound to let the player know the monster is going away. Checks if the scene has one playing to prevent audio stacking.
        if(GameObject.Find("SmogMonsterBanished(Clone)")==null){
            Instantiate(audio2Scared,player.transform);
            }

        //Set monster to go to a random left/right location local to it's position using a bool random function.
        //Temp position the monster will run to.
        Vector3 MonsterRunsTo;
        int tempBoolRandom = Random.Range(0,10);
        if(tempBoolRandom >= 5){
            MonsterRunsTo = gameObject.transform.right*100;
        }else{MonsterRunsTo = -gameObject.transform.right*100;
        }

        
        thisEnemyController.SetPlayerLastSeenLocation(MonsterRunsTo);
        thisEnemyController.ChangeMonsterSpeed(60);
        thisEnemyController.ChangeMonsterAcceleration(200);
        yield return new WaitForSeconds(3);
        thisEnemyController.ResetMonsterSpeed();
        thisEnemyController.ResetMonsterAcceleration();
        //Deactivate and reactivate the monster for n seconds.
        StartCoroutine(BanishedMonsterTimeOut());
    }

    //This Coroutine waits for n seconds to banish (de-activate) the monster.
    IEnumerator BanishedMonsterTimeOut(){

        //Get the objects inside the enemy prefab and hide them. Pause the enemy movement.
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        thisEnemyController.PauseEnemyMovement();

        //Pull the controller script also attached to this enemy object, and set this enemy's back to it's spawn position.
        //Added a random unit circle to change a bit where it spawns, note values <20 are CRAZY but it tends stay coming from one direction.
        Vector2 randomPosition = Random.insideUnitCircle * 3;
        this.gameObject.transform.position = new Vector3(thisEnemyController.GetEnemyStartingPosition().x + randomPosition.x, thisEnemyController.GetEnemyStartingPosition().y, thisEnemyController.GetEnemyStartingPosition().z + randomPosition.y);
        
        //Wait for n seconds.
        yield return new WaitForSeconds(monsterBanishTime);

        //Reactivate the enemy, and reset it's known player position.
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        thisEnemyController.ContinueEnemyMovement();
        thisEnemyController.SetPlayerLastSeenLocation(player.transform.position);
    }
}
