using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    /// <summary>
    /// Enum representing a GameAction. Numbers are chosen for easy serialization
    /// </summary>
    public enum GameAction
    {
        None = 0,
        MoveLeft = 1,
        MoveRight = 2,
        Jump = 4,
        Duck = 8,
        PrimaryUse = 16,
        SecondaryUse = 32
    }
}
