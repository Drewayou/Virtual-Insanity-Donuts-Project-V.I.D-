using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicAhhEnemyAI : MonoBehaviour
{
    public NavMeshAgent ai;
    public List <Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, sightDistance, catchDistance, chaseTime, minChaseTime, maxChaseTime;
    public bool walking, chasing, flashlightHit = false;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randNum, randNum2;
    public int destinationAmount;
    public Vector3 rayCastOffset;
    public string deathScene;

    //Declare audio sources of this BlackSmogEnemy
    [SerializeField]
    [Tooltip("Put the possible audio sounds of the smog monster getting scared and jumpscare here.")]
    public GameObject audio2Scared;

    void Start()
    {
        walking = true;
        randNum = Random.Range(0, destinationAmount);
        currentDest = destinations[randNum];

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
            float distance = Vector3.Distance(player.position, ai.transform.position);
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
    
    //The Public method that lets the smog monster to react to the flashlight.
    public void SmogMonsterReactionToLight(){
        StartCoroutine(MonsterShysFromLight());
    }

    //This Coroutine for the smog monster to "hide" for a bit before vanishing (Gets destroyed).
    IEnumerator MonsterShysFromLight(){

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
    


}

