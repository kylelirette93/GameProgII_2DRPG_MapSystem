using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// This interface is responsible for handling the turn based system. Enforcing implementation of turn based methods.
    /// </summary>
    public interface ITurnTaker
    {
        void StartTurn();
        void TakeTurn();
        void EndTurn();
        string Id { get; }
        bool IsTurn { get; }
    }
}
