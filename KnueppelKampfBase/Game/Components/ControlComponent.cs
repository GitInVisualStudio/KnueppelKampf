using KnueppelKampfBase.Game.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class ControlComponent : GameComponent
    {
        private MoveComponent move;
        public ControlComponent(Player playerObject)
        {
            this.move = playerObject.GetComponent<MoveComponent>();
            if (move == null)
                throw new Exception($"GameObject: {playerObject} hat kein component {typeof(MoveComponent)}");
        }

        public override void ApplyState(ComponentState state)
        {
        }

        public override ComponentState GetState()
        {
            return null;
        }

        public override void OnRender()
        {
        }

        public override void OnUpdate()
        {
        }

        public void HandleInpute(GameAction action)
        {
            
            switch (action)
            {
                //TODO: find a neat way to implement jumping => onGround problem
                case GameAction.Duck:
                    break;
                case GameAction.Jump:
                    move.Y = -1;
                    break;
                case GameAction.MoveLeft:
                    move.X = -1;
                    break;
                case GameAction.MoveRight:
                    move.X = 1;
                    break;
                case GameAction.PrimaryUse:
                    break;
                case GameAction.SecondaryUse:
                    //weiß nicht ob wir das überhaupt mal brauchen werden
                    break;
            }
        }

        public void HandleInputs(GameAction[] gameActions)
        {
            foreach (GameAction a in gameActions)
                this.HandleInpute(a);
        }
    }
}
