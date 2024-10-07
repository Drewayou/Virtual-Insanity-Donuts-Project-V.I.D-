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
    public bool walking, chasing;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randNum, randNum2;
    public int destinationAmount;
    public Vector3 rayCastOffset;
     public string deathScene;

    void Start()
    {
        walking = true;
        randNum = Random.Range(0, destinationAmount);
        currentDest = destinations[randNum];
    }

    void Update()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + rayCastOffset, direction, out hit, sightDistance))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                walking = false;
                StopCoroutine("stayIdle");
                StopCoroutine("chaseRoutine");
                StartCoroutine("chaseRoutine");
                chasing = true;
            }
        }
        if (chasing == true)
        {
            dest = player.position;
            ai.destination = dest;
            ai.speed = chaseSpeed;
            aiAnim.ResetTrigger("walk");
            aiAnim.ResetTrigger("idle");
            aiAnim.SetTrigger("sprint");
            float distance = Vector3.Distance(player.position, ai.transform.position);
            if (distance <= catchDistance)
            {
                player.gameObject.SetActive(false);
                aiAnim.ResetTrigger("walk");
                aiAnim.ResetTrigger("idle");
                aiAnim.ResetTrigger("sprint");
                aiAnim.SetTrigger("jumpscare");
                
                chasing = false;
            }
        }
        if(walking == true)
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
                    aiAnim.ResetTrigger("walk");
                    aiAnim.SetTrigger("Idle");
                    ai.speed = 0;
                    StopCoroutine("stayIdle");
                    StartCoroutine("stayIdle");
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


}

