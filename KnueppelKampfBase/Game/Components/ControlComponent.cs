﻿    using KnueppelKampfBase.Game.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class ControlComponent : GameComponent
    {
        private MoveComponent move;
        private bool blocking;
        public bool Blocking { get => blocking; set => blocking = value; }

        public ControlComponent()
        {
        }

        public override void Init()
        {
            base.Init();
            this.move = GameObject.GetComponent<MoveComponent>();
            if (move == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(MoveComponent)}");
        }

        public override void ApplyState(ComponentState state)
        {
            return;
        }

        public override ComponentState GetState()
        {
            return null;
        }

        public override void OnRender()
        {
            //return null;
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
                    if(move.OnGround)
                        move.Y = -5f;
                    break;
                case GameAction.MoveLeft:
                    move.X = -2f;
                    break;
                case GameAction.MoveRight:
                    move.X = 2f;
                    break;
                case GameAction.PrimaryUse:
                    ItemComponent item = GameObject.GetComponent<ItemComponent>();
                    if (item == null)
                        return;
                    item.Attack();
                    break;
                case GameAction.SecondaryUse:
                    //weiß nicht ob wir das überhaupt mal brauchen werden
                    blocking = true;
                    break;
            }
        }

        public void HandleInputs(GameAction[] gameActions)
        {
            blocking = false;

            foreach (GameAction a in gameActions)
                this.HandleInpute(a);
        }
    }
}