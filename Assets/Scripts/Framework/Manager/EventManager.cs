using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EventHandler(object args);
    public Dictionary<int,EventHandler> Events= new Dictionary<int, EventHandler>();
    public void Subscribe(int id,EventHandler handler)
    {
        if(Events.ContainsKey(id)) Events[id] += handler;
        else Events.Add(id,handler);
    }
    public void UnSubscribe(int id, EventHandler handler)
    {
        if (Events.ContainsKey(id))
        {
            if (Events[id] != null) Events[id] -= handler;
            else Events.Remove(id);
        }
    }
    public void Fire(int id,object args = null)
    {
        EventHandler @event = null;
        if (Events.TryGetValue(id,out @event))
        {
            @event?.Invoke(args);
        }
    }
}
