using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex05.DamkaGameLogic
{
    public class DamkaGame
    {  
        private GameBoard m_GameBoard;
        private Player m_UpSidePlayer;
        private Player m_DownSidePlayer;
        private bool m_LastMoveWasEat;
        private int m_RowIndexOfCheckerThatWasEating;
        private int m_ColIndexOfCheckerThatWasEating;

        public event SoldierEventHandler SoldierWasEat;

        public DamkaGame(GameBoard i_gameBoard)
        {
            m_GameBoard = i_gameBoard;
            m_UpSidePlayer = null;
            m_DownSidePlayer = null;
            m_LastMoveWasEat = false;
            m_RowIndexOfCheckerThatWasEating = 0;
            m_ColIndexOfCheckerThatWasEating = 0;
        }

        public DamkaGame(int i_BoardSize, string i_DownSidePlayerName, ePlayerSide i_DownSidePlayer, string i_UpSidePlayerName, ePlayerSide i_UpSidePlayer)
        {
            m_GameBoard = new GameBoard(i_BoardSize);
            m_UpSidePlayer = new Player(i_UpSidePlayer, i_UpSidePlayerName, i_BoardSize);
            m_DownSidePlayer = new Player(i_DownSidePlayer, i_DownSidePlayerName, i_BoardSize);
            m_LastMoveWasEat = false;
            m_RowIndexOfCheckerThatWasEating = 0;
            m_ColIndexOfCheckerThatWasEating = 0;
        }

        public GameBoard Board
        {
            get { return m_GameBoard; }
            set { m_GameBoard = value; }
        }

        protected virtual void OnSoldierEat(Square i_EatenSquare)
        {
            SoldierEventArgs e = new SoldierEventArgs(i_EatenSquare);

            if (SoldierWasEat != null)
            {
                SoldierWasEat.Invoke(this, e);
            }
        }

        private void makeCheckerMove(int i_MoveFromCol, int i_MoveFromRow, int i_MoveToCol, int i_MoveToRow, ePlayerSide i_PlayerTurn)
        {
            Square from = new Square(i_MoveFromRow, i_MoveFromCol, ePlayerSide.Empty);
            Square to = new Square(i_MoveToRow, i_MoveToCol, i_PlayerTurn);

            if (m_GameBoard.CheckIfKingInSquare(i_MoveFromCol, i_MoveFromRow, i_PlayerTurn) == false)
            {
                checkAndUpdateIfCheckerBecomeKing(i_MoveToCol, i_MoveToRow, i_PlayerTurn);
            }

            if (m_GameBoard.CheckIfKingInSquare(i_MoveFromCol, i_MoveFromRow, i_PlayerTurn))
            {
                m_GameBoard.SetKingInSquare(i_MoveToCol, i_MoveToRow);
                m_GameBoard.RemoveKingFromSquare(i_MoveFromCol, i_MoveFromRow);
            }

            m_GameBoard[i_MoveFromRow, i_MoveFromCol] = ePlayerSide.Empty;
            m_GameBoard[i_MoveToRow, i_MoveToCol] = i_PlayerTurn;
        }

        public List<Square> GetListOfValidMoves(int i_CurrentCol, int i_CurrentRow, ePlayerSide i_PlayerTurn, bool i_LastMoveWasAnEat)
        {
            bool isCheckerInSquareKing = m_GameBoard.CheckIfKingInSquare(i_CurrentCol, i_CurrentRow, i_PlayerTurn);
            List<Square> listOfValidMoves = new List<Square>();

            listOfValidMoves.Add(new Square(i_CurrentRow - 1, i_CurrentCol - 1, ePlayerSide.Empty));
            listOfValidMoves.Add(new Square(i_CurrentRow - 1, i_CurrentCol + 1, ePlayerSide.Empty));
            listOfValidMoves.Add(new Square(i_CurrentRow + 1, i_CurrentCol + 1, ePlayerSide.Empty));
            listOfValidMoves.Add(new Square(i_CurrentRow + 1, i_CurrentCol - 1, ePlayerSide.Empty));
            removeInvalidMovesFromListAndAddPosibleEatingMoves(
                ref listOfValidMoves, i_PlayerTurn, isCheckerInSquareKing, i_CurrentCol, i_CurrentRow, i_LastMoveWasAnEat);

            return listOfValidMoves;
        }

        private void removeInvalidMovesFromListAndAddPosibleEatingMoves(
            ref List<Square> io_ListOfValidMoves,
            ePlayerSide i_PlayerTurn,
            bool i_IsCheckerInSquareKing,
            int i_CurrentCol,
            int i_CurrentRow,
            bool i_LastMoveWasAnEat)
        {
            bool removeSquareFromList = false;
            bool noPosibleEating = true;
            int listSize = io_ListOfValidMoves.Count;
            int index = 0;
            Square targetPosition = null;

            for (int i = 0; i < listSize; i++, index++)
            {
                targetPosition = io_ListOfValidMoves[index];
                checkBoandsStartAndTargetPosition(i_CurrentCol, i_CurrentRow, targetPosition, ref removeSquareFromList);
                checkIfMoveIsInRightDirection(i_PlayerTurn, i_CurrentRow, targetPosition, i_IsCheckerInSquareKing, ref removeSquareFromList);
                checkIfMoveIsSkippingMoveAfterEatMove(targetPosition, i_LastMoveWasAnEat, ref removeSquareFromList);
                checkAndAddEatingMove(
                    targetPosition, i_CurrentCol, i_CurrentRow, i_PlayerTurn, ref removeSquareFromList, ref noPosibleEating, ref io_ListOfValidMoves);
                if (removeSquareFromList == true)
                {
                    io_ListOfValidMoves.Remove(targetPosition);
                    removeSquareFromList = false;
                    index--;
                }
            }

            removeMovesWithNoEatsWhenThereIsMoveWithPosibleEat(ref io_ListOfValidMoves, i_CurrentCol, noPosibleEating);
        }

        private void checkBoandsStartAndTargetPosition(
            int i_CurrentCol,
            int i_CurrentRow,
            Square i_TargetPosition,
            ref bool io_RemoveMoveFromList)
        {
            if (checkIfPositionOutOfBoardBoands(i_CurrentCol, i_CurrentRow) == true)
            {
                io_RemoveMoveFromList = true;
            }
            else if (checkIfPositionOutOfBoardBoands(i_TargetPosition.ColIndex, i_TargetPosition.RowIndex) == true)
            {
                io_RemoveMoveFromList = true;
            }
        }

        private void checkIfMoveIsInRightDirection(
            ePlayerSide i_PlayerTurn,
            int i_CurrentRow,
            Square i_TargetPosition,
            bool i_IsCheckerInSquareKing,
            ref bool io_RemoveMoveFromList)
        {
            if (io_RemoveMoveFromList == false)
            {
                if (i_PlayerTurn == ePlayerSide.UpSidePlayer && (i_CurrentRow - i_TargetPosition.RowIndex > 0) && i_IsCheckerInSquareKing == false)
                {
                    io_RemoveMoveFromList = true;
                }
                else if (i_PlayerTurn == ePlayerSide.DownSidePlayer && (i_CurrentRow - i_TargetPosition.RowIndex < 0) && i_IsCheckerInSquareKing == false)
                {
                    io_RemoveMoveFromList = true;
                }
            }
        }

        private void checkIfMoveIsSkippingMoveAfterEatMove(Square i_TargetPosition, bool i_LastMoveWasAnEat, ref bool io_RemoveMoveFromList)
        {
            if (io_RemoveMoveFromList == false)
            {
                if (m_GameBoard[i_TargetPosition.RowIndex, i_TargetPosition.ColIndex] == ePlayerSide.Empty && i_LastMoveWasAnEat == true)
                {
                    io_RemoveMoveFromList = true;
                }
            }
        }

        private void checkAndAddEatingMove(
            Square i_TargetPosition,
            int i_CurrentCol,
            int i_CurrentRow,
            ePlayerSide i_PlayerTurn,
            ref bool io_RemoveMoveFromList,
            ref bool io_NoPosibleEating,
            ref List<Square> io_ListOfValidMoves)
        {
            int rowDirection = 0;
            int colDirection = 0;

            if (io_RemoveMoveFromList == false)
            {
                if (m_GameBoard[i_TargetPosition.RowIndex, i_TargetPosition.ColIndex] != ePlayerSide.Empty)
                {
                    rowDirection = i_CurrentRow - i_TargetPosition.RowIndex;
                    colDirection = i_CurrentCol - i_TargetPosition.ColIndex;
                    io_RemoveMoveFromList = true;

                    if (i_PlayerTurn != m_GameBoard[i_TargetPosition.RowIndex, i_TargetPosition.ColIndex])
                    {
                        if (checkIfPositionOutOfBoardBoands(i_TargetPosition.ColIndex - colDirection, i_TargetPosition.RowIndex - rowDirection) == false)
                        {
                            if (m_GameBoard[i_TargetPosition.RowIndex - rowDirection, i_TargetPosition.ColIndex - colDirection] == ePlayerSide.Empty)
                            {
                                io_ListOfValidMoves.Add(
                                    new Square(i_TargetPosition.RowIndex - rowDirection, i_TargetPosition.ColIndex - colDirection, ePlayerSide.Empty));
                                io_NoPosibleEating = false;
                            }
                        }
                    }
                }
            }
        }

        private bool checkIfPositionOutOfBoardBoands(int i_ColIndex, int i_RowIndex)
        {
            bool outOfBoardBounds = true;

            if ((i_RowIndex < 0 || i_ColIndex < 0 || i_RowIndex > m_GameBoard.BoardSize - 1 || i_ColIndex > m_GameBoard.BoardSize - 1) == false)
            {
                outOfBoardBounds = false;
            }

            return outOfBoardBounds;
        }

        private void removeMovesWithNoEatsWhenThereIsMoveWithPosibleEat(
            ref List<Square> io_ListOfValidMoves,
            int i_CurrentCol,
            bool i_NoPosibleEating)
        {
            int listSize = io_ListOfValidMoves.Count;
            int index = 0;
            Square position = null;

            if (i_NoPosibleEating == false)
            {
                for (int i = 0; i < listSize; i++, index++)
                {
                    position = io_ListOfValidMoves[index];
                    if (Math.Abs(i_CurrentCol - position.ColIndex) < 2)
                    {
                        io_ListOfValidMoves.Remove(position);
                        index--;
                    }
                }
            }
        }

        private bool checkValidationOfUserMove(
            int i_MoveFromCol,
            int i_MoveFromRow,
            int i_MoveToCol,
            int i_MoveToRow,
            ePlayerSide i_PlayerTurn,
            ref bool io_PossibleEatFromTargetPosition)
        {
            bool isMoveInListOfValidMoves = false;
            bool PossibleEatFromCurrentPosition = false;

            if (m_LastMoveWasEat == true && (m_RowIndexOfCheckerThatWasEating != i_MoveFromRow || m_ColIndexOfCheckerThatWasEating != i_MoveFromCol))
            {
                isMoveInListOfValidMoves = false;
            }
            else if (findAllPositionsWithPossibleEatsAndCompareToUserInput(i_MoveFromCol, i_MoveFromRow, i_PlayerTurn, ref PossibleEatFromCurrentPosition) == true)
            {
                checkIfUserMoveInListOfValidMoves(i_MoveFromCol, i_MoveFromRow, i_PlayerTurn, i_MoveToCol, i_MoveToRow, ref isMoveInListOfValidMoves);
            }

            io_PossibleEatFromTargetPosition = m_LastMoveWasEat;

            return isMoveInListOfValidMoves;
        }

        private void checkIfUserMoveInListOfValidMoves(
            int i_MoveFromCol,
            int i_MoveFromRow,
            ePlayerSide i_PlayerTurn,
            int i_MoveToCol,
            int i_MoveToRow,
            ref bool io_IsMoveInListOfValidMoves)
        {
            if (m_GameBoard[i_MoveFromRow, i_MoveFromCol] == i_PlayerTurn)
            {
                const bool v_lastMoveWasAnEat = true;
                List<Square> ListOfAllValidMoves = GetListOfValidMoves(i_MoveFromCol, i_MoveFromRow, i_PlayerTurn, !v_lastMoveWasAnEat);

                foreach (Square position in ListOfAllValidMoves)
                {
                    if (i_MoveToCol == position.ColIndex && i_MoveToRow == position.RowIndex)
                    {
                        io_IsMoveInListOfValidMoves = true;
                        makeCheckerMove(i_MoveFromCol, i_MoveFromRow, i_MoveToCol, i_MoveToRow, i_PlayerTurn);
                        checkIfMoveWasEatingMoveAndUpdateAmountOfCheckers(i_MoveFromCol, i_MoveFromRow, i_MoveToCol, i_MoveToRow, i_PlayerTurn);
                    }
                }
            }
        }

        private void checkIfMoveWasEatingMoveAndUpdateAmountOfCheckers(
            int i_MoveFromCol,
            int i_MoveFromRow, 
            int i_MoveToCol,
            int i_MoveToRow,
            ePlayerSide i_PlayerTurn)
        {
            int rowIndexOfEatenChecker = 0;
            int colIndexOfEatenChecker = 0;
            bool isCheckerWasEatenIsKing = false;
            bool isTherePossibleEatFromCurrentPosition = false;

            if (Math.Abs(i_MoveFromCol - i_MoveToCol) == 2)
            {
                calcPositionOfEatenChecker(i_MoveFromCol, i_MoveFromRow, i_MoveToCol, i_MoveToRow, ref colIndexOfEatenChecker, ref rowIndexOfEatenChecker);
                isCheckerWasEatenIsKing = m_GameBoard.CheckIfKingInSquare(colIndexOfEatenChecker, rowIndexOfEatenChecker, GetOppositePlayer(i_PlayerTurn));
                removeCheckerFromSquareAfterEat(colIndexOfEatenChecker, rowIndexOfEatenChecker);
                updateKingPositionAfterMove(i_MoveFromCol, i_MoveFromRow, i_PlayerTurn, i_MoveToCol, i_MoveToRow);
                reduceAmountOfCheckersAfterEat(i_PlayerTurn, isCheckerWasEatenIsKing);
                checkForSkippingMoveAfterEat(i_MoveFromCol, i_MoveFromRow, i_PlayerTurn, i_MoveToCol, i_MoveToRow, ref isTherePossibleEatFromCurrentPosition);
            }
        }

        private void checkForSkippingMoveAfterEat(
            int i_MoveFromCol,
            int i_MoveFromRow,
            ePlayerSide i_PlayerTurn,
            int i_MoveToCol,
            int i_MoveToRow,
            ref bool io_IsTherePossibleEatFromCurrentPosition)
        {
            if (findAllPositionsWithPossibleEatsAndCompareToUserInput(i_MoveToCol, i_MoveToRow, i_PlayerTurn, ref io_IsTherePossibleEatFromCurrentPosition))
            {
                if (io_IsTherePossibleEatFromCurrentPosition == true)
                {
                    m_LastMoveWasEat = true;
                    m_RowIndexOfCheckerThatWasEating = i_MoveToRow;
                    m_ColIndexOfCheckerThatWasEating = i_MoveToCol;
                }
                else
                {
                    m_LastMoveWasEat = false;
                }
            }
            else
            {
                m_LastMoveWasEat = false;
            }
        }

        private void calcPositionOfEatenChecker(
            int i_MoveFromCol,
            int i_MoveFromRow,
            int i_MoveToCol,
            int i_MoveToRow,
            ref int io_ColIndexOfEatenChecker,
            ref int io_RowIndexOfEatenChecker)
        {
            io_RowIndexOfEatenChecker = i_MoveToRow + ((i_MoveFromRow - i_MoveToRow) / 2);
            io_ColIndexOfEatenChecker = i_MoveToCol + ((i_MoveFromCol - i_MoveToCol) / 2);
        }

        private void removeCheckerFromSquareAfterEat(int i_ColIndexOfEatenChecker, int i_RowIndexOfEatenChecker)
        {
            m_GameBoard[i_RowIndexOfEatenChecker, i_ColIndexOfEatenChecker] = ePlayerSide.Empty;
            m_GameBoard.RemoveKingFromSquare(i_ColIndexOfEatenChecker, i_RowIndexOfEatenChecker);
            OnSoldierEat(new Square(i_RowIndexOfEatenChecker, i_ColIndexOfEatenChecker, ePlayerSide.Empty));
        }

        private void updateKingPositionAfterMove(int i_MoveFromCol, int i_MoveFromRow, ePlayerSide i_PlayerTurn, int i_MoveToCol, int i_MoveToRow)
        {
            if (m_GameBoard.CheckIfKingInSquare(i_MoveFromCol, i_MoveFromRow, i_PlayerTurn))
            {
                m_GameBoard.RemoveKingFromSquare(i_MoveFromCol, i_MoveFromRow);
                m_GameBoard.SetKingInSquare(i_MoveToCol, i_MoveToRow);
            }
        }

        private void reduceAmountOfCheckersAfterEat(ePlayerSide i_PlayerTurn, bool i_IsCheckerWasEatenIsKing)
        {
            if (i_PlayerTurn == ePlayerSide.UpSidePlayer)
            {
                m_DownSidePlayer.UpdatePlayerAmountOfCheckerAfterEat(i_IsCheckerWasEatenIsKing);
            }
            else
            {
                m_UpSidePlayer.UpdatePlayerAmountOfCheckerAfterEat(i_IsCheckerWasEatenIsKing);
            }
        }

        public static ePlayerSide GetOppositePlayer(ePlayerSide i_PlayerTurn)
        {
            ePlayerSide oppositePlayer = ePlayerSide.DownSidePlayer;

            if (i_PlayerTurn == ePlayerSide.DownSidePlayer)
            {
                oppositePlayer = ePlayerSide.UpSidePlayer;
            }

            return oppositePlayer;
        }

        private bool checkAvailableMoveForEachPlayers(ePlayerSide i_PlayerToCheck)
        {
            bool isValidMoveExist = false;
            const bool v_lastMoveWasAnEat = true;
            List<Square> ListOfAllValidMoves = null;

            for (int i = 0; i < m_GameBoard.BoardSize; i++)
            {
                for (int j = 0; j < m_GameBoard.BoardSize; j++)
                {
                    if (m_GameBoard[i, j] == i_PlayerToCheck)
                    {
                        ListOfAllValidMoves = GetListOfValidMoves(j, i, i_PlayerToCheck, !v_lastMoveWasAnEat);
                        if (ListOfAllValidMoves.Count > 0)
                        {
                            isValidMoveExist = true;
                        }
                    }
                }
            }

            return isValidMoveExist;
        }

        private bool findAllPositionsWithPossibleEatsAndCompareToUserInput(
            int i_CurrentCol,
            int i_CurrentRow,
            ePlayerSide i_PlayerTurn,
            ref bool io_PossibleEatFromCurrentPosition)
        {
            const bool v_LastMoveWasAnEat = true;
            bool isUsersInputValid = false;
            List<Square> ListOfAllPositionsToEatFrom = new List<Square>();
            List<Square> ListOfAllEatingMoves = null;

            for (int i = 0; i < m_GameBoard.BoardSize; i++)
            {
                for (int j = 0; j < m_GameBoard.BoardSize; j++)
                {
                    if (m_GameBoard[i, j] == i_PlayerTurn)
                    {
                        ListOfAllEatingMoves = GetListOfValidMoves(j, i, i_PlayerTurn, v_LastMoveWasAnEat);
                        if (ListOfAllEatingMoves.Count > 0)
                        {
                            ListOfAllPositionsToEatFrom.Add(new Square(i, j, ePlayerSide.Empty));
                        }
                    }
                }
            }

            if (comparePositionsWithPossibleEatToUserInput(
                ListOfAllPositionsToEatFrom, i_CurrentCol, i_CurrentRow, ref io_PossibleEatFromCurrentPosition) == true)
            {
                isUsersInputValid = true;
            }

            return isUsersInputValid;
        }

        private bool comparePositionsWithPossibleEatToUserInput(
            List<Square> i_ListOfAllPositionsToEatFrom,
            int i_CurrentCol,
            int i_CurrentRow,
            ref bool io_PossibleEatFromCurrentPosition)
        {
            bool userInputValid = false;

            if (i_ListOfAllPositionsToEatFrom.Count == 0)
            {
                userInputValid = true;
            }
            else
            {
                foreach (Square position in i_ListOfAllPositionsToEatFrom)
                {
                    if (position.ColIndex == i_CurrentCol && position.RowIndex == i_CurrentRow)
                    {
                        userInputValid = true;
                        io_PossibleEatFromCurrentPosition = true;
                    }
                }
            }

            return userInputValid;
        }

        public bool RunPlayerTurn(
            int i_MoveFromCol,
            int i_MoveFromRow,
            int i_MoveToCol,
            int i_MoveToRow,
            ePlayerSide i_PlayerTurn,
            ref bool io_PossibleEatFromTargetPosition)
        {
            bool isValidMove = checkValidationOfUserMove(i_MoveFromCol, i_MoveFromRow, i_MoveToCol, i_MoveToRow, i_PlayerTurn, ref io_PossibleEatFromTargetPosition);

            return isValidMove;
        }

        private void checkAndUpdateIfCheckerBecomeKing(int i_CurrentCol, int i_CurrentRow, ePlayerSide i_PlayerTurn)
        {
            if (i_PlayerTurn == ePlayerSide.UpSidePlayer && i_CurrentRow == m_GameBoard.BoardSize - 1)
            {
                updateAmountAfterCheckerBecomeKing(m_UpSidePlayer, i_CurrentCol, i_CurrentRow);
            }
            else if (i_PlayerTurn == ePlayerSide.DownSidePlayer && i_CurrentRow == 0)
            {
                updateAmountAfterCheckerBecomeKing(m_DownSidePlayer, i_CurrentCol, i_CurrentRow);
            }
        }

        private void updateAmountAfterCheckerBecomeKing(Player i_Player, int i_CurrentCol, int i_CurrentRow)
        {
            m_GameBoard.SetKingInSquare(i_CurrentCol, i_CurrentRow);
            i_Player.AmountOfRegularCheckers--;
            i_Player.AmountOfKingCheckers++;
        }

        public ePlayerSide WinCheck(ePlayerSide i_PlayerTurn, ref bool io_IsGameFinishInTie)
        {
            ePlayerSide winnerPlayer = ePlayerSide.Empty;

            if (m_UpSidePlayer.CheckOutOfCheckers())
            {
                winnerPlayer = ePlayerSide.DownSidePlayer;
            }
            else if (m_DownSidePlayer.CheckOutOfCheckers())
            {
                winnerPlayer = ePlayerSide.UpSidePlayer;
            }
            else if (checkIfNoAvailableMoveForTwoPlayers(ref winnerPlayer, i_PlayerTurn) == true)
            {
                io_IsGameFinishInTie = true;
            }

            return winnerPlayer;
        }

        private bool checkIfNoAvailableMoveForTwoPlayers(ref ePlayerSide io_WinnerPlayer, ePlayerSide i_PlayerTurn)
        {
            bool isGameOverByTie = true;
            bool isUpPlayerHasMove = checkAvailableMoveForEachPlayers(ePlayerSide.UpSidePlayer);
            bool isDownPlayerHasMove = checkAvailableMoveForEachPlayers(ePlayerSide.DownSidePlayer);

            if (isUpPlayerHasMove == false && i_PlayerTurn == ePlayerSide.UpSidePlayer)
            {
                checkIfOppositePlayerHasMove(ref io_WinnerPlayer, i_PlayerTurn, isDownPlayerHasMove, ref isGameOverByTie);
            }
            else if (isDownPlayerHasMove == false && i_PlayerTurn == ePlayerSide.DownSidePlayer)
            {
                checkIfOppositePlayerHasMove(ref io_WinnerPlayer, i_PlayerTurn, isUpPlayerHasMove, ref isGameOverByTie);
            }
            else
            {
                io_WinnerPlayer = ePlayerSide.Empty;
                isGameOverByTie = false;
            }

            return isGameOverByTie;
        }

        private void checkIfOppositePlayerHasMove(
            ref ePlayerSide io_WinnerPlayer,
            ePlayerSide i_PlayerTurn,
            bool i_PlayerHasMove,
            ref bool io_IsGameOverByTie)
        {
            if (i_PlayerHasMove == true)
            {
                io_WinnerPlayer = GetOppositePlayer(i_PlayerTurn);
                io_IsGameOverByTie = false;
            }
        }

        public void CalcPlayersTotalPoints(ref int io_UpSidePlayerPoints, ref int io_DownSidePlayerPoints)
        {
            io_UpSidePlayerPoints += (m_UpSidePlayer.AmountOfKingCheckers * 4) + m_UpSidePlayer.AmountOfRegularCheckers;
            io_DownSidePlayerPoints += (m_DownSidePlayer.AmountOfKingCheckers * 4) + m_DownSidePlayer.AmountOfRegularCheckers;
        }

        private List<ComputerMove> searchForEatingAndRegularMoves(ePlayerSide i_PlayerToCheck, bool i_SearchForEatintMoves)
        {
            List<ComputerMove> ListOfAllComputerValidMoves = new List<ComputerMove>();
            List<Square> ListOfAllValidMoves = null;

            for (int i = 0; i < m_GameBoard.BoardSize; i++)
            {
                for (int j = 0; j < m_GameBoard.BoardSize; j++)
                {
                    if (m_GameBoard[i, j] == i_PlayerToCheck)
                    {
                        ListOfAllValidMoves = GetListOfValidMoves(j, i, i_PlayerToCheck, i_SearchForEatintMoves);
                        if (ListOfAllValidMoves.Count > 0)
                        {
                            addComputerMoves(ref ListOfAllComputerValidMoves, ListOfAllValidMoves, j, i);
                        }
                    }
                }
            }

            return ListOfAllComputerValidMoves;
        }

        public List<ComputerMove> GetAllValidMovesByPlayerSide(ePlayerSide i_PlayerToCheck)
        {
            const bool v_SearchForEatintMoves = true;
            List<ComputerMove> ListOfAllComputerValidMoves = searchForEatingAndRegularMoves(i_PlayerToCheck, v_SearchForEatintMoves);

            if (ListOfAllComputerValidMoves.Count == 0)
            {
                ListOfAllComputerValidMoves = searchForEatingAndRegularMoves(i_PlayerToCheck, !v_SearchForEatintMoves);
            }

            return ListOfAllComputerValidMoves;
        }

        private void addComputerMoves(
            ref List<ComputerMove> io_ListOfAllComputerValidMoves,
            List<Square> i_ListOfAllValidMovesFromCurrentPosition,
            int i_CurrentCol,
            int i_CurrentRow)
        {
            for (int i = 0; i < i_ListOfAllValidMovesFromCurrentPosition.Count; i++)
            {
                io_ListOfAllComputerValidMoves.Add(new ComputerMove(i_CurrentCol, i_CurrentRow, i_ListOfAllValidMovesFromCurrentPosition[i]));
            }
        }
    }
}
