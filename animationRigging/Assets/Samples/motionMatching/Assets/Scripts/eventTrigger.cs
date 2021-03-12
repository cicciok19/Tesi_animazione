using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MxM;

public class eventTrigger : MonoBehaviour
{
    private MxMAnimator m_animator;

    [SerializeField]
    private MxMEventDefinition m_attackDefinition;
    [SerializeField]
    private MxMEventDefinition m_greetingsDefinition;
    [SerializeField]
    private MxMEventDefinition m_pickDefinition;
    [SerializeField]
    private Transform objectToPick;

    void Start()
    {
        m_animator = GetComponent<MxMAnimator>();
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

        if (Input.GetKeyDown(KeyCode.Y))
        {
            m_pickDefinition.ClearContacts();
            m_pickDefinition.AddEventContact(objectToPick);
            m_animator.BeginEvent(m_pickDefinition);
        }

    }
}
