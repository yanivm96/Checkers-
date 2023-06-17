using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex05.DamkaGameLogic
{
    public class Square
    {
        private int m_RowIndex;
        private int m_ColIndex;
        private bool m_KingInSquare;
        private ePlayerSide m_PlayerThatOwnThisSqure;

        public Square(int i_RowIndex, int i_ColIndex, ePlayerSide i_PlayerThatOwnThisSquare)
        {
            m_RowIndex = i_RowIndex;
            m_ColIndex = i_ColIndex;
            m_KingInSquare = false;
            m_PlayerThatOwnThisSqure = i_PlayerThatOwnThisSquare;
        }

        public int RowIndex
        {
            get { return m_RowIndex; }
            set { m_RowIndex = value; }
        }

        public int ColIndex
        {
            get { return m_ColIndex; }
            set { m_ColIndex = value; }
        }

        public ePlayerSide PlayerThatOwnThisSquare
        {
            get { return m_PlayerThatOwnThisSqure; }
            set { m_PlayerThatOwnThisSqure = value; }
        }

        public bool KingInSquare
        {
            get { return m_KingInSquare; }
            set { m_KingInSquare = value; }
        }
    }
}
