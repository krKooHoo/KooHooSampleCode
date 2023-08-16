using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Scheduler : MonoBehaviour
{
    public class ScheduleEvent
    {
        public DateTime Time;
        public Action<object> CallBackChain;
        public object Parameter;
    }
    
    public class ScheduleEventCompare : IComparer<ScheduleEvent>
    {
        public int Compare(ScheduleEvent x, ScheduleEvent y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return -1 * x.Time.CompareTo(y.Time); //오름차순 전환
        }
    }
    
    public class Plan : ScheduleEvent
    {
        public float Interval { get; set; }
    }

    private PriorityQueue<ScheduleEvent> ScheduleQueue = new PriorityQueue<ScheduleEvent>(new ScheduleEventCompare());
    
    void Update()
    {
        ScheduleEvent peekEvent = PeekEventOrNull();
        while (peekEvent != null && peekEvent.Time <= DateTime.UtcNow) 
        { 
            peekEvent.CallBackChain?.Invoke(peekEvent.Parameter);
            ScheduleQueue.Dequeue();
            peekEvent = PeekEventOrNull();
        }

        ScheduleEvent PeekEventOrNull()
        {
            if (ScheduleQueue.IsEmpty()) return null;
            return ScheduleQueue.Peek();
        }
    }

    public void RegisterSchedule(ScheduleEvent scheduleEvent)
    {
        if (scheduleEvent is Plan plan)
        {
            Action<object> registerDelegate = o =>
            {
                plan.Time = DateTime.UtcNow + TimeSpan.FromSeconds(plan.Interval);
                ScheduleQueue.Enqueue(plan);
            };
            
            registerDelegate.Invoke(null);
            plan.CallBackChain += registerDelegate;
            return;
        }
        ScheduleQueue.Enqueue(scheduleEvent);
    }
}
