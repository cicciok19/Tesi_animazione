// ================================================================================================
// File: MxMAnimator_Layers.cs
// 
// Authors:  Kenneth Claassen
// Date:     2019-10-10: Created this file.
// 
//     Contains a part of the 'MxM' namespace for 'Unity Engine'.
// ================================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MxM
{
    //============================================================================================
    /**
    *  @brief This is partial implementation of the MxMAnimator. This particular partial class 
    *  handles all layer system logic for the MxMAnimator.
    *         
    *********************************************************************************************/
    public partial class MxMAnimator : MonoBehaviour
    {
        //Layers
        private Coroutine m_controllerFadeCoroutine; //Coroutine used for layer fading
        private Dictionary<int, MxMLayer> m_layers; //Map or layers. The index is the input ID on the MxM Layer Mixer
        private List<MxMLayer> m_transitioningLayers; //A list of layers that are currently requiring updates for transitions.

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        private void UpdateLayers()
        {
            //Update MxMLayer Transitions
            for (int i = 0; i < m_transitioningLayers.Count; ++i)
            {
                if (m_transitioningLayers[i].UpdateTransition())
                {
                    m_transitioningLayers.RemoveAt(i);
                    --i;
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void SetLayerClip(int a_layerId, AnimationClip a_clip)
        {
            if (a_layerId > 1)
            {
                MxMLayer layer = null;
                if (m_layers.TryGetValue(a_layerId, out layer))
                {
                    layer.SetLayerClip(a_clip);
                    layer.PrimaryClipTime = 0f;
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void SetLayerClip(int a_layerId, AnimationClip a_clip, AvatarMask a_mask, float a_time = 0f, float a_weight = 1f)
        {
            if (a_layerId > 1)
            {
                MxMLayer layer = null;
                if (m_layers.TryGetValue(a_layerId, out layer))
                {
                    layer.SetLayerClip(a_clip, a_time);
                    layer.Mask = a_mask;
                    layer.Weight = a_weight;
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void SetLayerMask(int a_layerId, AvatarMask a_mask)
        {
            if (a_layerId > 1)
            {
                MxMLayer layer = null;
                if (m_animationLayerMixer.IsValid() && m_layers.TryGetValue(a_layerId, out layer))
                {
                    m_layers[a_layerId].Mask = a_mask;
                }
                else
                {
                    Debug.LogError("Trying to set a layer mask before the playable graph is created. Aborting Operation");
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void TransitionLayerClip(int a_layerId, AnimationClip a_clip, float a_fadeRate, float a_time = 0f)
        {
            MxMLayer layer = null;
            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                layer.TransitionToClip(a_clip, a_fadeRate, a_time);

                if (!m_transitioningLayers.Contains(layer))
                {
                    m_transitioningLayers.Add(layer);
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void TransitionLayerPlayable(int a_layerId, ref Playable a_playable, float a_fadeRate, float a_time = 0f)
        {
            MxMLayer layer = null;
            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                layer.TransitionToPlayable(ref a_playable, a_fadeRate, a_time);

                if (!m_transitioningLayers.Contains(layer))
                {
                    m_transitioningLayers.Add(layer);
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void BlendInLayer(int a_layerId, float a_fadeRate, float a_targetWeight = 1f)
        {
            MxMLayer layer = null;

            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                if (layer.FadeCoroutine != null)
                    StopCoroutine(layer.FadeCoroutine);

                layer.FadeCoroutine = StartCoroutine(FadeInLayer(a_layerId, a_fadeRate, Mathf.Clamp01(a_targetWeight)));
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void BlendOutLayer(int a_layerId, float a_fadeRate, float a_targetWeight = 0f)
        {
            MxMLayer layer = null;

            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                if (layer.FadeCoroutine != null)
                    StopCoroutine(layer.FadeCoroutine);

                layer.FadeCoroutine = StartCoroutine(FadeOutLayer(a_layerId, a_fadeRate, Mathf.Clamp01(a_targetWeight)));
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        private IEnumerator FadeInLayer(int a_layerId, float a_fadeRate, float a_targetWeight)
        {
            if (a_layerId < m_animationLayerMixer.GetInputCount())
            {
                float curWeight = m_animationLayerMixer.GetInputWeight(a_layerId);

                while (curWeight < a_targetWeight)
                {
                    curWeight += a_fadeRate * p_currentDeltaTime * m_playbackSpeed;

                    if (curWeight > a_targetWeight)
                        curWeight = a_targetWeight;

                    m_animationLayerMixer.SetInputWeight(a_layerId, curWeight);

                    yield return null;
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        private IEnumerator FadeOutLayer(int a_layerId, float a_fadeRate, float a_targetWeight)
        {
            if (a_layerId < m_animationLayerMixer.GetInputCount())
            {
                float curWeight = m_animationLayerMixer.GetInputWeight(a_layerId);

                while (curWeight > a_targetWeight)
                {
                    curWeight -= a_fadeRate * p_currentDeltaTime * m_playbackSpeed;

                    if (curWeight < a_targetWeight)
                        curWeight = a_targetWeight;

                    m_animationLayerMixer.SetInputWeight(a_layerId, curWeight);

                    yield return null;
                }
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void BlendOutController(float a_fadeRate)
        {
            if (p_animator.runtimeAnimatorController != null)
            {
                if (m_controllerFadeCoroutine != null)
                    StopCoroutine(m_controllerFadeCoroutine);

                m_controllerFadeCoroutine = StartCoroutine(FadeOutLayer(1, a_fadeRate, 0));
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void BlendInController(float a_fadeRate)
        {
            if (p_animator.runtimeAnimatorController != null)
            {
                if (m_controllerFadeCoroutine != null)
                    StopCoroutine(m_controllerFadeCoroutine);

                m_controllerFadeCoroutine = StartCoroutine(FadeInLayer(1, a_fadeRate, 1f));
            }
        }

        //============================================================================================
        /**
        *  @brief Sets the animation mask to use for the animator controller layer if you are using 
        *  one. This enables control of masked layer animations directly through the Animator Controller
        *  
        *  @param [AvatarMask] a_mask - the avatar mask to use
        *         
        *********************************************************************************************/
        public void SetControllerMask(AvatarMask a_mask)
        {
            if (a_mask == null)
            {
                Debug.LogWarning("Cannot set animator controller mask on MxMAnimator with a null mask");
                return;
            }

            MxMLayer layer = null;

            if (m_layers.TryGetValue(1, out layer))
            {
                layer.Mask = a_mask;
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public int AddLayer(AnimationClip a_clip, float a_weight = 0f, bool a_additive = false, AvatarMask a_mask = null)
        {
            int slotToUse = -1;

            if (a_clip == null)
                return slotToUse;

            int inputCount = m_animationLayerMixer.GetInputCount();

            for (int i = 2; i < inputCount; ++i)
            {
                Playable layerPlayable = m_animationLayerMixer.GetInput(i);

                if (layerPlayable.IsNull())
                {
                    slotToUse = i;
                    break;
                }
            }

            if (slotToUse < 0)
            {
                m_animationLayerMixer.SetInputCount(inputCount + 1);
                slotToUse = inputCount;
            }

            MxMLayer layer = new MxMLayer(slotToUse, 2, ref m_animationLayerMixer, a_clip, a_mask, a_weight, a_additive);
            m_layers.Add(slotToUse, layer);

            return slotToUse;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public int AddLayer(Playable a_playable, float a_weight = 0f, bool a_additive = false, AvatarMask a_mask = null)
        {
            int slotToUse = -1;

            if (!a_playable.IsNull())
            {
                int inputCount = m_animationLayerMixer.GetInputCount();

                for (int i = 2; i < inputCount; ++i)
                {
                    Playable layerPlayable = m_animationLayerMixer.GetInput(i);

                    if (layerPlayable.IsNull())
                    {
                        slotToUse = i;
                        break;
                    }
                }

                if (slotToUse < 0)
                {
                    m_animationLayerMixer.SetInputCount(inputCount + 1);
                    slotToUse = inputCount;
                }

                MxMLayer layer = new MxMLayer(slotToUse, ref m_animationLayerMixer, a_playable, a_mask, a_weight, a_additive);
                m_layers.Add(slotToUse, layer);
            }

            return slotToUse;
        }

        //============================================================================================
        /**
        *  @brief Returns the requested layer if it exists.
        *  
        *  @param [int] a_layerId - the id of the layer turn return (must be 2 or greater)
        *  
        *  @return [MxMLayer] - a reference to the requested layer or null if it doesn't exist
        *         
        *********************************************************************************************/
        public MxMLayer GetLayer(int a_layerId)
        {
            MxMLayer layer = null;
            m_layers.TryGetValue(a_layerId, out layer);

            return layer;
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public int SetLayer(int a_layerId, AnimationClip a_clip, float a_weight = 0f, bool a_additive = false, AvatarMask a_mask = null)
        {
            if (a_clip == null)
            {
                Debug.LogError("Attempting to set a layer with a null AnimationClip.... action aborted.");
                return -1;
            }

            MxMLayer layer = null;

            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                layer.SetLayerClip(a_clip);
                layer.Mask = a_mask;

                m_animationLayerMixer.SetInputWeight(a_layerId, a_weight);
                m_animationLayerMixer.SetLayerAdditive((uint)a_layerId, a_additive);

                return a_layerId;
            }
            else
            {
                Debug.LogError("Could not set layer. Layer does not exist.");
                return -1;
            }

        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public int SetLayer(int a_layerId, ref Playable a_playable, float a_weight = 0, bool a_additive = false, AvatarMask a_mask = null)
        {
            if (!a_playable.IsValid())
            {
                Debug.LogError("Attempting to set a layer with an Invalid Playable.... action aborted.");
                return -1;
            }

            MxMLayer layer = null;

            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                layer.SetLayerPlayable(ref a_playable);

                m_animationLayerMixer.SetInputWeight(a_layerId, a_weight);
                m_animationLayerMixer.SetLayerAdditive((uint)a_layerId, a_additive);
                m_animationLayerMixer.SetLayerMaskFromAvatarMask((uint)a_layerId, a_mask);

                return a_layerId;
            }
            else
            {
                Debug.LogWarning("Could not set layer. Layer does not exist.");
                return -1;
            }
        }

        //============================================================================================
        /**
        *  @brief Removes an MxM layer from the playable graph
        *         
        *********************************************************************************************/
        public bool RemoveLayer(int a_layerId, bool a_destroyPlayable = true)
        {
            MxMLayer layer = null;

            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                m_animationLayerMixer.SetInputWeight(a_layerId, 0f);
                layer.ClearLayer();

                m_layers.Remove(a_layerId);
                return true;
            }
            else
            {
                Debug.LogWarning("Could not remove MxM Layer. The player doesn't exist");
                return false;
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void RemoveAllLayers(bool a_destroyPlayable = true)
        {
            foreach (KeyValuePair<int, MxMLayer> pair in m_layers)
            {
                if (pair.Value != null)
                {
                    pair.Value.ClearLayer();
                }

                m_animationLayerMixer.SetInputWeight(pair.Key, 0f);
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void SetLayerWeight(int a_layerId, float a_weight)
        {
            if (a_layerId == 0)
                return;

            MxMLayer layer = null;
            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                layer.Weight = a_weight;
            }
            else
            {
                Debug.LogWarning("Trying to set the layer weight on an MxMLayer but the layer doesn't exist");
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void SetControllerLayerWeight(float a_weight)
        {
            if (p_animator.runtimeAnimatorController != null)
            {
                m_animationLayerMixer.SetInputWeight(1, Mathf.Clamp01(a_weight));
            }
            else
            {
                Debug.LogWarning("Trying to set the layer weight of the mecanim controller while using " +
                    "MxM. However there is no controller available.");
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void SetLayerAdditive(int a_layerId, bool a_additive)
        {
            if (a_layerId == 0)
                return;

            MxMLayer layer = null;
            if (m_layers.TryGetValue(a_layerId, out layer))
            {
                layer.Additive = a_additive;
            }
            else
            {
                Debug.LogWarning("Trying to set layer additive on an MxMLayer but the layer doesn't exist");
            }
        }

        //============================================================================================
        /**
        *  @brief 
        *         
        *********************************************************************************************/
        public void ResetLayerWeights()
        {
            if (!m_animationLayerMixer.IsNull())
            {
                m_animationLayerMixer.SetInputWeight(0, 1f);

                int inputCount = m_animationLayerMixer.GetInputCount();
                for (int i = 1; i < inputCount; ++i)
                {
                    m_animationLayerMixer.SetInputWeight(i, 0f);
                }
            }
        }

    }//End of partical class: MxMAnimator
}//End of namespace: MxM
