using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MxM;

namespace MxMEditor
{
   
    //===========================================================================================
    /**
    *  @brief Class used to hold data on a tag track.
    *  
    *  This is used specifically for the editor tagging system. Each animation node in 
    *         
    *********************************************************************************************/
    [System.Serializable]
    public class TagTrack
    {
        [SerializeField] private ETags m_tagId;
        [SerializeField] private string m_tagName;
        [SerializeField] private float m_clipLength;
        [SerializeField] private List<Vector2> m_tags = new List<Vector2>();

        [System.NonSerialized] int m_selectId = -1;
        [System.NonSerialized] TagSelectType m_selectType;
        [System.NonSerialized] bool m_draggingSelected;

        public ETags TagId { get { return (ETags)m_tagId; } }
        public string TagName { get { return m_tagName; } set { m_tagName = value; } }
        public int SelectId { get { return m_selectId; } }
        public TagSelectType SelectType { get { return m_selectType; } }

        public List<Vector2> Tags { get { return m_tags; } }

        //============================================================================================
        /**
        *  @brief Constructor for the TagTrack class
        *  
        *  @param [int] _tagId - the id of the tag (relates to the tags in the animator
        *  @param [float] _clipLength -  the length of the animation clip the track belongs to
        *         
        *********************************************************************************************/
        public TagTrack(ETags _tagId, float _clipLength)
        {
            m_tagId = _tagId;
            m_clipLength = _clipLength;
            m_tags = new List<Vector2>();
        }

        //============================================================================================
        /**
        *  @brief Copy constructor for the TagTrack class
        *  
        *  @param [TagTrack] _copy - the track to copy from 
        *         
        *********************************************************************************************/
        public TagTrack(TagTrack _copy)
        {
            m_tagId = _copy.m_tagId;
            m_clipLength = _copy.m_clipLength;
            m_tags = new List<Vector2>(_copy.m_tags);
        }

        //============================================================================================
        /**
        *  @brief Adds a tag to the track at a specific time point. 
        *  
        *  The tag length will either be 0.5f or half the clip length, whichever is smallest
        *  
        *  @param [float] _point - the time point to add the the tag at
        *         
        *********************************************************************************************/
        public void AddTag(float a_point)
        {
            a_point = Mathf.Clamp(a_point, 0f, m_clipLength - Mathf.Min(m_clipLength / 2f, 0.05f));

            m_tags.Add(new Vector2(a_point, a_point + Mathf.Min(m_clipLength - a_point, 0.25f)));
        }

        //============================================================================================
        /**
        *  @brief Adds a tag to the track given a specific range
        *  
        *  @param [float] _start - the start time of the tag
        *  @param [float] _end - the end time of the tag
        *         
        *********************************************************************************************/
        public void AddTag(float a_start, float a_end)
        {
            m_tags.Add(new Vector2(a_start, a_end));
        }

        //============================================================================================
        /**
        *  @brief Removes a tag by its ID if it exists
        *  
        *  @param [int] _id - The id of the tag to remove
        *         
        *********************************************************************************************/
        public void RemoveTag(int a_id)
        {
            if(a_id < m_tags.Count && a_id >= 0)
                m_tags.RemoveAt(a_id);
        }

        //============================================================================================
        /**
        *  @brief Removes all tags that cover a specific time point
        *  
        *  @param [float] _time - the time point to remove tags from
        *         
        *********************************************************************************************/
        public void RemoveTags(float _time)
        {
            if(_time >=0f && _time <= m_clipLength)
            {
                for(int i=0; i < m_tags.Count; ++i)
                {
                    if(_time > m_tags[i].x && _time < m_tags[i].y)
                    {
                        m_tags.RemoveAt(i);
                        --i;
                    }
                }
            }
        }

        //============================================================================================
        /**
        *  @brief Removes all tags that overlap a specific time range
        *  
        *  @param [float] _time - the time point to remove tags from
        *         
        *********************************************************************************************/
        public void RemoveTags(float _start, float _end)
        {
            _start = Mathf.Clamp(_start, 0f, m_clipLength);
            _end = Mathf.Clamp(_end, 0f, m_clipLength);

            for (int i = 0; i < m_tags.Count; ++i)
            {
                Vector2 tag = m_tags[i];

                if (tag.x > _start && tag.x < _end ||
                    tag.y > _start && tag.y < _end ||
                    _start > tag.x && _start < tag.y)
                {
                    m_tags.RemoveAt(i);
                    --i;
                }
            }
        }

        //============================================================================================
        /**
        *  @brief Returns a tag by its id
        *  
        *  @param [int] _id - The id of the tag to return
        *  
        *  @return Vector2 - the tag bounds of the id'd tag
        *         
        *********************************************************************************************/
        public Vector2 GetTag(int _id)
        {
            if (_id < m_tags.Count && _id >= 0)
                return m_tags[_id];

            return Vector2.zero;
        }

        //============================================================================================
        /**
        *  @brief Returns a tag that envelops a specific time
        *  
        *  @param [float] _time - the time to sample
        *  
        *  @return Vector2 - the tag bounds of the id'd tag
        *         
        *********************************************************************************************/
        public Vector2 GetTag(float _time)
        {
            if (_time > 0f && _time <= m_clipLength)
            {
                foreach (Vector2 tagRange in m_tags)
                {
                    if (_time >= tagRange.x && _time <= tagRange.y)
                        return tagRange;
                }
            }

            return Vector2.zero;
        }

        //============================================================================================
        /**
        *  @brief Returns a tag id based on a specific time
        *  
        *  @param [float] _time - the time to sample
        *  
        *  @return int - the id of the tag at that specific time
        *         
        *********************************************************************************************/
        public int GetTagId(float _time)
        {
            if (_time > 0f && _time <= m_clipLength)
            {
                for(int i=0; i < m_tags.Count; ++i)
                {
                    if (_time >= m_tags[i].x && _time <= m_tags[i].y)
                        return i;
                }
            }

            return -1;
        }

        //============================================================================================
        /**
        *  @brief Checks if a specific time on the track is tagged
        *  
        *  @param [float] _time - the time to check
        *  
        *  @return bool - whether or not the time has been tagged
        *         
        *********************************************************************************************/
        public bool IsTimeTagged(float _time)
        {
            if (_time > 0f && _time <= m_clipLength)
            {
                for (int i = 0; i < m_tags.Count; ++i)
                {
                    if (_time >= m_tags[i].x && _time <= m_tags[i].y)
                        return true;
                }
            }

            return false;
        }

        //============================================================================================
        /**
        *  @brief Draws and manages all the tags within the track along a GUI track in the editor
        *  
        *  @param [Rect] _trackRect - the rect of the track to draw them on
        *  @param [float] _scrollValueX - the scroll value of the scroll viewEditorUtil.EditorFunctions.GetHighlightTex()
        *  @param [float] _zoom - tyhe zoom value of the 
        *         
        *********************************************************************************************/
        public bool DrawTags(Rect _trackRect, float _zoom, MxMTaggingWindow _taggingWindow)
        {
            bool ret = false;

            Texture markerIcon = EditorGUIUtility.IconContent("blendKey").image;
            Texture selectIcon = EditorGUIUtility.IconContent("curvekeyframe").image;

            Event evt = Event.current;

            for (int i = 0; i < m_tags.Count; ++i)
            {
                Vector2 tag = m_tags[i];

                float startX = (tag.x * 75f) * _zoom;
                float endX = (tag.y * 75f) * _zoom;

                Color baseColor = GUI.color;

                if(EditorGUIUtility.isProSkin)
                    GUI.color = Color.black;

                Rect barRect = new Rect(startX, _trackRect.y + 7f, endX - startX, 5f);
                GUI.Box(barRect, "");
                GUI.color = baseColor;

                if (m_selectId == i && m_selectType == TagSelectType.All)
                    GUI.DrawTexture(barRect, EditorUtil.EditorFunctions.GetHighlightTex());

                barRect = _trackRect;
                barRect.x = startX + markerIcon.width / 2f;
                barRect.width = endX - startX - markerIcon.width;

                startX -= markerIcon.width / 2f;
                endX -= markerIcon.width / 2f;

                Rect leftRect = new Rect(startX, _trackRect.y + 3f,
                        markerIcon.width, markerIcon.height);

                Rect rightRect = new Rect(endX, _trackRect.y + 3f,
                        markerIcon.width, markerIcon.height);

                if (startX > -markerIcon.width
                    && startX < _trackRect.width + markerIcon.width)
                {
                    GUI.DrawTexture(leftRect, markerIcon);

                    if (m_selectId == i && (m_selectType == TagSelectType.Left
                        || m_selectType == TagSelectType.All))
                    {
                        GUI.DrawTexture(leftRect, selectIcon);
                    }

                }
                if (endX > -markerIcon.width
                    && endX < _trackRect.width + markerIcon.width)
                {
                    GUI.DrawTexture(rightRect, markerIcon);
                    if (m_selectId == i && (m_selectType == TagSelectType.Right
                        || m_selectType == TagSelectType.All))
                    {
                        GUI.DrawTexture(rightRect, selectIcon);
                    }
                }

                //Selection and moving
                if (evt.isMouse && evt.button == 0)
                {
                    switch (evt.type)
                    {
                        case EventType.MouseDown:
                            {
                                if (barRect.Contains(evt.mousePosition))
                                {
                                    m_selectId = i;
                                    m_selectType = TagSelectType.All;
                                    ret = true;
                                    m_draggingSelected = true;
                                    _taggingWindow.SetTime(m_tags[m_selectId].x);
                                }
                                else if (leftRect.Contains(evt.mousePosition))
                                {
                                    m_selectId = i;
                                    m_selectType = TagSelectType.Left;
                                    ret = true;
                                    m_draggingSelected = true;
                                    _taggingWindow.SetTime(m_tags[m_selectId].x);
                                }
                                else if (rightRect.Contains(evt.mousePosition))
                                {
                                    m_selectId = i;
                                    m_selectType = TagSelectType.Right;
                                    ret = true;
                                    m_draggingSelected = true;

                                    _taggingWindow.SetTime(m_tags[m_selectId].y);
                                }
                            }
                            break;
                        case EventType.MouseUp:
                            {
                                m_draggingSelected = false;


                            }
                            break;
                        case EventType.MouseDrag:
                            {
                                if (m_draggingSelected && m_selectId == i)
                                {
                                    float desiredValueDelta = ((evt.delta.x / _zoom)) / 75f;

                                    Vector2 selected = m_tags[m_selectId];

                                    switch (m_selectType)
                                    {
                                        case TagSelectType.All:
                                            {
                                                selected.x += desiredValueDelta;
                                                selected.y += desiredValueDelta;

                                                if (selected.x < 0f)
                                                {
                                                    selected.y -= selected.x;
                                                    selected.x -= selected.x;
                                                }

                                                if (selected.y > m_clipLength)
                                                {
                                                    selected.x -= (selected.y - m_clipLength);
                                                    selected.y -= (selected.y - m_clipLength);
                                                }

                                                _taggingWindow.Modified(selected.x);
                                            }
                                            break;
                                        case TagSelectType.Left:
                                            {
                                                selected.x += desiredValueDelta;
                                                selected.x = Mathf.Clamp(selected.x, 0f, selected.y - 0.1f);
                                                _taggingWindow.Modified(selected.x);
                                            }
                                            break;
                                        case TagSelectType.Right:
                                            {
                                                selected.y += desiredValueDelta;
                                                selected.y = Mathf.Clamp(selected.y, selected.x + 0.1f, m_clipLength);
                                                _taggingWindow.Modified(selected.y);
                                            }
                                            break;
                                    }

                                    m_tags[m_selectId] = selected;

                                }

                            }
                            break;
                    }
                }
            }
            return ret;
        }

        //============================================================================================
        /**
        *  @brief Deselects all tags on the track
        *         
        *********************************************************************************************/
        public void Deselect()
        {
            m_selectId = -1;
            m_selectType = TagSelectType.None;
        }

        //============================================================================================
        /**
        *  @brief Deselects all tags on the track
        *         
        *********************************************************************************************/
        public void DeleteSelectedTag()
        {
            if(m_selectId > -1)
            {
                m_tags.RemoveAt(m_selectId);
                Deselect();
            }
        }

        //============================================================================================
        /**
        *  @brief Verifies the tag data compared to the passed clip. The passed clip should be the
        *  clip that this tag track is placed on
        *  
        *  @param [AnimationClip] a_clip - the clip to base verification on.
        *         
        *********************************************************************************************/
        public void VerifyData(AnimationClip a_clip)
        {
            for(int i=0; i < m_tags.Count; ++i)
            {
                Vector2 tag = m_tags[i];

                if(tag.x > a_clip.length)
                {
                    tag.x = a_clip.length - 0.01f;
                }

                if(tag.y > a_clip.length)
                {
                    tag.y = a_clip.length;
                }

                if(tag.x > tag.y)
                {
                    tag.x = tag.y - 0.01f;
                }

                m_tags[i] = tag;
            }
        }
    }//End of class: TagTrack
}//End of namespace: MxM
