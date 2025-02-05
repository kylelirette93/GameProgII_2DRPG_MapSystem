using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public abstract class Component
{
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        private bool isActive;

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
