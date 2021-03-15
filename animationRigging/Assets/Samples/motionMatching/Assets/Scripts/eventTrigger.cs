﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MxM;

public class eventTrigger : MonoBehaviour
{
    private MxMAnimator m_animator;

    public bool sit;

    [SerializeField]
    private MxMEventDefinition m_attackDefinition;

    [SerializeField]
    private MxMEventDefinition m_greetingsDefinition;

    [SerializeField]
    private MxMEventDefinition m_sitDefinition;

    [SerializeField]
    private MxMEventDefinition m_standUpDefinition;

    void Start()
    {
        m_animator = GetComponent<MxMAnimator>();

        sit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            m_animator.BeginEvent(m_attackDefinition);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            m_animator.BeginEvent(m_greetingsDefinition);
        }

        if (Input.GetKeyDown(KeyCode.K) && !sit)
        {
            m_animator.BeginEvent(m_sitDefinition);
            sit = true;
            m_animator.SetRequiredTag("Sitting");
        }

        else if(Input.GetKeyDown(KeyCode.K) && sit)
        {
            m_animator.BeginEvent(m_standUpDefinition);
            sit = false;
            m_animator.SetRequiredTag("Locomotion");

        }

        if (m_animator.IsEventComplete)
        {
            m_animator.ForceExitEvent();
            print("HI");
        }
    }
}
