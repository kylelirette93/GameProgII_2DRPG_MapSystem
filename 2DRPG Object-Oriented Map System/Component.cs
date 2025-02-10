using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public abstract class Component
{
        // Different components inherit from this class.

        // Property for the game object this component is attached to.
        public GameObject GameObject { get; private set; }
       

        public void SetGameObject(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public virtual void Update()
        {
            // Enforces all subclasses to inherit this method. Kinda like an interface but not really.
        }
}
}
