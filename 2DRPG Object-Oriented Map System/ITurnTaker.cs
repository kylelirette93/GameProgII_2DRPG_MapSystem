using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public interface ITurnTaker
    {
        void StartTurn();
        void TakeTurn();
        void EndTurn();
        string Id { get; }
        bool IsTurn { get; }
    }
}
