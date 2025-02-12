using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Game Object class is responsible for managing game object's components.
    /// </summary>
    public class GameObject
    {
        private string _tag;
        public string Tag { get { return _tag; } set { _tag = value; } }
        private List<Component> components = new List<Component>();
        private List<Component> toRemove = new List<Component>();
        /// <summary>
        /// The game object constructor uses a tag to identify the game object.
        /// </summary>
        /// <param name="tag"></param>
        public GameObject(string tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// Add Component method adds a component to the game object and sets the game object reference.
        /// </summary>
        /// <param name="component"></param>
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
        /// <summary>
        /// Get Component method returns a component from a generic type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Remove Component method removes a component from the game object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
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

        /// <summary>
        /// Update Components method updates all components attached to the game object.
        /// </summary>
        public void UpdateComponents()
        {
            // Update all components.
            foreach (Component component in components)
            {
                component.Update();
            }
        }

        /// <summary>
        /// Draw method draws all components attached to the game object, if applicable.
        /// </summary>
        /// <param name="spriteBatch"></param>
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
        /// <summary>
        /// Update method calls update component on all components attached to the game object.
        /// </summary>
        public virtual void Update()
        {
            // Updates all components attached to this game object.
            UpdateComponents();
        }
    }
}
