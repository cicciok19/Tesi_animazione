using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MxM;

public class PlayerLogic : MonoBehaviour
{
    [SerializeField] private MxMTrajectoryGenerator trajectory = null;
    [SerializeField] private MxMAnimator animator = null;

    public bool walking;


    void Start()
    {
        walking = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(walking && Input.GetKeyDown(KeyCode.LeftShift))
        {
            walking = false;
            trajectory.MaxSpeed = 4;
        }
        else if(!walking && Input.GetKeyDown(KeyCode.LeftShift))
        {
            walking = true;
            trajectory.MaxSpeed = 2;
        }
    }
}
