using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
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
