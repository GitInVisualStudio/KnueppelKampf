using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game.Objects
{
    public class Floor : GameObject
    {
        public Floor(Vector position, Vector size)
        {
            this.position = position;
            this.size = size;
            AddComponent(new BoxComponent(HandleCollision));
        }

        private void HandleCollision(BoxComponent b)
        {
            if (b.GameObject is Floor)
                return;
            GameObject obj = b.GameObject;
            MoveComponent move = obj.GetComponent<MoveComponent>();
            if (move == null)
                return;
            obj.Position -= move.Velocity * 10f;
            move.Velocity = default;
        }

        public override void OnRender()
        {
            StateManager.SetColor(Color.Black);
            StateManager.FillRect(Position, Size);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
