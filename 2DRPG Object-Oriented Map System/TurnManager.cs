using _2DRPG_Object_Oriented_Map_System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Turn Manager class is responsible for managing the turn-based system.
/// </summary>
public class TurnManager
{
    /// <summary>
    /// Create a queue of turn takers.
    /// </summary>
    public Queue<ITurnTaker> TurnQueue { get => turnQueue; }
    private Queue<ITurnTaker> turnQueue = new Queue<ITurnTaker>();
    private ITurnTaker currentTurnTaker;
    private bool isTurnActive = false;
    float waitingTime = 0f;

    /// <summary>
    /// The current turn taker's ID.
    /// </summary>
    public string CurrentTurnId { get; private set; }

    public static TurnManager Instance { get; private set; }

    private TurnManager()
    {

    }
    static TurnManager()
    {
        Instance = new TurnManager();
    }

    /// <summary>
    /// This method add's a turn taker to the queue.
    /// </summary>
    /// <param name="turnTaker"></param>
    public void AddTurnTaker(ITurnTaker turnTaker)
    {
        foreach (ITurnTaker turnTaker1 in turnQueue)
        {
            if (turnTaker.Id == turnTaker1.Id)
            {
                Debug.WriteLine($"Duplicate turn taker found: {turnTaker.Id}");
                return;
            }
        }
        turnQueue.Enqueue(turnTaker);      
    }

    /// <summary>
    /// This method removes a turn taker from the queue.
    /// </summary>
    /// <param name="turnTaker"></param>
    public void RemoveTurnTaker(ITurnTaker turnTaker)
    {
        turnQueue = new Queue<ITurnTaker>(turnQueue.Where(t => !t.Equals(turnTaker)));
    }

    /// <summary>
    /// This method updates the turn cycle.
    /// </summary>
    /// <param name="gameTime"></param>
    public void UpdateTurn(GameTime gameTime)
    {
        if (!isTurnActive && turnQueue.Count > 0)
        {
            // Get the next turn taker from the queue if there is no active turn.
            currentTurnTaker = turnQueue.Dequeue();
            isTurnActive = true;
            CurrentTurnId = currentTurnTaker.Id;

            // Start the turn taker's turn.
            //Debug.WriteLine($"{currentTurnTaker} is starting its turn.");
            currentTurnTaker.StartTurn(); 
        }

        // Check if the current turn taker has finished their turn.
        if (currentTurnTaker != null && !currentTurnTaker.IsTurn)
        {
            waitingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (waitingTime > 0.25f)
            {
                //Debug.WriteLine($"{currentTurnTaker} has finished its turn.");
                isTurnActive = false;

                // Add the turn taker back to the queue.
                turnQueue.Enqueue(currentTurnTaker);
                // Reset the current turn taker so the next turn taker can take its turn.
                currentTurnTaker = null;
                CurrentTurnId = null;
                waitingTime = 0f;
            }
        }
    }
}