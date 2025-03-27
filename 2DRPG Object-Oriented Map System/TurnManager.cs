using _2DRPG_Object_Oriented_Map_System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class TurnManager
{
    public Queue<ITurnTaker> TurnQueue { get => turnQueue; }
    private Queue<ITurnTaker> turnQueue = new Queue<ITurnTaker>();
    private ITurnTaker currentTurnTaker;
    private bool isTurnActive = false;
    float waitingTime = 0f;
    public string CurrentTurnId { get; private set; }

    public static TurnManager Instance { get; private set; }

    private TurnManager()
    {

    }
    static TurnManager()
    {
        Instance = new TurnManager();
    }

    // Add a turn taker.
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

    public void RemoveTurnTaker(ITurnTaker turnTaker)
    {
        turnQueue = new Queue<ITurnTaker>(turnQueue.Where(t => !t.Equals(turnTaker)));
    }

    public void UpdateTurn(GameTime gameTime)
    {
        // Only process the next turn if no other turn is active
        if (!isTurnActive && turnQueue.Count > 0)
        {
            // Get the next turn taker from the queue
            currentTurnTaker = turnQueue.Dequeue();
            isTurnActive = true;
            CurrentTurnId = currentTurnTaker.Id;

            // Start the turn
            Debug.WriteLine($"{currentTurnTaker} is starting its turn.");
            currentTurnTaker.StartTurn(); 
        }

        // Check if the current turn taker has finished their turn
        if (currentTurnTaker != null && !currentTurnTaker.IsTurn)
        {
            waitingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (waitingTime > 0.01f)
            {
                // End the current turn and reset the turn state
                Debug.WriteLine($"{currentTurnTaker} has finished its turn.");
                isTurnActive = false;

                turnQueue.Enqueue(currentTurnTaker);
                currentTurnTaker = null;
                CurrentTurnId = null;
                waitingTime = 0f;
            }
        }
    }
}