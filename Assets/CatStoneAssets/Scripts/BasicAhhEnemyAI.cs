using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicAhhEnemyAI : MonoBehaviour
{
    public NavMeshAgent ai;

    //GameManager Object. Used to pull valuable gameobjects and game logic scripts when needed.
    GameObject gameManagerinstance;

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
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, sightDistance, catchDistance, chaseTime, minChaseTime, maxChaseTime;
    public bool walking, chasing, flashlightHit = false;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randNum, randNum2;
    public Vector3 rayCastOffset;
    public string deathScene;

    //Declare audio sources of this BlackSmogEnemy
    [SerializeField]
    [Tooltip("Put the possible audio sounds of the smog monster getting scared and jumpscare here.")]
    public GameObject audio2Scared;

    void Start()
    {
        //Loads the current instance of the game manager into this script for refferencing.
        gameManagerinstance = GameObject.Find("GameManagerObject");

        //Save the player's location when this monster was spawned in.
        player = GameObject.Find("PlayerObject").transform;

        //Save the monster's spawn point transform object.
        monsterSpawnPoint = new GameObject("MonsterSpawnPointObject");
        monsterSpawnPoint.transform.position = this.gameObject.transform.position;

        //Start the monster state as walking
        walking = true;

        //Create the possible destinations.
        CreateRoamingDestinations(destinationAmount, destinationRadiusPatrolSize);
    }

    void Update()
    {
        RaycastHit hit;
        Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.TransformDirection(Vector3.forward*100));
        if (Physics.Raycast(transform.position + rayCastOffset, transform.TransformDirection(Vector3.forward*100), out hit, sightDistance) && !flashlightHit)
        {
            if (hit.collider.tag == "Player")
            {
                Debug.Log(hit.rigidbody.name);
                walking = false;
                StopCoroutine(stayIdle());
                StopCoroutine(chaseRoutine());
                StartCoroutine(chaseRoutine());
                chasing = true;
            }
        }
        if (chasing == true && !flashlightHit)
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
        if(walking == true && !flashlightHit)
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
    
    //The Public method that lets the a monster to react to the flashlight.
    public void LightReaction(){
        StartCoroutine(MonsterReactsToLight());
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

    //FIXME: Currently, it's for the smog monster. Alter it to be usable for ALL monsters.
    //This Coroutine for the smog monster to "hide" for a bit before vanishing (Gets destroyed).
    IEnumerator MonsterReactsToLight(){

        //Set monster to go to a random left/right location local to it's position using a bool random function.
        //Delete the monster after.
        Vector3 MonsterRunsTo;
        MonsterRunsTo = this.transform.forward*-100;
        ai.speed = chaseSpeed;
        ai.destination = MonsterRunsTo;

        flashlightHit = true;
        StopCoroutine("stayIdle");
        StopCoroutine("chaseRoutine");
        //Play a sound to let the player know the monster is going away. Checks if the scene has one playing to prevent audio stacking.
        if(GameObject.Find("SmogMonsterBanished(Clone)")==null){
            Instantiate(audio2Scared,player.transform);
            }

        yield return new WaitForSeconds(3);
        
        Destroy(this.gameObject);
        
    }

    //When this object gets destroyed, destroy all the anchors it has generated to save memory.
    void OnDestroy(){
        foreach(Transform anchorRoamPoint in destinations){
            Destroy(anchorRoamPoint.gameObject);
        }
    }
    


}

