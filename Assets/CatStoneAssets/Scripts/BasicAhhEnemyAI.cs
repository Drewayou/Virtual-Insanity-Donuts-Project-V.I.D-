using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicAhhEnemyAI : MonoBehaviour
{
    public NavMeshAgent ai;

    //AI Selection adjustment. This will alter how this AI reacts to light, sound, ect.
    public enum aISelected{BlackSmogMonster, ShyGuy, AggressiveDog, HippoMonster}
    public aISelected monsterAI;

    //GameManager Object. Used to pull valuable gameobjects and game logic scripts when needed.
    GameObject gameManagerinstance;

    //Save this object's CapsuleCollider collider unto here. Automatically gets set in Start().
    CapsuleCollider thisMonsterCollider;

    //Sets how many points the monster would roam to (If <=2  points, it'll only roam toward the player and it's spawnpoint when spawning).
    [Tooltip("This sets how many roaming points the monster can travel. <=2 will cause the monster to roam from it's spawnpoint -> player")]
    public int destinationAmount = 2;
    //Sets the radius size for the possible roaming points to be (Meters).
    [Tooltip("This sets the radius size for the possible monster roaming points to generate (Meters)")]
    public float destinationRadiusPatrolSize = 2f;
    //The Monster's spawn point set via a transform object.
    public GameObject monsterSpawnPoint;
    //The list that'll be generated for player spawn points.
    public List <Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, sightDistance, catchDistance, chaseTime, minChaseTime, maxChaseTime, dbReactionThreshold;
    public bool walking, chasing, runningAway = false, ranAway = false, soundBasedAI = false, isCollidingWithPlayer = false;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randNum, randNum2;
    public Vector3 rayCastOffset;
    public string deathScene;

    //Declare audio sources of this BlackSmogEnemy
    [SerializeField]
    [Tooltip("Drag and Drop the audio cue(s) the monster plays when it reacts to something.")]
    public List<GameObject> monsterReactionAudio;

    void Start()
    {
        //Loads the current instance of the game manager into this script for refferencing.
        gameManagerinstance = GameObject.Find("GameManagerObject");

        //Save the player's location when this monster was spawned in.
        player = GameObject.Find("PlayerObject").transform;

        //Saves the collider attached to this monster.
        thisMonsterCollider = this.gameObject.GetComponent<CapsuleCollider>();

        //Save the monster's spawn point transform object.
        monsterSpawnPoint = new GameObject("MonsterSpawnPointObject");
        monsterSpawnPoint.transform.position = this.gameObject.transform.position;

        //Loads the AI of this monster to the attached navmesh AI agent.
        ai = this.gameObject.GetComponent<NavMeshAgent>();

        //Start the monster state as walking
        walking = true;

        //Create the possible destinations.
        CreateRoamingDestinations(destinationAmount, destinationRadiusPatrolSize);
    }

    void Update()
    {
        if(soundBasedAI){
            if(dbReactionThreshold<gameManagerinstance.GetComponent<GameManagerScript>().playerInGameDBLoudness){
                SoundReaction();
            }
        }

        RaycastHit hit;
        Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.TransformDirection(Vector3.forward*100));
        if (Physics.Raycast(transform.position + rayCastOffset, transform.TransformDirection(Vector3.forward*100), out hit, sightDistance) && !runningAway)
        {
            //Debug.Log(hit.collider.name);
            if (hit.collider.tag == "Player")
            {
                walking = false;
                StopCoroutine(stayIdle());
                StopCoroutine(chaseRoutine());
                StartCoroutine(chaseRoutine());
                chasing = true;
            }
        }
        if (chasing == true && !runningAway)
        {
            dest = player.GetChild(0).transform.position;
            ai.destination = dest;
            ai.speed = chaseSpeed;
            //aiAnim.ResetTrigger("walk");
            //aiAnim.ResetTrigger("idle");
            //aiAnim.SetTrigger("sprint");
            float distance = Vector3.Distance(player.GetChild(0).transform.position, ai.transform.position);
            if (distance <= catchDistance)
            {
                //FIXME: DO JUMP SCARE
                
                //player.gameObject.SetActive(false);
                //aiAnim.ResetTrigger("walk");
                //aiAnim.ResetTrigger("idle");
                //aiAnim.ResetTrigger("sprint");
                //aiAnim.SetTrigger("jumpscare");
                
                chasing = false;
            }
        }
        if(walking == true && !runningAway)
        {
            dest = currentDest.position;
            ai.destination = dest;
            ai.speed = walkSpeed;
            if(ai.remainingDistance <= ai.stoppingDistance)
            {
                randNum2 = Random.Range(0,2);
                if(randNum2 == 0)
                {
                    randNum = Random.Range(0, destinationAmount);
                    currentDest = destinations[randNum];
                }
                if(randNum2 == 1)
                {
                    //aiAnim.ResetTrigger("walk");
                    //aiAnim.SetTrigger("Idle");
                    ai.speed = 0;
                    StopCoroutine(stayIdle());
                    StartCoroutine(stayIdle());
                    walking = false;
                }
            }
        }

        if(runningAway)
        {
            StopCoroutine(stayIdle());
            StopCoroutine(chaseRoutine());
            chaseTime = 0;
            idleTime = 0;
            if(ranAway == false){
                walking = false;
                chasing = false;
                if(this.gameObject != null){
                Vector3 MonsterRunsTo;
                MonsterRunsTo = this.transform.forward*-100;
                ai.speed = chaseSpeed;
                ai.destination = MonsterRunsTo;
                ranAway = true;
                }
            }
        }

        IEnumerator stayIdle()
        {
            idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleTime);
            walking = true;
            randNum = Random.Range(0, destinations.Count);
            currentDest = destinations[randNum];
        }
        IEnumerator chaseRoutine()
        {
            chaseTime = Random.Range(minChaseTime, maxChaseTime);
            yield return new WaitForSeconds(chaseTime);
            walking = true;
            chasing = false;
            randNum = Random.Range(0, destinations.Count);
            currentDest = destinations[randNum];
        }
    }

    public void stopChase(){
            walking = true;
            chasing = false;
            StopCoroutine("chaseRoutine");
            randNum = Random.Range(0, destinationAmount);
            currentDest = destinations[randNum];
        }

    
    //The Public method that lets the a monster to react to the flashlight.
    public void LightReaction(){
        StartCoroutine(MonsterReaction());
    }

    //The Public method that lets the a monster to react to sound.
    public void SoundReaction(){
        StartCoroutine(MonsterReaction());
    }

    //This method creates the distinations for this monster AI to roam to.
    public void CreateRoamingDestinations(int destinationAmountToGenerate, float destinationRadiusPatrolSizeToGenerate){

        //If the roaming destinations are <=2, make the monster only roam to/from player position when this monster was spawned.
            for(int i = destinationAmountToGenerate; i>0; i--){

                //Instantiate an empty game anchor for this roaming mechanic.
                GameObject generatedRoamingAnchor;
                generatedRoamingAnchor = new GameObject(this.name + "SpawnAnchor: " + i);

                if(i>2){
                    //Get a random point in a unit circle to spawn the monster.
                    Vector2 newMonsterRoamingRadiusPoint = Random.insideUnitCircle * destinationRadiusPatrolSizeToGenerate;

                    //Debug.Log(newMonsterRoamingRadiusPoint.x +"|"+ newMonsterRoamingRadiusPoint.y);
                    Vector3 monsterRoamingPosition = new Vector3 (newMonsterRoamingRadiusPoint.x + monsterSpawnPoint.transform.position.x, 0, newMonsterRoamingRadiusPoint.y + monsterSpawnPoint.transform.position.z);
                    
                    //Spawns the anchors into the transform objects of the enemy spawnpoint.
                    generatedRoamingAnchor.transform.position = monsterRoamingPosition;
                    destinations.Add(generatedRoamingAnchor.transform);
                }

                if(i==2){
                    //Spawns the player anchor for where the monster needs to find the player.
                    generatedRoamingAnchor.transform.position = player.transform.position;
                    destinations.Add(generatedRoamingAnchor.transform);
                }

                if(i==1){
                    //Saves the monster spawnpoint anchor for where the monster needs to find the player.
                    generatedRoamingAnchor.transform.position = monsterSpawnPoint.transform.position;
                    destinations.Add(generatedRoamingAnchor.transform);
                }

                //Sets the parent of these anchors as the "PlayableAreaobject". 
                //The rationale is to organize game hierarchy for readability and easy deletion of the anchor points.
                //(NOTE: PlayableAreaGameObject holds these anchors and can be pulled from the GameObjectManagerScript)
                generatedRoamingAnchor.transform.SetParent(gameManagerinstance.GetComponent<GameManagerScript>().playableAreaGameObject.transform,true);
            }
        //Randomly set a destination for this monster to roam.
        randNum = Random.Range(0, destinationAmount);
        currentDest = destinations[randNum];
    }

    //FIXME: Currently, it's for the smog monster. Alter it to be usable for ALL monsters (In Progress).
    //This Coroutine for the smog monster to "hide" for a bit before vanishing (Gets destroyed).
    IEnumerator MonsterReaction(){

        //FIXME: Make Cases in the switch statement below to check what monster reaction this AI must do.
        /****************************************************************************************************************************************************************
        NOTE: BELOW SWITCH STATEMENT IS CRUCIAL TO ALTERING THE AI OF DIFFERENT MONSTERS.
        *****************************************************************************************************************************************************************/

        switch(monsterAI.ToString()){
            /***
            BLACK SMOG MONSTER CASE
            ***/
            case "BlackSmogMonster":
                //Set monster to run away from whatever triggered it (This case, the flashlight).
                //Delete the monster after.
                walking = false;
                chasing = false;
                runningAway = true;
                StopCoroutine("stayIdle");
                StopCoroutine("chaseRoutine");
                ai.destination = this.transform.forward*-100;
                
                //Play a sound to let the player know the monster is going away. Checks if the scene has one playing to prevent audio stacking.
                if(GameObject.Find("BlackSmogMonsterBanished(Clone)")==null){
                    Instantiate(monsterReactionAudio[0],player.transform);
                    }

                yield return new WaitForSeconds(5);
                    Destroy(this.gameObject);
                break;

            /***
            SHY GUY MONSTER CASE
            ***/
            case "ShyGuy":
                //Set monster to chase mode when triggered(This case, the flashlight triggers this action).
                //Set monster to chase mode.
                StopCoroutine("stayIdle");
                StopCoroutine("chaseRoutine");
                
                chasing = true;
                ai.speed = chaseSpeed;
                walking = false;
                //Play a sound to let the player know the monster is chasing the player. Checks if the scene has one playing to prevent audio stacking.
                if(GameObject.Find("ShyGuyGotSpotterSFX(Clone)")==null){
                    Instantiate(monsterReactionAudio[0],player.transform);
                    }
                //Stop The Shy Guy From crying and instead instantiate a scream/different audio sfx on them. Checks if there was a prefab loaded.
                this.gameObject.GetComponent<AudioSource>().Stop();
                if(monsterReactionAudio.Count <2 || monsterReactionAudio[1] == null){
                    Debug.LogError("ERROR | PREFAB SFX MISSING : DRAG ANOTHER AUDIO SFX PREFAB TO PLAY WHEN THE SHY GUY IS AGGRO!");
                }else{
                    if(GameObject.Find("ShyGuyMonsterChasePlayerSFX(Clone)")==null){
                    Instantiate(monsterReactionAudio[1],this.gameObject.transform);
                    }
                }
                break;
            /***
            AGGRESSIVE DOG MONSTER CASE
            ***/
            case "AggressiveDog":
                //Set monster to run away from whatever triggered it (This case, the player making loud noise).
                //Delete the monster after.
                walking = false;
                chasing = false;
                runningAway = true;
                StopCoroutine("stayIdle");
                StopCoroutine("chaseRoutine");
                ai.speed = 0;
    
                //Stops the normal Dgog Monster SFX.
                this.gameObject.GetComponent<AudioSource>().Stop();
                
                
                //Play a sound to let the player know the Dog Monster is going away. Checks if the scene has one playing to prevent audio stacking.
                if(GameObject.Find("AggressiveDogScared(Clone)")==null){
                    Instantiate(monsterReactionAudio[0],player.gameObject.transform);
                    }

                Destroy(this.gameObject);
                yield return new WaitForSeconds(1);
                    
                break;
            /***
            HIPPO MONSTER CASE 
            ***/
            case "HippoMonster":
                //Set monster to chase mode when triggered (This case, the player making too much noise).
                //Set monster to chase mode.
                StopCoroutine("stayIdle");
                StopCoroutine("chaseRoutine");
                chasing = true;
                walking = false;
                ai.speed = chaseSpeed;
                ai.speed = chaseSpeed;
                ai.speed = chaseSpeed;
                ai.speed = chaseSpeed;
                ai.speed = chaseSpeed;
                
                //Play a sound to let the player know the monster has started to chase the player. Checks if the scene has one playing to prevent audio stacking.
                if(GameObject.Find("HippoThumperGotMAD(Clone)")==null){
                    Instantiate(monsterReactionAudio[0],player.transform);
                    }
                //Spawn the angry hippo sound on the monster. Checks if there was a prefab loaded. Stops the normal hippo SFX too.
                this.gameObject.GetComponent<AudioSource>().Stop();
                if(monsterReactionAudio.Count <2 || monsterReactionAudio[1] == null){
                    Debug.LogError("ERROR | PREFAB SFX MISSING : DRAG ANOTHER AUDIO SFX PREFAB TO PLAY WHEN THE HIPPO AGGRO!");
                }else{
                    if(GameObject.Find("HippoThumperChasingThePlayer(Clone)")==null){
                    Instantiate(monsterReactionAudio[1],this.gameObject.transform);
                    }
                }
                break;
        }
    }

    //If the player touches this enemy and this enemy has a CapsuleCollider on it, trigger a sanity drop and a jumpscare. Player replays zone too.
    void OnTriggerEnter(Collider colliderThatTouchesThisTrigger){
        Debug.Log("PlayerTouchedAMonster!");
        if(isCollidingWithPlayer){
            return;
            }
        isCollidingWithPlayer = true;
        gameManagerinstance.GetComponent<GameManagerScript>().nextMonsterJumpscareAtPlayer = monsterAI.ToString();
        gameManagerinstance.GetComponent<GameManagerScript>().PlayerReplaysZoneDueToMonster();
    }

    //When this object gets destroyed, destroy all the anchors and spawn points it has generated to save memory.
    void OnDestroy(){
        foreach(Transform anchorRoamPoint in destinations){
            Destroy(anchorRoamPoint.gameObject);
        }
        Destroy(monsterSpawnPoint);
    }
    


}

