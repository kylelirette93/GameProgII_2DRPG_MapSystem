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
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        private bool isActive;

        public void SetGameObject(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public void OnEnable()
        {
            isActive = true;
        }

        public void OnDisable()
        {
            isActive = false;
        }

        public virtual void Update()
        {
            if (isActive)
            {
                // Do stuff here.
            }
        }
}
}
