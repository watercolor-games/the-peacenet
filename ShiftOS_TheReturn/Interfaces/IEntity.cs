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
    public interface IEntity
    {
        void Update(GameTime time);
        void Draw(GameTime time, GraphicsContext gfx);
        void OnKeyEvent(KeyboardEventArgs e);
        void OnMouseUpdate(MouseState mouse);
    }

    public class EntityContainer : IEntity, IDisposable
    {
        private List<IEntity> _entities = new List<IEntity>();

        public IEntity[] Entities
        {
            get
            {
                return _entities.ToArray();
            }
        }

        public void AddEntity(IEntity entity)
        {
            if (entity == null)
                return;
            if (_entities.Contains(entity))
                return;
            _entities.Add(entity);
        }

        public void RemoveEntity(IEntity entity)
        {
            if (entity == null)
                return;
            if (!_entities.Contains(entity))
                return;
            _entities.Remove(entity);
        }

        public void ClearEntities()
        {
            _entities.Clear();
        }

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


        public virtual void Draw(GameTime time, GraphicsContext gfx)
        {
            foreach (var entity in Entities)
                entity.Draw(time, gfx);
        }

        public virtual void OnKeyEvent(KeyboardEventArgs e)
        {
            foreach (var entity in Entities)
                entity.OnKeyEvent(e);
        }

        public virtual void OnMouseUpdate(MouseState mouse)
        {
            foreach (var entity in Entities)
                entity.OnMouseUpdate(mouse);
        }

        public virtual void Update(GameTime time)
        {
            foreach (var entity in Entities)
                entity.Update(time);
        }

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
