using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class GameObject
    {
        // Generic list of components.
        private List<Component> components = new List<Component>();

        public Vector2 Position { get; set; }
        private Vector2 _position;

        public bool IsActive { get; set; }
        private bool _isActive;

        public void OnEnable()
        {
            IsActive = true;
        }

        public void OnDisable()
        {
            IsActive = false;
        }

        public void Update()
        {

        }

        public void AddComponent(Component component)
        {
            components.Add(component);
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component is T tComponent)
                {
                    return tComponent;
                }
            }
            return null;
        }

        public void RemoveComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component is T)
                {
                    components.Remove(component);
                }
            }
        }

        public void UpdateComponents()
        {
            foreach (Component component in components)
            {
                component.Update();
            }
        }
    }

}
