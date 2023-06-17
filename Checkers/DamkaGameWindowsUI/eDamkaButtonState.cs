using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex05.DamkaGameWindowsUI
{
    public enum eDamkaButtonState
    {
        RegularRed = 0,
        RegularBlack = 1,
        RedKing = 2,
        BlackKing = 3,
        Empty = 4,
        StartOfMarkedValues = 5,
        RegularRedMarked = 5,
        RegularBlackMarked = 6,
        MarkedRedKing = 7,
        MarkedBlackKing = 8,
        OptionalForMove = 9,
        EndOfMarkedValues = 9,
        NotUsed = 10,
    }
}
