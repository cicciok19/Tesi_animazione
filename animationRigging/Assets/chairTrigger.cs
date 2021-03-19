using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chairTrigger : MonoBehaviour
{

    // Update is called once per frame
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "AI")
            collider.gameObject.GetComponent<AI_mover>().chair = true;
    }
}
