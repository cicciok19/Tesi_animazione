// ================================================================================================
// File: UnityRiggingIntegration.cs
// 
// Authors:  Kenneth Claassen
// Date:     2019-10-10: Created this file.
// 
//     Contains a part of the 'MxM' namespace for 'Unity Engine'.
// ================================================================================================
#if UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.Animations.Rigging;

namespace MxM
{
    //============================================================================================
    /**
    *  @brief A gameObject component to add for Unity Rigging Integration with 'Motion Matching for Unity'
    *  
    *  This component should be attached either alongside the MxMAnimator or on any child object.
    *  It is recommend it to place it on the rig parent object itself. In the inspector there will 
    *  be a list of rig controls. Place all of your rig controls into this reference list for the 
    *  integration to work correctly.
    *         
    *********************************************************************************************/
    public class UnityRiggingIntegration : MonoBehaviour, IMxMUnityRiggingIntegration
    {
        [SerializeField]
        private Transform[] m_rigControls = null;

        private RigBuilder m_rigBuilder;
        private Rig[] m_rigs;

        private List<IRigConstraint> m_rigConstraints; 

        private NativeArray<TransformStreamHandle> m_rigStreamHandles;
        private NativeArray<TransformData> m_rigTransformCache;

        private NativeArray<PropertyStreamHandle> m_rigWeightHandles;
        private NativeArray<float> m_rigWeightCache;

        private AnimationScriptPlayable m_riggingScriptPlayable;

        private bool m_fixRigTransforms;
        private float m_lastTimeCached;

        //============================================================================================
        /**
        *  @brief Monobehaviour OnDestroy function.
        *         
        *********************************************************************************************/
        private void OnDestroy()
        {
            DisposeNativeData();
        }

        //============================================================================================
        /**
        *  @brief Initializes the rigging integration. This is called automatically by MxM
        *  
        *  This function sets up native arrays of TransformStreamHandles so that the transforms 
        *  of the rig can be accessed from an animation job
        *  
        *  @param [PlayableGraph] a_playableGraph - the playable graph to use for the integration
        *  @param [Animator] a_animator - a reference to the animator component
        *         
        *********************************************************************************************/
        public void Initialize(PlayableGraph a_playableGraph, Animator a_animator)
        {
            m_rigBuilder = GetComponentInChildren<RigBuilder>();

            m_fixRigTransforms = false;
            m_lastTimeCached = Time.time;

            if(m_rigBuilder != null)
            {
                var rigLayers = m_rigBuilder.layers;

                m_rigs = new Rig[rigLayers.Count];
                for (int i = 0; i < rigLayers.Count; ++i)
                {
                    Rig rig = rigLayers[i].rig;
                    m_rigs[i] = rig;
                }
            }

            m_rigStreamHandles = new NativeArray<TransformStreamHandle>(m_rigControls.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_rigTransformCache = new NativeArray<TransformData>(m_rigControls.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            m_rigWeightHandles = new NativeArray<PropertyStreamHandle>(m_rigs.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_rigWeightCache = new NativeArray<float>(m_rigs.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            for(int i = 0; i < m_rigControls.Length; ++i)
            {
                Transform controlTransform = m_rigControls[i];

                m_rigStreamHandles[i] = a_animator.BindStreamTransform(controlTransform);
                m_rigTransformCache[i] = new TransformData() { Position = controlTransform.localPosition, Rotation = controlTransform.localRotation };
            }

            for(int i = 0; i < m_rigs.Length; ++i)
            {
                m_rigWeightHandles[i] = a_animator.BindStreamProperty(m_rigs[i].transform, typeof(Rig), "m_Weight");
                m_rigWeightCache[i] = m_rigs[i].weight;
            }

            var rigIntegrationJob = new RiggingIntegrationJob()
            {
                FixRigTransforms = m_fixRigTransforms,
                RigStreamHandles = m_rigStreamHandles,
                RigTransformCache = m_rigTransformCache,
                RigWeightHandles = m_rigWeightHandles,
                RigWeightCache = m_rigWeightCache,
            };

            m_riggingScriptPlayable = AnimationScriptPlayable.Create(a_playableGraph, rigIntegrationJob);
            var output = AnimationPlayableOutput.Create(a_playableGraph, "RigIntegrationOutput", a_animator);

            output.SetSourcePlayable(m_riggingScriptPlayable);
        }

        //============================================================================================
        /**
        *  @brief This function is called automatically by the MxMAnimator before any playable graph
        *  changes have occured. It caches the position and rotation of all transfroms in your control
        *  rig so they can be re-applied in AnimationJobs.
        *         
        *********************************************************************************************/
        public void CacheTransforms()
        {
            if (Time.time - m_lastTimeCached > Mathf.Epsilon)
            {
                m_lastTimeCached = Time.time;

                var rigJobData = m_riggingScriptPlayable.GetJobData<RiggingIntegrationJob>();
                m_fixRigTransforms = rigJobData.FixRigTransforms = false;

                for (int i = 0; i < m_rigControls.Length; ++i)
                {
                    Transform controlTransform = m_rigControls[i];

                    rigJobData.RigTransformCache[i] = new TransformData() { Position = controlTransform.localPosition, Rotation = controlTransform.localRotation };
                }

                for(int i = 0; i < m_rigs.Length; ++i)
                {
                    rigJobData.RigWeightCache[i] = m_rigs[i].weight;
                }

                m_riggingScriptPlayable.SetJobData(rigJobData);
            }
        }

        //============================================================================================
        /**
        *  @brief This function is called automatically by the MxMAnimator when there is a playable
        *  graph change. It informs the animation job to fix any control transforms that are 
        *  reset by the graph restructure.
        *         
        *********************************************************************************************/
        public void FixRigTransforms()
        {
            if (!m_fixRigTransforms)
            {
                var rigJobData = m_riggingScriptPlayable.GetJobData<RiggingIntegrationJob>();
                m_fixRigTransforms = rigJobData.FixRigTransforms = true;
                m_riggingScriptPlayable.SetJobData(rigJobData);
            }
        }

        //============================================================================================
        /**
        *  @brief Disposes all native data to avoid memory leaks. This is called automatically by 
        *  the MxMAnimator at appropriate times or if the component is destroyed.
        *         
        *********************************************************************************************/
        public void DisposeNativeData()
        {
            if (m_rigStreamHandles.IsCreated)
                m_rigStreamHandles.Dispose();

            if (m_rigTransformCache.IsCreated)
                m_rigTransformCache.Dispose();

            if (m_rigWeightHandles.IsCreated)
                m_rigWeightHandles.Dispose();

            if (m_rigWeightCache.IsCreated)
                m_rigWeightCache.Dispose();
        }
    }//End of class: UnityRiggingIntegration

    //============================================================================================
    /**
    *  @brief Animation job used to fix rig control transforms inbetween the MxMAnimation stream
    *  and the AnimationRigging stream
    *         
    *********************************************************************************************/
    [BurstCompile]
    public struct RiggingIntegrationJob : IAnimationJob
    {
        public bool FixRigTransforms;

        public NativeArray<TransformStreamHandle> RigStreamHandles;
        public NativeArray<TransformData> RigTransformCache;

        public NativeArray<PropertyStreamHandle> RigWeightHandles;
        public NativeArray<float> RigWeightCache;

        public void ProcessAnimation(AnimationStream stream)
        {
            if (FixRigTransforms)
            {
                for (int i = 0; i < RigStreamHandles.Length; ++i)
                {
                    RigStreamHandles[i].SetLocalPosition(stream, RigTransformCache[i].Position);
                    RigStreamHandles[i].SetLocalRotation(stream, RigTransformCache[i].Rotation);
                }

                for(int i = 0; i < RigWeightHandles.Length; ++i)
                {
                    RigWeightHandles[i].SetFloat(stream, RigWeightCache[i]);
                }
            }
        }

        public void ProcessRootMotion(AnimationStream stream) { }

    }//End of struct: RiggingIntegrationJob
}//End of namespace: MxM
#endif