using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex05.DamkaGameLogic
{
    public class Player
    {
        private readonly ePlayerSide m_PlayerSide;
        private readonly string m_PlayerName;
        private int m_AmountOfRegularCheckers;
        private int m_AmountOfKingCheckers;

        public Player(ePlayerSide i_PlayerSide, string i_PlayerName, int i_BoardSize)
        {
            m_PlayerSide = i_PlayerSide;
            m_PlayerName = i_PlayerName;
            m_AmountOfRegularCheckers = ((i_BoardSize - 2) / 2) * (i_BoardSize / 2);
            m_AmountOfKingCheckers = 0;
        }

        public int AmountOfRegularCheckers
        {
            get { return m_AmountOfRegularCheckers; }
            set { m_AmountOfRegularCheckers = value; }
        }

        public int AmountOfKingCheckers
        {
            get { return m_AmountOfKingCheckers; }
            set { m_AmountOfKingCheckers = value; }
        }

        public void UpdatePlayerAmountOfCheckerAfterEat(bool i_IsCheckerKing)
        {
            if (i_IsCheckerKing == true)
            {
                m_AmountOfKingCheckers--;
            }
            else
            {
                m_AmountOfRegularCheckers--;
            }
        }

        public bool CheckOutOfCheckers()
        {
            bool isOutOfCheckers = AmountOfKingCheckers + AmountOfRegularCheckers == 0;
            return isOutOfCheckers;
        }
    }
}
