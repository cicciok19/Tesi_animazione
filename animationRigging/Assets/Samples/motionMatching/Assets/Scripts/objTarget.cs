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
                //set weight head
                var data_h = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
                data_h.SetWeight(0, 0f);
                headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_h;

                //set weight spine
                var data_s = spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
                data_s.SetWeight(0, 0f);
                spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_s;

                this.GetComponent<RigBuilder>().Build();

                print(angle);
                zeroValue = true;
            }
            else if(Mathf.Abs(angle) < 110 && zeroValue)
            {
                var data_h = headAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
                data_h.SetWeight(0, 1f);
                headAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_h;

                var data_s = spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
                data_s.SetWeight(0, 1f);
                spineAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data_s;

                this.GetComponent<RigBuilder>().Build();

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
}
