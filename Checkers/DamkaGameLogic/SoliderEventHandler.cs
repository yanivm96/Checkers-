using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex05.DamkaGameLogic
{
    public delegate void SoldierEventHandler(object sender, SoldierEventArgs e);

    public class SoldierEventArgs : EventArgs
    {
        private Square m_SquarePosition;

        public SoldierEventArgs(Square SquarePosition)
        {
            m_SquarePosition = SquarePosition;
        }

        public Square Position
        {
            get { return m_SquarePosition; }
        }
    }
}
