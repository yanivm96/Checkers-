using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex05.DamkaGameLogic
{
    public class GameBoard
    {
        private readonly Square[,] m_Board;
        private readonly int m_BoardSize;

        public event SoldierEventHandler KingEnterSquare;

        public event SoldierEventHandler KingLeftSquare;

        public GameBoard(int i_BoardSize)
        {
            m_Board = new Square[i_BoardSize, i_BoardSize];
            m_BoardSize = i_BoardSize;

            for (int i = 0; i < i_BoardSize; i++)
            {
                for (int j = 0; j < i_BoardSize; j++)
                {
                    m_Board[i, j] = new Square(i, j, (int)ePlayerSide.Empty);
                }
            }

            boardGameStartArranging();
        }

        public int BoardSize
        {
            get { return m_BoardSize; }
        }

        public ePlayerSide this[int i_RowIndex, int i_ColIndex]
        {
            get { return m_Board[i_RowIndex, i_ColIndex].PlayerThatOwnThisSquare; }
            set { m_Board[i_RowIndex, i_ColIndex].PlayerThatOwnThisSquare = value; }
        }

        private void boardGameStartArranging()
        {
            int upSidePlayerStartRowInBoardInit = 0;
            int upSidePlayerEndRowInBoardInit = (m_BoardSize - 2) / 2;
            int downSidePlayerStartRowInBoardInit = m_BoardSize - upSidePlayerEndRowInBoardInit;
            int downSidePlayerEndRowInBoardInit = m_BoardSize;

            boardInitArranging(upSidePlayerStartRowInBoardInit, upSidePlayerEndRowInBoardInit, ePlayerSide.UpSidePlayer);
            boardInitArranging(downSidePlayerStartRowInBoardInit, downSidePlayerEndRowInBoardInit, ePlayerSide.DownSidePlayer);
        }

        private void boardInitArranging(int i_StartRow, int i_LastRow, ePlayerSide i_PlayerSide)
        {
            for (int i = i_StartRow; i < i_LastRow; i++)
            {
                for (int j = 0; j < m_BoardSize; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        m_Board[i, j].PlayerThatOwnThisSquare = i_PlayerSide;
                    }
                    else
                    {
                        m_Board[i, j].PlayerThatOwnThisSquare = ePlayerSide.Empty;
                    }
                }
            }
        }

        public bool CheckIfKingInSquare(int i_CurrentCol, int i_CurrentRow, ePlayerSide i_PlayerNumber)
        {
            bool isThereKingInSquare = m_Board[i_CurrentRow, i_CurrentCol].KingInSquare;
            bool isPlayerOwnThisSquare = m_Board[i_CurrentRow, i_CurrentCol].PlayerThatOwnThisSquare == i_PlayerNumber;

            return isThereKingInSquare && isPlayerOwnThisSquare;
        }

        public void RemoveKingFromSquare(int i_CurrentCol, int i_CurrentRow)
        {
            m_Board[i_CurrentRow, i_CurrentCol].KingInSquare = false;
            OnKingLeft(new Square(i_CurrentRow, i_CurrentCol, ePlayerSide.Empty));
        }

        public void SetKingInSquare(int i_CurrentCol, int i_CurrentRow)
        {
            m_Board[i_CurrentRow, i_CurrentCol].KingInSquare = true;
            OnKingEnter(new Square(i_CurrentRow, i_CurrentCol, ePlayerSide.Empty));
        }

        protected virtual void OnKingLeft(Square i_PositionSquare)
        {
            SoldierEventArgs e = new SoldierEventArgs(i_PositionSquare);

            if (KingLeftSquare != null)
            {
                KingLeftSquare.Invoke(this, e);
            }
        }

        protected virtual void OnKingEnter(Square i_PositionSquare)
        {
            SoldierEventArgs e = new SoldierEventArgs(i_PositionSquare);

            if (KingEnterSquare != null)
            {
                KingEnterSquare.Invoke(this, e);
            }
        }
    }
}
