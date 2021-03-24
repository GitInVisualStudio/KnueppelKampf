    using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    /// <summary>
    /// dient zur verarbeitung der eingaben um den spieler zu steuern
    /// </summary>
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
            ControlState bs = (ControlState)state;
            this.blocking = bs.Blocking;
            return;
        }

        public override ComponentState GetState()
        {
            return new ControlState() { Blocking = blocking };
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
            //alle aktionen des spielers verarbeiten
            foreach (GameAction a in gameActions)
                this.HandleInpute(a);
        }
    }

    public class ControlState : ComponentState
    {
        private static Type componentType = typeof(ControlComponent);

        [DontSerialize]
        public static Type ComponentType { get => componentType; set => componentType = value; }
        
        private bool blocking;

        public bool Blocking { get => blocking; set => blocking = value; }

        public override int ToBytes(byte[] array, int startIndex)
        {
            GetHeader(array, startIndex);
            array[startIndex + 1] = (byte)(blocking ? 0 : 1);
            return HEADER_SIZE + 1;
        }

        public override GameComponent ToComponent()
        {
            return new ControlComponent();
        }

        public static int FromBytes(byte[] bytes, int startIndex, out ControlState cs)
        {
            cs = new ControlState();
            cs.Blocking = bytes[startIndex] > 0;
            return 1;
        }
    }
}