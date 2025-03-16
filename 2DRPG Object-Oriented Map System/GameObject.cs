using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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


        public event Action<GameObject> OnBeforeDestroy;
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
            components.RemoveAll(component => component is T);
        }

        /// <summary>
        /// Update Components method updates all components attached to the game object.
        /// </summary>
        public void UpdateComponents()
        {
            foreach (Component component in toRemove)
            {
                components.Remove(component);
            }
            toRemove.Clear();

            // Update all components.
            foreach (Component component in components)
            {
                component.Update();
            }
        }

        /// <summary>
        /// Destroy method destroys the game object, removes the turn component and removes it from the object manager.
        /// </summary>
        public void Destroy()
        {
            OnBeforeDestroy?.Invoke(this);
            TurnComponent turnComponentInstance = GetComponent<TurnComponent>();
            if (turnComponentInstance != null)
            {
                Debug.WriteLine($"Retrieved component: {turnComponentInstance.GetType().Name}");
                TurnManager.DequeueParticipant(turnComponentInstance);
            }

            foreach (Component component in components)
            {
                toRemove.Add(component);
            }
            ObjectManager.RemoveGameObject(this);
        }

        /// <summary>
        /// Draw method draws all components attached to the game object, if applicable.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var component in components)
            {
                if (component is DrawableComponent)
                {
                    (component as DrawableComponent).Draw(spriteBatch);
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
