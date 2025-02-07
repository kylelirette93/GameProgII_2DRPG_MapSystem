using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class GameObject
    {
        // Tag property for game object.
        public string Tag { get; private set; }
        private List<Component> components = new List<Component>();
        public GameObject(string tag)
        {
            Tag = tag;
        }

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

        public void AddComponent(Component component)
        {
            component.SetGameObject(this);
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
            UpdateComponents();
        }
    }

}
