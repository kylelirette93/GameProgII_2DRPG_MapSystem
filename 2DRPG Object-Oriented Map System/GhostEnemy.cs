using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class GhostEnemy : BaseEnemyAI
    {
        public bool CanDamage { get; set; }
        bool canDamage = false;
        public GhostEnemy(string name) : base(name)
        {
        }    
    }
}
