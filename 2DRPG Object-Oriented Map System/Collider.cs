using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Collider : Component
{
        public bool IsTrigger
        {
            get { return _isTrigger; }
            set { _isTrigger = value; }
        }
        private bool _isTrigger;

        public void DrawRectangle(int width, int height)
        {
            // Draw a rectangle.

        }

        public void UpdateBounds()
        {

        }

        public bool Intersects(Collider other)
        {
            return false;
        }

        public override void Update()
        {
            // Update the collider.
        }
}
}
