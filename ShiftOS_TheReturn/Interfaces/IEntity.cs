using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Plex.Engine.Interfaces
{
    /// <summary>
    /// Represents a Peace engine entity which can receive user input, update each frame and render to the screen.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Update the entity.
        /// </summary>
        /// <param name="time">The time since the last frame.</param>
        void Update(GameTime time);
        /// <summary>
        /// Render the entity.
        /// </summary>
        /// <param name="time">The time since the last frame.</param>
        /// <param name="gfx">A <see cref="GraphicsContext"/> for rendering to the screen.</param>
        void Draw(GameTime time, GraphicsContext gfx);
        /// <summary>
        /// Handle a keyboard event with this entity.
        /// </summary>
        /// <param name="e">The <see cref="KeyboardEventArgs"/> containing information about the event.</param>
        void OnKeyEvent(KeyboardEventArgs e);
        /// <summary>
        /// Handle a mouse event with this entity.
        /// </summary>
        /// <param name="mouse">A <see cref="MouseState"/> containing information about the mouse's current state.</param>
        void OnMouseUpdate(MouseState mouse);
    }

    /// <summary>
    /// Represents an <see cref="IEntity"/> which can contain child entities. 
    /// </summary>
    public class EntityContainer : IEntity, IDisposable
    {
        private List<IEntity> _entities = new List<IEntity>();

        /// <summary>
        /// Retrieves an array of all child entities.
        /// </summary>
        public IEntity[] Entities
        {
            get
            {
                return _entities.ToArray();
            }
        }

        /// <summary>
        /// Adds a child entity to this entity container.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void AddEntity(IEntity entity)
        {
            if (entity == null)
                return;
            if (_entities.Contains(entity))
                return;
            _entities.Add(entity);
        }

        /// <summary>
        /// Removes a child entity from this entity container.
        /// </summary>
        /// <param name="entity">The child entity to remove.</param>
        public void RemoveEntity(IEntity entity)
        {
            if (entity == null)
                return;
            if (!_entities.Contains(entity))
                return;
            _entities.Remove(entity);
        }

        /// <summary>
        /// Clears all child entities from this entity container.
        /// </summary>
        public void ClearEntities()
        {
            _entities.Clear();
        }

        /// <summary>
        /// Send a child entity to the back of the entity list.
        /// </summary>
        /// <param name="entity">The child entity to move.</param>
        public void SendToBack(IEntity entity)
        {
            if (entity == null)
                return;
            if (!_entities.Contains(entity))
                return;
            if (_entities.IndexOf(entity) == 0)
                return;
            RemoveEntity(entity);
            _entities.Insert(0, entity);
        }

        /// <summary>
        /// Bring an entity to the front of the child entity list.
        /// </summary>
        /// <param name="entity">The entity to move.</param>
        public void BringToFront(IEntity entity)
        {
            if (entity == null)
                return;
            if (!_entities.Contains(entity))
                return;
            if (_entities.IndexOf(entity) == _entities.Count - 1)
                return;
            RemoveEntity(entity);
            AddEntity(entity);
        }

        /// <inheritdoc/>
        public virtual void Draw(GameTime time, GraphicsContext gfx)
        {
            foreach (var entity in Entities)
                entity.Draw(time, gfx);
        }

        /// <inheritdoc/>
        public virtual void OnKeyEvent(KeyboardEventArgs e)
        {
            foreach (var entity in Entities)
                entity.OnKeyEvent(e);
        }

        /// <inheritdoc/>
        public virtual void OnMouseUpdate(MouseState mouse)
        {
            foreach (var entity in Entities)
                entity.OnMouseUpdate(mouse);
        }

        /// <inheritdoc/>
        public virtual void Update(GameTime time)
        {
            foreach (var entity in Entities)
                entity.Update(time);
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            while(_entities.Count>0)
            {
                var entity = _entities[0];
                if (entity is IDisposable)
                    (entity as IDisposable).Dispose();
                _entities.Remove(entity);
            }
        }
    }
}
