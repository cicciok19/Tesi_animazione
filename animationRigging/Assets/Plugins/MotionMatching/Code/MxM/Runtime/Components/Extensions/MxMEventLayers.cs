using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace MxM
{
    public struct EventLayerData
    {
        public int SlotId;
        public AnimationClipPlayable ClipPlayable;
        public float StartTime;
        public float EndTime;
        public float BlendRate;
        public float PlaybackRate;
        public float Weight;
        public AvatarMask Mask;

    }//End of struct EventLayerData

    [DisallowMultipleComponent]
    [RequireComponent(typeof(MxMAnimator))]
    public class MxMEventLayers : MonoBehaviour, IMxMExtension
    {
        private AnimationLayerMixerPlayable m_layerMixer;
        private MxMLayer m_baseLayer;
        private List<EventLayerData> m_eventLayers;
        private MxMAnimator m_mxmAnimator;

        private int m_layerId = 2;

        public bool DoUpdatePhase1 { get { return false; } }
        public bool DoUpdatePhase2 { get { return true; } }
        public bool DoUpdatePost { get { return false; } }

        public void Initialize()
        {
            m_eventLayers = new List<EventLayerData>(4);
            m_mxmAnimator = GetComponent<MxMAnimator>();

            if(m_mxmAnimator == null)
            {
                Debug.LogError("Could not find MxMAnimator component, MxMEventLayers component disabled");
                enabled = false;
                return;
            }

            m_layerMixer = AnimationLayerMixerPlayable.Create(m_mxmAnimator.MxMPlayableGraph, 1);

            m_layerId = m_mxmAnimator.AddLayer((Playable)m_layerMixer, 0f, false, null);
            m_baseLayer = m_mxmAnimator.GetLayer(m_layerId);
            m_mxmAnimator.SetLayerWeight(m_layerId, 1f);
        }

        public void UpdatePhase2()
        {
            float highestWeight = 0f;
            for (int i = 0; i < m_eventLayers.Count; ++i)
            {
                EventLayerData eventLayer = m_eventLayers[i];

                float curTime = (float)eventLayer.ClipPlayable.GetTime();

                float age = curTime - eventLayer.StartTime;
                float deathAge = eventLayer.EndTime - curTime;

                float weight = Mathf.Min(Mathf.Clamp01(age / eventLayer.BlendRate), Mathf.Clamp01(deathAge / eventLayer.BlendRate));

                if (weight > highestWeight)
                    highestWeight = weight;

                if (curTime >= eventLayer.EndTime)
                {
                    m_layerMixer.SetInputWeight(eventLayer.SlotId, 0f);
                    m_layerMixer.DisconnectInput(eventLayer.SlotId);
                    eventLayer.ClipPlayable.Destroy();
                    m_eventLayers.RemoveAt(i);
                    --i;
                }
                else if (Mathf.Abs(weight - eventLayer.Weight) > Mathf.Epsilon)
                {
                    eventLayer.Weight = weight;
                    m_eventLayers[i] = eventLayer;
                    m_layerMixer.SetInputWeight(eventLayer.SlotId, weight);
                }


            }

            if (m_eventLayers.Count > 0)
            {
                m_mxmAnimator.SetLayerWeight(m_layerId, highestWeight);
            }
        }

        public void Terminate() { }
        public void UpdatePhase1() { }

        public void UpdatePost() { }

        //============================================================================================
        /**
        *  @brief Begins an event that is masked by a passed avatar mask (e.g. upper body event).
        *  
        *  Note that masked events cannot use animation warping and other dynamic features that normal
        *  events use.
        *  
        *  @param [MxMEventDefinition] a_eventDefinition - the event definition to use
        *  @param [AvatarMask] a_eventMask - the mask to use for the event
        *         
        *********************************************************************************************/
        public void BeginEvent(MxMEventDefinition a_eventDefinition, AvatarMask a_eventMask, float a_blendRate,
            float a_playbackRate = 1f)
        {
            if (a_eventDefinition == null)
            {
                Debug.LogError("Trying to play a layered event but the passed event definision is null.");
                return;
            }

            MxMAnimData currentAnimData = m_mxmAnimator.CurrentAnimData;

            float bestCost = float.MaxValue;
            int bestWindupPoseId = 0;
            ref EventData bestEvent = ref currentAnimData.Events[0];
            Vector3 localPosition = Vector3.zero;

            float playerRotationY = m_mxmAnimator.AnimationRoot.rotation.eulerAngles.y;
            float desiredDelay = a_eventDefinition.DesiredDelay;
            bool eventFound = false;

            for (int i = 0; i < currentAnimData.Events.Length; ++i)
            {
                ref EventData evt = ref currentAnimData.Events[i];

                if (evt.EventId != a_eventDefinition.Id)
                    continue;

                for (int k = 0; k < evt.WindupPoseContactOffsets.Length; ++k)
                {
                    float cost = 0f;

                    if (a_eventDefinition.MatchPose)
                    {
                        cost += m_mxmAnimator.ComputePoseCost(ref currentAnimData.Poses[evt.StartPoseId + k]);
                    }

                    if (a_eventDefinition.MatchTrajectory)
                    {
                        cost += m_mxmAnimator.ComputeTrajectoryCost(ref currentAnimData.Poses[evt.StartPoseId + k]);
                    }

                    if (a_eventDefinition.MatchTiming)
                    {
                        float timeWarp = Mathf.Abs(desiredDelay - evt.TimeToHit + currentAnimData.PoseInterval * k);
                        cost += timeWarp * a_eventDefinition.TimingWeight;
                    }

                    if (cost < bestCost)
                    {
                        bestWindupPoseId = k;
                        bestCost = cost;
                        bestEvent = ref evt;

                        eventFound = true;
                    }
                }
            }

            if (!eventFound)
            {
                Debug.LogWarning("Could not find an event to match event definition: " + a_eventDefinition.ToString());
                return;
            }


            ref PoseData pose = ref currentAnimData.Poses[bestEvent.StartPoseId + bestWindupPoseId];
            AnimationClip clip = currentAnimData.Clips[pose.PrimaryClipId];

            var clipPlayable = AnimationClipPlayable.Create(m_mxmAnimator.MxMPlayableGraph, clip);

            //Setup the event layer here
            EventLayerData newEventLayer = new EventLayerData()
            {
                SlotId = FindEmptySlot(),
                ClipPlayable = clipPlayable,
                StartTime = pose.Time,
                EndTime = pose.Time + bestEvent.Length - (bestWindupPoseId * currentAnimData.PoseInterval),
                BlendRate = a_blendRate,
                PlaybackRate = a_playbackRate,
                Mask = a_eventMask,
                Weight = 0f
            };

            m_eventLayers.Add(newEventLayer);

            m_layerMixer.ConnectInput(newEventLayer.SlotId, newEventLayer.ClipPlayable, 0);
            m_layerMixer.SetInputWeight(newEventLayer.SlotId, 0.001f);
            m_layerMixer.SetLayerMaskFromAvatarMask((uint)newEventLayer.SlotId, a_eventMask);

            clipPlayable.SetTime(newEventLayer.StartTime);
            clipPlayable.SetTime(newEventLayer.StartTime);
            clipPlayable.SetSpeed(a_playbackRate * m_mxmAnimator.PlaybackSpeed);

            m_baseLayer.Mask = newEventLayer.Mask;
        }

        //============================================================================================
        /**
        *  @brief Begins an event that is masked by a passed avatar mask (e.g. upper body event).
        *  
        *  Note that masked events cannot use animation warping and other dynamic features that normal
        *  events use.
        *  
        *  @param [string] a_eventName - the name of the event to use
        *  @param [AvatarMask] a_eventMask - the mask to use for the event
        *         
        *********************************************************************************************/
        public void BeginEvent(string a_eventName, AvatarMask a_eventMask, float a_blendRate,
            bool a_matchPose = true, bool a_matchTrajectory = true, float a_playbackRate = 1f)
        {
            MxMAnimData currentAnimData = m_mxmAnimator.CurrentAnimData;

            if (currentAnimData == null)
                return;

            BeginEvent(currentAnimData.EventIdFromName(a_eventName), a_eventMask, a_blendRate, 
                a_matchPose, a_matchTrajectory, a_playbackRate);
        }

        //============================================================================================
        /**
        *  @brief Begins an event that is masked by a passed avatar mask (e.g. upper body event).
        *  
        *  Note that masked events cannot use animation warping and other dynamic features that normal
        *  events use.
        *  
        *  @param [int] a_eventDefinition - the id of the event to use
        *  @param [AvatarMask] a_eventMask - the mask to use for the event
        *         
        *********************************************************************************************/
        public void BeginEvent(int a_eventId, AvatarMask a_eventMask, float a_blendRate,
            bool a_matchPose = true, bool a_matchTrajectory = true, float a_playbackRate = 1f)
        {
            if (a_eventId < 0 || a_eventId > m_mxmAnimator.CurrentAnimData.Events.Length)
            {
                Debug.LogError("Trying to play a layered event but the Id id is out of bounds");
                return;
            }

            MxMAnimData currentAnimData = m_mxmAnimator.CurrentAnimData;

            float bestCost = float.MaxValue;
            int bestWindupPoseId = 0;
            ref EventData bestEvent = ref currentAnimData.Events[0];
            Vector3 localPosition = Vector3.zero;

            float playerRotationY = m_mxmAnimator.AnimationRoot.rotation.eulerAngles.y;
            bool eventFound = false;

            for (int i = 0; i < currentAnimData.Events.Length; ++i)
            {
                ref EventData evt = ref currentAnimData.Events[i];

                if (evt.EventId != a_eventId)
                    continue;

                for (int k = 0; k < evt.WindupPoseContactOffsets.Length; ++k)
                {
                    float cost = 0f;

                    if (a_matchPose)
                    {
                        cost += m_mxmAnimator.ComputePoseCost(ref currentAnimData.Poses[evt.StartPoseId + k]);
                    }

                    if (a_matchTrajectory)
                    {
                        cost += m_mxmAnimator.ComputeTrajectoryCost(ref currentAnimData.Poses[evt.StartPoseId + k]);
                    }

                    if (cost < bestCost)
                    {
                        bestWindupPoseId = k;
                        bestCost = cost;
                        bestEvent = ref evt;

                        eventFound = true;
                    }
                }
            }

            if (!eventFound)
            {
                Debug.LogWarning("Could not find an event to match event Id: " + a_eventId.ToString());
                return;
            }


            ref PoseData pose = ref currentAnimData.Poses[bestEvent.StartPoseId + bestWindupPoseId];
            AnimationClip clip = currentAnimData.Clips[pose.PrimaryClipId];

            var clipPlayable = AnimationClipPlayable.Create(m_mxmAnimator.MxMPlayableGraph, clip);

            //Setup the event layer here
            EventLayerData newEventLayer = new EventLayerData()
            {
                SlotId = FindEmptySlot(),
                ClipPlayable = clipPlayable,
                StartTime = pose.Time,
                EndTime = pose.Time + bestEvent.Length - (bestWindupPoseId * currentAnimData.PoseInterval),
                BlendRate = a_blendRate,
                PlaybackRate = a_playbackRate,
                Mask = a_eventMask,
                Weight = 0f
            };

            m_eventLayers.Add(newEventLayer);

            int inputCount = m_layerMixer.GetInputCount();

            m_layerMixer.ConnectInput(newEventLayer.SlotId, newEventLayer.ClipPlayable, 0);
            m_layerMixer.SetInputWeight(newEventLayer.SlotId, 0.001f);
            m_layerMixer.SetLayerMaskFromAvatarMask((uint)newEventLayer.SlotId, a_eventMask);

            clipPlayable.SetTime(newEventLayer.StartTime);
            clipPlayable.SetTime(newEventLayer.StartTime);
            clipPlayable.SetSpeed(a_playbackRate * m_mxmAnimator.PlaybackSpeed);

            m_baseLayer.Mask = newEventLayer.Mask;
        }

        private int FindEmptySlot()
        {
            int slotId = -1;

            int inputCount = m_layerMixer.GetInputCount();
            for (int i = 0; i < inputCount; ++i)
            {
                if (!m_layerMixer.GetInput(i).IsValid())
                {
                    slotId = i;
                    break;
                }
            }

            if (slotId == -1)
            {
                slotId = inputCount;
                m_layerMixer.SetInputCount(++inputCount);
            }

            return slotId;
        }


    }//End of class: MxMEventLayers
}//End of namespace: MxM
