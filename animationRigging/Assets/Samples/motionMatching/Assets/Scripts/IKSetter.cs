using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;


//principal class for setting the Final IK
public class IKSetter : MonoBehaviour
{
    /// <summary>
    /// creating the HeadIK of the gameObject, it may be specified the bones of the head and the neck
    /// </summary>
    /// <param name="headBone"></param>
    /// <returns></returns>
    protected AimIK SetIKHead(Transform headBone, Transform neckBone)
    {
        AimIK HeadIK = this.gameObject.AddComponent<AimIK>();
        HeadIK.solver.transform = headBone;
        HeadIK.solver.axis = HeadIK.solver.transform.position.normalized;
        HeadIK.solver.AddBone(neckBone);
        return HeadIK;
    }

    /// <summary>
    /// creating the IK for the right hand
    /// </summary>
    /// <param name="rightHandBone"></param>
    /// <param name="rightForeArm"></param>
    /// <param name="rightArm"></param>
    /// <param name="rightShoulder"></param>
    /// <returns></returns>
    protected AimIK SetIKRightHand(Transform rightHandBone, Transform rightForeArm, Transform rightArm, Transform rightShoulder)
    {
        AimIK RightIK = this.gameObject.AddComponent<AimIK>();
        RightIK.solver.transform = rightHandBone;
        RightIK.solver.axis = RightIK.solver.transform.position.normalized;
        RightIK.solver.AddBone(rightShoulder);
        RightIK.solver.AddBone(rightArm);
        RightIK.solver.AddBone(rightForeArm);
        return RightIK;
    }

    /// <summary>
    /// creating the IK for the left hand
    /// </summary>
    /// <param name="leftHandBone"></param>
    /// <param name="leftForeArm"></param>
    /// <param name="leftArm"></param>
    /// <param name="leftShoulder"></param>
    /// <returns></returns>
    protected AimIK SetIKLeftHand(Transform leftHandBone, Transform leftForeArm, Transform leftArm, Transform leftShoulder)
    {
        AimIK LeftIK = this.gameObject.AddComponent<AimIK>();
        LeftIK.solver.transform = leftHandBone;
        LeftIK.solver.axis = LeftIK.solver.transform.position.normalized;
        LeftIK.solver.AddBone(leftShoulder);
        LeftIK.solver.AddBone(leftArm);
        LeftIK.solver.AddBone(leftForeArm);
        return LeftIK;
    }

    /// <summary>
    /// setting the target of the specified AimIK
    /// </summary>
    /// <param name="aimIK"></param>
    /// <param name="target"></param>
    public void SetTarget(AimIK aimIK, Transform target)
    {
        aimIK.solver.IKPosition = target.position;
    }

    /// <summary>
    /// setting the weight's target of the specified AimIK
    /// </summary>
    /// <param name="aimIK"></param>
    /// <param name="weight"></param>
    public void SetWeightTarget(AimIK aimIK, float weight)
    {
        aimIK.solver.IKPositionWeight = weight;
    }
}
