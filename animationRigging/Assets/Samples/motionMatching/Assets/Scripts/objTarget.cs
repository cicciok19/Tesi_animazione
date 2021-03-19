using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class objTarget : MonoBehaviour
{
    public GameObject target;

    private GameObject rig;
    private GameObject headAim;
    private GameObject spineAim;

    float angle;
    float a;

    bool checkAngle;
    bool zeroValue;
    bool coroutine;

    void Start()
    {
        rig = this.transform.Find("Rig").gameObject;
        headAim = rig.transform.Find("HeadAim").gameObject;
        spineAim= rig.transform.Find("SpineAim").gameObject;

        //weight head
        var data_h = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
        data_h.SetWeight(0,1f);
        headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_h;

        //weight spine
        var data_s = spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
        data_s.SetWeight(0, 1f);
        spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_s;

        this.GetComponent<RigBuilder>().Build();

        checkAngle = true;
        zeroValue = false;
        coroutine = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkAngle)
        {
            checkAngle = false;
            angle = Vector3.Angle(this.transform.forward, target.transform.position - this.transform.position);
            if (Mathf.Abs(angle) > 110 && !zeroValue)
            {
                if (!coroutine)
                {
                    coroutine = true;
                    StartCoroutine(setWeightToZero());
                }

                zeroValue = true;
            }
            else if(Mathf.Abs(angle) < 110 && zeroValue)
            {
                if (!coroutine) {
                    coroutine = true;
                    StartCoroutine(setWeightToOne()); 
                }

                zeroValue = false;
            }
            checkAngle = true;
        }
    }

    //per switchare target: 
    //index sta per l'indice in base all'array di target
    //target è l'oggetto
    public void SetTarget(int index, GameObject target)
    {
        var data_h = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
        data_h.SetTransform(index, target.transform);
        headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_h;

        var data_s = spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
        data_s.SetTransform(index, target.transform);
        spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_s;

        this.GetComponent<RigBuilder>().Build();
    }

    IEnumerator setWeightToZero()
    {
        float w = 1f;

        while (w > 0)
        {
            w -= .01f;
            //set weight head
            var data_h = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
            data_h.SetWeight(0, w);
            headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_h;

            //set weight spine
            var data_s = spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
            data_s.SetWeight(0, w);
            spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_s;

            //this.GetComponent<RigBuilder>().Build();

            yield return new WaitForSeconds(.0001f);
        }

        this.GetComponent<RigBuilder>().Build();
        coroutine = false;
    }

    IEnumerator setWeightToOne()
    {
        float w = 0f;

        while (w < 1f)
        {
            w += .01f;
            //set weight head
            var data_h = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
            data_h.SetWeight(0, w);
            headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_h;

            //set weight spine
            var data_s = spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
            data_s.SetWeight(0, w);
            spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_s;

            yield return new WaitForSeconds(.0001f);
        }

        this.GetComponent<RigBuilder>().Build();
        coroutine = false;
    }
}
