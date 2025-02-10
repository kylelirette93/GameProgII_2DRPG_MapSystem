using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class GameObject
    {
        // Tag property for game object.
        private string _tag;
        public string Tag { get { return _tag; } set { _tag = value; } }
        private List<Component> components = new List<Component>();
        private List<Component> toRemove = new List<Component>();
        public GameObject(string tag)
        {
            Tag = tag;
        }

        public void AddComponent(Component component)
        {
            if (component != null)
            {
                component.SetGameObject(this);
                components.Add(component);
            }
            else
            {
                Console.WriteLine("Component is null!");
            }
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
            // Collect components to remove.
            foreach (Component component in components)
            {
                if (component is T)
                {
                    toRemove.Add(component);
                }
            }

            // Remove components outside of the loop to avoid modification during iteration.
            foreach (Component component in toRemove)
            {
                components.Remove(component);
            }
        }

        public void UpdateComponents()
        {
            // Update all components.
            foreach (Component component in components)
            {
                component.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var component in components)
            {
                if (component is Sprite sprite)
                {
                    sprite.Draw(spriteBatch);
                }
                else if (component is Tilemap tilemap)
                {
                    tilemap.Draw(spriteBatch);
                }
                else if (component is Collider collider)
                {
                    collider.Draw(spriteBatch);
                }
            }
        }
        public virtual void Update()
        {
            // Updates all components attached to this game object.
            UpdateComponents();
        }
    }
}
