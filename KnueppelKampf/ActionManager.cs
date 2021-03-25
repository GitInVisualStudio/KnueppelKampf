using KnueppelKampfBase.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

namespace KnueppelKampf
{
    /// <summary>
    /// Class for getting user inputs from keyboard
    /// </summary>
    class ActionManager
    {
        /// <summary>
        /// Dictionary mapping buttons to actions
        /// </summary>
        private static Dictionary<Key, GameAction> bindings = new Dictionary<Key, GameAction>()
        {
            { Key.Space, GameAction.Jump },
            { Key.A, GameAction.MoveLeft },
            { Key.D, GameAction.MoveRight},
            { Key.LeftCtrl, GameAction.Duck},
            { Key.T, GameAction.PrimaryUse},
            { Key.H, GameAction.SecondaryUse}
        };

        /// <summary>
        /// Binds a key to an action
        /// </summary>
        public static void Bind(Key key, GameAction action)
        {
            KeyValuePair<Key, GameAction> assigned = bindings.FirstOrDefault(x => x.Value == action);
            if (!assigned.Equals(default(KeyValuePair<Key, GameAction>))) // there shouldnt be two buttons mapping to one action
                bindings.Remove(assigned.Key);
            bindings[key] = action;
        }

        /// <summary>
        /// Returns an array of actions to execute
        /// </summary>
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
