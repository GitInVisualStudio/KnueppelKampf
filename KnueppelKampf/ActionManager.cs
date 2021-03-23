using KnueppelKampfBase.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace KnueppelKampf
{
    class ActionManager
    {
        private static Dictionary<Key, GameAction> bindings = new Dictionary<Key, GameAction>()
        {
            { Key.Space, GameAction.Jump },
            { Key.A, GameAction.MoveLeft },
            { Key.D, GameAction.MoveRight},
            { Key.LeftCtrl, GameAction.Duck},
            { Key.T, GameAction.PrimaryUse},
            { Key.H, GameAction.SecondaryUse}
        };

        public static void Bind(Key key, GameAction action)
        {
            KeyValuePair<Key, GameAction> assigned = bindings.FirstOrDefault(x => x.Value == action);
            if (!assigned.Equals(default(KeyValuePair<Key, GameAction>)))
                bindings.Remove(assigned.Key);
            bindings[key] = action;
        }

        public static GameAction[] GetActions()
        {
            List<GameAction> actions = new List<GameAction>();
            foreach (Key k in bindings.Keys)
            {
                KeyStates state = Keyboard.GetKeyStates(k);
                if ((state & KeyStates.Down) > 0)
                    actions.Add(bindings[k]);
            }
            return actions.ToArray();
        }
    }
}
