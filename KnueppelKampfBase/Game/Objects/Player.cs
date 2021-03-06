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
        
        public Player()
        {
            this.position = default(Vector);
            Color[] colors = new Color[] { Color.White, Color.Green, Color.Azure, Color.Orange };
            this.size = new Vector(50, 100);
            color = colors[this.Id % colors.Length];
            this.AddComponent(new HealthComponent(10.0f));
            this.AddComponent(new MoveComponent());
            this.AddComponent(new BoxComponent());
            this.AddComponent(new ControlComponent());
            this.AddComponent(new ItemComponent());
            this.AddComponent(new PlayerAnimationComponent());
        }

        public Player(Vector position = default) : this()
        {
            this.position = position;
        }

        public override void OnRender()
        {
            StateManager.Push();
            StateManager.SetColor(Color.Green);
            //von der mitte des objektes wird rotiert
            StateManager.Translate(Position + (PrevPosition - position) * StateManager.partialTicks + Size / 2);
            //NOTE: in umgekehrte richtung, damit es keine probleme gibt, falls während des durchgangs ein element entfernt wird
            for (int i = Components.Count - 1; i >= 0; i--)
            {
                StateManager.Push();
                Components[i].OnRender();
                StateManager.Pop();
            }

            StateManager.Pop();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
