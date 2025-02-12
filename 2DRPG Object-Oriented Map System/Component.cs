using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Component is an abstract class that all components inherit from. It is used to enforce a common interface for all components.
    /// </summary>
    public abstract class Component
{
        // Different components inherit from this class.

        // Property for the game object this component is attached to.
        /// <summary>
        /// Game object property for the component.
        /// </summary>
        public GameObject GameObject { get; private set; }
       
        /// <summary>
        /// Set Game Object method is responsible for setting the game object for the component.
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetGameObject(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        /// <summary>
        /// Update method is responsible for updating the component.
        /// </summary>
        public virtual void Update()
        {
            // This method is overridden by the inheriting class.
        }
}
}
