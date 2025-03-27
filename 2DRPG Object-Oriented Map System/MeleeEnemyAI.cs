using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class MeleeEnemyAI : BaseEnemyAI
    {
        public MeleeEnemyAI(string name) : base(name)
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
