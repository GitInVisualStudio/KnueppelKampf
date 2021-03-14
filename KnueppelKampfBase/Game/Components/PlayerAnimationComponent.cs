﻿using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;

namespace KnueppelKampfBase.Game.Components
{
    public class PlayerAnimationComponent : GameComponent
    {
        private Player player;
        private MoveComponent move;
        private float state;
        private float leftLeg, rightLeg;
        public PlayerAnimationComponent(Player playerObject)
        {
            this.player = playerObject;
            this.move = playerObject.GetComponent<MoveComponent>();
            if(move == null)
            {
                throw new Exception($"GameObject: {playerObject} hat kein component {typeof(MoveComponent)}");
            }
        }

        public override void ApplyState(ComponentState state)
        {
            throw new NotImplementedException();
        }

        public override ComponentState GetState()
        {
            throw new NotImplementedException();
        }

        public override void OnRender()
        {
            //StateManager.Translate(player.Size.X / 2, 0);
            state += move.Length * StateManager.delta * 10;
            StateManager.SetColor(player.Color);
            int headSize = 25;
            StateManager.FillCircle(0, -player.Size.Y / 2 - (float)(-Sin(state) * 2), headSize);
            float width = player.Size.X / 5;
            float height = player.Size.Y;
            StateManager.FillRoundRect(-width / 2, -height/ 2, width, height / 2, 5, 10);
            float rot = (float)Sin(state) * 30;
            rightLeg += (rot - rightLeg) / 4 * StateManager.delta * 30;
            StateManager.Rotate(rightLeg);
            StateManager.FillRoundRect(-width / 2, -6, width, height / 2, 5, 10);
            StateManager.Rotate(-rightLeg);
            rot = (float)Sin(-state) * 30; //new value of leg rotation
            leftLeg += (rot - leftLeg) / 4 * StateManager.delta * 30;
            StateManager.Rotate(leftLeg);
            StateManager.FillRoundRect(-width / 2, -6, width, height / 2, 5, 10);
            StateManager.Rotate(-leftLeg);
        }

        public override void OnUpdate()
        {
            //throw new NotImplementedException();
        }
    }
}
