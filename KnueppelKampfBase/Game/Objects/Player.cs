﻿using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace KnueppelKampfBase.Game.Objects
{
    public class Player : GameObject
    {
        private Color color;
        public Color Color { get => color; set => color = value; }

        public Player(Vector position = default)
        {
            this.position = position;
            this.size = new Vector(50, 100);
            color = Color.Black;
            this.AddComponent(new MoveComponent());
            this.AddComponent(new BoxComponent());
            this.AddComponent(new PlayerAnimationComponent(this));
            this.AddComponent(new ControlComponent(this));
        }

        public override void OnRender()
        {
            StateManager.SetColor(Color.Red);
            StateManager.DrawRect(this.position, this.size);
            StateManager.Push();
            //von der mitte des objektes wird rotiert
            StateManager.Translate(Position + (prevPosition - position) * StateManager.partialTicks + Size / 2);
            StateManager.Rotate(Rotation);
            //NOTE: in umgekehrte richtung, damit es keine probleme gibt, falls während des durchgangs ein element entfernt wird
            for (int i = Components.Count - 1; i >= 0; i--)
                Components[i].OnRender();

            StateManager.Pop();

        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
