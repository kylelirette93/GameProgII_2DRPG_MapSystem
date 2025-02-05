using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Transform : Component
{
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        private Vector2 _position;
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
        private float _rotation;
        public Vector2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        private Vector2 _scale;

        void Translate(Vector2 translation)
        {
            _position += translation;
        }

        public override void Update()
        {
            
        }
    }
}
