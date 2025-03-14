using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public static class TurnManager
    {
        public static Queue<TurnComponent> TurnQueue { get { return _turnQueue; } }
        private static Queue<TurnComponent> _turnQueue = new Queue<TurnComponent>();
        private static bool _isTurnInProgress = false;
        private static int _turnDelayMilliseconds = 100;


        public static void PopulateQueue()
        {
            _turnQueue = ObjectManager.ReturnAll();
        }

        public static async void StartTurnCycle()
        {
            if (_turnQueue.Count == 0)
            {
                PopulateQueue();
            }
            NextTurn();
        }
        public static void NextTurn()
        {
            if (_turnQueue.Count == 0)
            {
                PopulateQueue();
            }

            if (_turnQueue.Count > 0)
            {
                TurnComponent currentParticipant = _turnQueue.Dequeue();
                _isTurnInProgress = true;
                currentParticipant.TakeTurn();
                _turnQueue.Enqueue(currentParticipant);
            }
        }

        public static async void EndTurn()
        {
            await Task.Delay(_turnDelayMilliseconds);
            if (_isTurnInProgress)
            {
                _isTurnInProgress = false;
                NextTurn();
            }
        }
        public static void DequeueParticipant(TurnComponent participant)
        {
            if (participant == null || _turnQueue == null)
            {
                return; // Handle null cases.
            }

            // Create a new queue without the participant.
            _turnQueue = new Queue<TurnComponent>(_turnQueue.Where(p => p != participant));
        }

    }
}

  