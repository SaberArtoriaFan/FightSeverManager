using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public class TimerSystemModel :SystemModelBase
    {

        private readonly List<Timer> updateList = new List<Timer>();
        private readonly Queue<Timer> availableQueue = new Queue<Timer>();

        public List<Timer> UpdateList => updateList;

        public Queue<Timer> AvailableQueue => availableQueue;
    }
}
