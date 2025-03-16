using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// The TurnManager class is responsible for managing the turn-based system of the game.
    /// </summary>
    public static class TurnManager
    {
        public static Queue<TurnComponent> TurnQueue { get { return _turnQueue; } }
        private static Queue<TurnComponent> _turnQueue = new Queue<TurnComponent>();
        private static bool _isTurnInProgress = false;
        private static int _turnDelayMilliseconds = 250;
        private static readonly object _queueLock = new object();
        private static TurnComponent _currentParticipant = null;

        /// <summary>
        /// Returns a current participant's name. To be used in other classes for UI purposes.
        /// </summary>
        public static string CurrentParticipant
        {
            get
            {
                try
                {
                    lock (_queueLock)
                    {
                        return _currentParticipant.Name;
                    }
                }
                catch (InvalidOperationException)
                {
                    return null; 
                }
            }
        }
        /// <summary>
        /// Populates the turn queue with all objects that have a TurnComponent.
        /// </summary>
        public static void PopulateQueue()
        {
            _turnQueue = ObjectManager.ReturnAll();
        }

        /// <summary>
        /// Subscribes a Game Object to the OnBeforeDestroy event. This removes the GameObject from the turn queue when it is destroyed.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void SubscribeToDestroy(GameObject gameObject)
        {
            gameObject.OnBeforeDestroy += DequeueAndUnsubscribe;
        }

        /// <summary>
        /// This method is called when a GameObject is destroyed. It removes the GameObject from the turn queue.
        /// </summary>
        /// <param name="gameObject"></param>
        private static void DequeueAndUnsubscribe(GameObject gameObject)
        {
            TurnComponent turnComponentInstance = gameObject.GetComponent<TurnComponent>();
            if (turnComponentInstance != null)
            {
                DequeueParticipant(turnComponentInstance);
            }
            gameObject.OnBeforeDestroy -= DequeueAndUnsubscribe;
        }

        /// <summary>
        /// This method starts the turn cycle. It will populate the queue if it's empty and start the first turn.
        /// </summary>
        public static async void StartTurnCycle()
        {
            if (_turnQueue.Count == 0)
            {
                PopulateQueue();
            }
            NextTurn();
        }

        /// <summary>
        /// This method is called when a participant has finished their turn. It will move the participant to the back of the queue and start the next turn.
        /// </summary>
        public static void NextTurn()
        {
            lock (_queueLock)
            {
                if (_turnQueue.Count == 0)
                {
                    PopulateQueue();
                }

                while (_turnQueue.Count > 0)
                {
                    _currentParticipant = _turnQueue.Dequeue();

                    // Check if the current participant is still valid and active.
                    if (_currentParticipant != null && _currentParticipant.IsActive)
                    {
                        _isTurnInProgress = true;
                        _currentParticipant.TakeTurn();
                        _turnQueue.Enqueue(_currentParticipant);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This method is called when a participant has finished their turn. It will move the participant to the back of the queue and start the next turn.
        /// </summary>
        public static async void EndTurn()
        {
            await Task.Delay(_turnDelayMilliseconds);
            // Check if the turn is still in progress before starting the next turn.
            if (_isTurnInProgress)
            {
                _isTurnInProgress = false;
                NextTurn();
            }
        }

        /// <summary>
        /// This method dequeues a participant from the turn queue. It is called when a participant is destroyed.
        /// </summary>
        /// <param name="participant"></param>
        public static void DequeueParticipant(TurnComponent participant)
        {
            lock (_queueLock)
                if (participant == null || _turnQueue == null)
            {
                return; // Handle null cases.
            }
            // Create a new queue without the participant.
            Queue<TurnComponent> newQueue = new Queue<TurnComponent>(_turnQueue.Where(p => p != participant));
            _turnQueue = newQueue;
        }

    }
}

  