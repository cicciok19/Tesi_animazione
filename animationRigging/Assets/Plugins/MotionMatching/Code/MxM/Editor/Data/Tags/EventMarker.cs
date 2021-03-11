using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MxM;

namespace MxMEditor
{
    //============================================================================================
    /**
    *  @brief Structure for holding data on events.
    *         
    *********************************************************************************************/
    [System.Serializable]
    public class EventMarker
    {
        [SerializeField] public int EventId;
        [SerializeField] public string EventName;
        [SerializeField] public float EventTime;
        [SerializeField] public float Windup;
        [SerializeField] public List<float> Actions;
        [SerializeField] public float FollowThrough;
        [SerializeField] public float Recovery;
        [SerializeField] public List<EventContact> Contacts;

        [System.NonSerialized] private bool m_selected;
        [System.NonSerialized] private bool m_dragging;

        //============================================================================================
        /**
        *  @brief constructor for event marker struct
        *         
        *********************************************************************************************/
        public EventMarker(int _eventId, float _eventTime,
            EventContact[] _contacts = null, float[] _actions = null, float _windup = 0.2f,
            float _followThrough = 0.2f, float _recovery = 0.2f)
        {
            EventId = _eventId;
            EventTime = _eventTime;
            Windup = _windup;
            Actions = new List<float>();
            FollowThrough = _followThrough;
            Recovery = _recovery;
            Contacts = new List<EventContact>();

            if (_contacts != null)
            {
                for (int i = 0; i < _contacts.Length; ++i)
                {
                    Contacts.Add(_contacts[i]);
                }
            }

            if (Contacts.Count == 0)
                Contacts.Add(new EventContact());

            if (_actions != null)
            {               
                for (int i = 0; i < _actions.Length; ++i)
                { 
                    Actions.Add(_actions[i]);
                }
            }

            if (Actions.Count == 0)
                Actions.Add(0.2f);
        }

        //============================================================================================
        /**
        *  @brief Copy constructor
        *         
        *********************************************************************************************/
        public EventMarker(EventMarker _copy)
        {
            EventId = _copy.EventId;
            EventTime = _copy.EventTime;
            Windup = _copy.Windup;
            Actions = new List<float>(_copy.Actions);
            FollowThrough = _copy.FollowThrough;
            Recovery = _copy.Recovery;
            Contacts = new List<EventContact>(_copy.Contacts);
        }

        //============================================================================================
        /**
        *  @brief Copy constructor
        *         
        *********************************************************************************************/
        public void DrawEvent(Rect _trackRect, float _zoom, MxMTaggingWindow _taggingWindow, IMxMAnim a_mxmAnim)
        {
            Texture markerIcon = EditorGUIUtility.IconContent("Animation.EventMarker").image;

            if(m_selected)
            {
                Handles.BeginGUI();
                Handles.color = Color.green;

                Vector3 start = new Vector3(EventTime * 75f * _zoom - (markerIcon.width / 2f),
                    _trackRect.y + markerIcon.height / 2f, 0f);
                Vector3 end = new Vector3(start.x - Actions[0] * 75f * _zoom + markerIcon.width / 2f, start.y, 0f);
                Vector3 up = new Vector3(end.x, end.y - markerIcon.height / 4f);
                Vector3 down = new Vector3(up.x, up.y + markerIcon.height / 2f);

                Handles.DrawLine(start, end);
                Handles.DrawLine(up, down);

                start = end;
                end = new Vector3(start.x - Windup * 75f * _zoom, start.y, 0f);
                up.x = down.x = end.x;

                Handles.color = Color.blue;
                Handles.DrawLine(start, end);
                Handles.DrawLine(up, down);

                start = new Vector3(EventTime * 75f * _zoom + (markerIcon.width / 2f),
                    _trackRect.y + markerIcon.height / 2f, 0f);

                Handles.color = Color.green;
                for(int i = 1; i < Actions.Count; ++i)
                {
                    float markerWidth = 0;

                    if(i == 1)
                        markerWidth = markerIcon.width / 2f;

                    end = new Vector3(start.x + Actions[i] * 75f * _zoom - markerWidth, start.y, 0f);
                    up.x = down.x = end.x;

                    Handles.DrawLine(start, end);
                    Handles.DrawLine(up, down);

                    //Draw a circle maybe

                    start = end;
                }

                end = new Vector3(start.x + FollowThrough * 75f * _zoom - markerIcon.width / 2f, start.y, 0f);
                up.x = down.x = end.x;

                Handles.color = Color.red;
                Handles.DrawLine(start, end);
                Handles.DrawLine(up, down);

                start = end;
                end = new Vector3(start.x + Recovery * 75f * _zoom, start.y, 0f);
                up.x = down.x = end.x;

                Handles.color = Color.yellow;
                Handles.DrawLine(start, end);
                Handles.DrawLine(up, down);

                Handles.EndGUI();
            }

            Rect markerRect = new Rect(EventTime * 75f * _zoom - (markerIcon.width / 2f), _trackRect.y,
                markerIcon.width, markerIcon.height);
            GUI.DrawTexture(markerRect, markerIcon);

            if (m_selected)
                GUI.DrawTexture(markerRect, EditorUtil.EditorFunctions.GetHighlightTex());

            Event evt = Event.current;

            markerRect.x -= 3f;
            markerRect.height = _trackRect.height;
            markerRect.width += 6f;

            if (evt.isMouse)
            {
                if (evt.button == 0)
                {
                    switch (evt.type)
                    {
                        case EventType.MouseDown:
                            {
                                if (markerRect.Contains(evt.mousePosition))
                                {
                                    m_selected = true;
                                    m_dragging = true;
                                    _taggingWindow.SelectEvent(this, EventTime);
                                    evt.Use();
                                }

                            }
                            break;
                        case EventType.MouseUp:
                            {
                                m_dragging = false;

                            }
                            break;
                        case EventType.MouseDrag:
                            {
                                if (m_dragging && m_selected)
                                {
                                    float desiredValueDelta = ((evt.delta.x / _zoom)) / 75f;

                                    EventTime += desiredValueDelta;

                                    EventTime = Mathf.Clamp(EventTime, 0f,
                                        _taggingWindow.TargetClip.length);

                                    _taggingWindow.Modified(EventTime);

                                    evt.Use();
                                }
                            }
                            break;
                    }
                }
                else if(evt.button == 1)
                {
                    //Begin Context menu
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Delete"), false, a_mxmAnim.OnDeleteEventMarker, this);
                    menu.ShowAsContext();
                }
            }
            
        }

        //============================================================================================
        /**
        *  @brief Deselects the event marker
        *         
        *********************************************************************************************/
        public void Deselect()
        {
            m_selected = false;
        }

        //============================================================================================
        /**
        *  @brief Validates that the event Ids and event name match up with Event definitions
        *         
        *********************************************************************************************/
        public void Validate(string[] a_eventNames)
        {
            if (a_eventNames == null)
                return;

            if (EventName == null || EventName == "")
            {
                if(EventId < 0 || EventId >= a_eventNames.Length)
                {
                    EventId = -1;
                    EventName = "";
                }
                else
                {
                    EventName = a_eventNames[EventId];
                }
            }
            else
            {
                //Find the Id from the name
                bool found = false;
                for(int i = 0; i < a_eventNames.Length; ++i)
                {
                    if(a_eventNames[i] == EventName)
                    {
                        found = true;
                        EventId = i;
                        break;
                    }
                }

                //If no id of that name is found reset the event to null
                if(!found)
                {
                    Debug.LogWarning("Event marker name: '" + EventName + "' does not exist. Have you deleted and event Id that was being used by an event marker?" +
                        "The marker event Id and name has been set to null.");
                    EventId = -1;
                }
            }
        }

    }//End of class: EventMarker
}//End of namespace: MxM
