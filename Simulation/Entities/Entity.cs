using System.Numerics;

namespace Simulation.Entities
{
    public abstract class Entity //: IWPFDrawable
    {
        protected Entity(float x, float y, World world)
        {
            _position = new Vector2(x, y);
            World = world;

            CreatedAt = DateTime.Now;
        }

        protected Entity(Vector2 position, World world) : this(position.X, position.Y, world)
        {
        }

        // TODO Maybe change Vector2 to something (or implement something) that uses double for greater precision
        private Vector2 _position;

        public virtual Vector2 Position
        {
            get => new Vector2(_position.X, _position.Y);
            protected set => _position = value;
        }

        public World World { get; }

        public DateTime CreatedAt { get; }

        public bool IsHidden { get; set; }

        // TODO IWPFDrawable - move
        //public abstract void DrawSelf(Canvas canvas);
    }
}