using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestroy : MonoBehaviour
{
    //Input field for how long you want this object to last.
    [SerializeField]
    [Tooltip("Set the time counter for how long you want this object to last (seconds).")]
    public float timeToSelfDestroy = 5f;
    
    // Update is called once per frame. Auto-destroy the object this script is attached to depending on input seconds above.
    void Update () {
        if (timeToSelfDestroy > 0f) {
            timeToSelfDestroy -= Time.deltaTime;
        } else {
            Destroy(this.gameObject);
        }
    }
}
