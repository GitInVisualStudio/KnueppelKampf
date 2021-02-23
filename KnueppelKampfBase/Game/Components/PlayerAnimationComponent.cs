using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class PlayerAnimationComponent : GameComponent
    {
        private Player player;
        private GameComponent move;
        public PlayerAnimationComponent(Player playerObject)
        {
            this.player = playerObject;
            this.move = playerObject.GetComponent<MoveComponent>();
            if(move == null)
            {
                throw new Exception($"GameObject: {playerObject} hat kein component {typeof(MoveComponent)}");
            }
        }

        public override void OnRender()
        {
            StateManager.SetColor(player.Color);
            StateManager.FillCircle(0, -50, 25);
            StateManager.FillRect(0, 0, player.Size.X, player.Size.Y);
        }

        public override void OnUpdate()
        {
            //throw new NotImplementedException();
        }
    }
}
