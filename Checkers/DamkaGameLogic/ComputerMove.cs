using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex05.DamkaGameLogic
{
    public class ComputerMove
    {
        private static bool s_LastMoveWasEat = false;
        private static int s_LastMoveRowIndex = 0;
        private static int s_LastMoveColIndex = 0;
        private Square m_Destination;
        private int m_MoveFromCol;
        private int m_MoveFromRow;

        public ComputerMove(int i_MoveFromCol, int i_MoveFromRow, Square i_Destination)
        {
            m_Destination = i_Destination;
            m_MoveFromCol = i_MoveFromCol;
            m_MoveFromRow = i_MoveFromRow;
        }

        public Square DestinationSquare
        {
            get { return m_Destination; }
            set { m_Destination = value; }
        }

        public int MoveFromCol
        {
            get { return m_MoveFromCol; }
            set { m_MoveFromCol = value; }
        }

        public int MoveFromRow
        {
            get { return m_MoveFromRow; }
            set { m_MoveFromRow = value; }
        }

        public static bool PickComputerNextMove(
            List<ComputerMove> i_ListOfAllComputerValidMoves,
            GameBoard i_GameBoard,
            ref int io_MoveFromCol,
            ref int io_MoveFromRow,
            ref int io_MoveToCol,
            ref int io_MoveToRow)
        {
            const int k_DepthOfRecursion = 4;
            int valueOfBestMove = 0;
            bool isValidComputerMoveExist = true;
            ComputerMove bestMoveToMake = null;

            if (i_ListOfAllComputerValidMoves.Count == 0)
            {
                isValidComputerMoveExist = false;
            }
            else
            {
                i_ListOfAllComputerValidMoves = checkIfSkippingMoveExistAfterEatingMove(i_ListOfAllComputerValidMoves);
                if (i_ListOfAllComputerValidMoves.Count > 0)
                {
                    bestMoveToMake = i_ListOfAllComputerValidMoves[0];
                }
                else
                {
                    valueOfBestMove = pickComputerMoveByAI(
                        ref bestMoveToMake, i_GameBoard, ePlayerSide.UpSidePlayer, k_DepthOfRecursion, k_DepthOfRecursion);
                }

                updateMoveComputerShouldMake(
                    i_GameBoard, bestMoveToMake, ref io_MoveFromCol, ref io_MoveFromRow, ref io_MoveToCol, ref io_MoveToRow);
            }

            return isValidComputerMoveExist;
        }

        private static void updateMoveComputerShouldMake(
            GameBoard i_GameBoard,
            ComputerMove i_BestMove,
            ref int io_MoveFromCol,
            ref int io_MoveFromRow,
            ref int io_MoveToCol,
            ref int io_MoveToRow)
        {
            io_MoveFromCol = i_BestMove.MoveFromCol;
            io_MoveFromRow = i_BestMove.MoveFromRow;
            io_MoveToCol = i_BestMove.DestinationSquare.ColIndex;
            io_MoveToRow = i_BestMove.DestinationSquare.RowIndex;
            if (checkIfMoveIsEatingMove(io_MoveFromRow, io_MoveToRow))
            {
                s_LastMoveWasEat = true;
            }

            s_LastMoveRowIndex = io_MoveToRow;
            s_LastMoveColIndex = io_MoveToCol;
        }

        private static bool checkIfMoveIsEatingMove(int i_MoveFromRow, int i_MoveToRow)
        {
            bool lastMoveWasEat = false;

            s_LastMoveWasEat = false;
            if (Math.Abs(i_MoveFromRow - i_MoveToRow) == 2)
            {
                lastMoveWasEat = true;
            }

            return lastMoveWasEat;
        }

        private static List<ComputerMove> checkIfSkippingMoveExistAfterEatingMove(List<ComputerMove> i_ListOfAllComputerValidMoves)
        {
            List<ComputerMove> newListOfValidMoves = new List<ComputerMove>();
            bool isSkippingMoveExist = false;

            if (s_LastMoveWasEat == true)
            {
                foreach (ComputerMove move in i_ListOfAllComputerValidMoves)
                {
                    if (Math.Abs(move.MoveFromRow - move.DestinationSquare.RowIndex) == 2 && move.MoveFromRow == s_LastMoveRowIndex
                        && move.MoveFromCol == s_LastMoveColIndex)
                    {
                        isSkippingMoveExist = true;
                    }
                }
            }

            if (isSkippingMoveExist == true)
            {
                foreach (ComputerMove move in i_ListOfAllComputerValidMoves)
                {
                    if ((move.MoveFromRow == s_LastMoveRowIndex && move.MoveFromCol == s_LastMoveColIndex) == true)
                    {
                        newListOfValidMoves.Add(move);
                    }
                }
            }

            return newListOfValidMoves;
        }

        private static GameBoard makeCopyOfBoard(GameBoard i_GameBoard)
        {
            GameBoard newCopyBoard = new GameBoard(i_GameBoard.BoardSize);

            for (int i = 0; i < i_GameBoard.BoardSize; i++)
            {
                for (int j = 0; j < i_GameBoard.BoardSize; j++)
                {
                    newCopyBoard[i, j] = i_GameBoard[i, j];
                }
            }

            return newCopyBoard;
        }

        private static GameBoard creatNewBoardAndSimulateMove(GameBoard i_GameBoard, ComputerMove i_MoveToMake, ePlayerSide i_PlayerTurn)
        {
            GameBoard newBoard = makeCopyOfBoard(i_GameBoard);
            int moveFromCol = i_MoveToMake.MoveFromCol;
            int moveFromRow = i_MoveToMake.MoveFromRow;
            int moveToCol = i_MoveToMake.m_Destination.ColIndex;
            int moveToRow = i_MoveToMake.m_Destination.RowIndex;

            if (i_GameBoard.CheckIfKingInSquare(moveFromCol, moveFromRow, i_PlayerTurn))
            {
                newBoard.RemoveKingFromSquare(moveFromCol, moveFromRow);
                newBoard.SetKingInSquare(moveToCol, moveToRow);
            }
            else
            {
                checkAndUpdateIfCheckerBecomeKing(newBoard, moveToRow, moveToCol, i_PlayerTurn);
            }

            if (Math.Abs(moveFromRow - moveToRow) == 2)
            {
                int RowIndexOfEatenChecker = moveToRow + ((moveFromRow - moveToRow) / 2);
                int ColIndexOfEatenChecker = moveToCol + ((moveFromCol - moveToCol) / 2);
                newBoard[RowIndexOfEatenChecker, ColIndexOfEatenChecker] = ePlayerSide.Empty;
                newBoard.RemoveKingFromSquare(ColIndexOfEatenChecker, RowIndexOfEatenChecker);
            }

            newBoard[moveFromRow, moveFromCol] = ePlayerSide.Empty;
            newBoard[moveToRow, moveToCol] = i_PlayerTurn;

            return newBoard;
        }

        private static void checkAndUpdateIfCheckerBecomeKing(GameBoard i_GameBoard, int i_TargetRow, int i_TargetCol, ePlayerSide i_PlayerTurn)
        {
            if (i_TargetRow == i_GameBoard.BoardSize - 1 && i_PlayerTurn == ePlayerSide.UpSidePlayer)
            {
                i_GameBoard.SetKingInSquare(i_TargetCol, i_TargetRow);
            }

            if (i_TargetRow == 0 && i_PlayerTurn == ePlayerSide.DownSidePlayer)
            {
                i_GameBoard.SetKingInSquare(i_TargetCol, i_TargetRow);
            }
        }

        private static int evaluateBoard(GameBoard i_GameBoard)
        {
            int difference = 0;

            for (int i = 0; i < i_GameBoard.BoardSize; i++)
            {
                for (int j = 0; j < i_GameBoard.BoardSize; j++)
                {
                    if (i_GameBoard.CheckIfKingInSquare(j, i, ePlayerSide.UpSidePlayer) == true)
                    {
                        difference += (int)eBoardEvaluateScore.King;
                    }
                    else if (i_GameBoard.CheckIfKingInSquare(j, i, ePlayerSide.DownSidePlayer) == true)
                    {
                        difference -= (int)eBoardEvaluateScore.King;
                    }
                    else
                    {
                        if (i_GameBoard[i, j] == ePlayerSide.UpSidePlayer)
                        {
                            difference += (int)eBoardEvaluateScore.RegularChecker;
                        }
                        else if (i_GameBoard[i, j] == ePlayerSide.DownSidePlayer)
                        {
                            difference -= (int)eBoardEvaluateScore.RegularChecker;
                        }
                    }
                }
            }

            return difference;
        }

        private static bool checkValidEatMoveFromGivenPosition(List<ComputerMove> i_AllValidMoves, int i_CurrentRow, int i_CurrentCol)
        {
            bool validEatMoveExist = false;

            foreach (ComputerMove move in i_AllValidMoves)
            {
                if (Math.Abs(move.MoveFromRow - move.DestinationSquare.RowIndex) == 2)
                {
                    if (i_CurrentRow == move.MoveFromRow && i_CurrentCol == move.MoveFromCol)
                    {
                        validEatMoveExist = true;
                    }
                }
            }

            return validEatMoveExist;
        }

        private static void checkIfSkippingMoveExist(ComputerMove i_CurrentMove, GameBoard i_GameBoard, ref ePlayerSide io_PlayerTurn)
        {
            if (checkIfMoveIsEatingMove(i_CurrentMove.MoveFromRow, i_CurrentMove.m_Destination.RowIndex) == true)
            {
                DamkaGame damkaGame = new DamkaGame(i_GameBoard);
                List<ComputerMove> EatingMoves = damkaGame.GetAllValidMovesByPlayerSide(io_PlayerTurn);
                if (checkValidEatMoveFromGivenPosition(
                    EatingMoves, i_CurrentMove.DestinationSquare.RowIndex, i_CurrentMove.DestinationSquare.ColIndex) == true)
                {
                    io_PlayerTurn = DamkaGame.GetOppositePlayer(io_PlayerTurn);
                }
            }
        }

        private static void updateMoveToMake(
            ref ComputerMove io_MoveToMake,
            ComputerMove i_LastMove,
            ePlayerSide i_PlayerTurn,
            ref int io_UpPlayerEvaluation,
            ref int io_DownPlayerEvaluation,
            int i_CurrentEvaluation,
            int i_CurrentRecurstionDepth,
            int i_OriginalRecurstionDepth)
        {
            if (i_PlayerTurn == ePlayerSide.UpSidePlayer)
            {
                if (io_UpPlayerEvaluation < i_CurrentEvaluation)
                {
                    io_UpPlayerEvaluation = i_CurrentEvaluation;
                    if (i_CurrentRecurstionDepth == i_OriginalRecurstionDepth)
                    {
                        io_MoveToMake = i_LastMove;
                    }
                }
            }
            else
            {
                io_DownPlayerEvaluation = Math.Max(io_DownPlayerEvaluation, i_CurrentEvaluation);
            }
        }

        private static int pickComputerMoveByAI(
            ref ComputerMove io_MoveToMake,
            GameBoard i_GameBoard,
            ePlayerSide i_PlayerTurn,
            int i_CurrentRecurstionDepth,
            int i_OriginalRecurstionDepth)
        {
            int upPlayerevaluation = int.MinValue;
            int downPlayerevaluation = int.MinValue;
            int lastEvaluate = 0;
            int finalEvaluation = 0;
            DamkaGame damkaGame = new DamkaGame(i_GameBoard);
            List<ComputerMove> allValidMoves = damkaGame.GetAllValidMovesByPlayerSide(i_PlayerTurn);
            ePlayerSide nextPlayerTurn = DamkaGame.GetOppositePlayer(i_PlayerTurn);

            if (i_CurrentRecurstionDepth == 0 || allValidMoves.Count == 0)
            {
                finalEvaluation = evaluateBoard(i_GameBoard);
            }
            else
            {
                foreach (ComputerMove currentMove in allValidMoves)
                {
                    GameBoard BoardAfterMoveSimulation = creatNewBoardAndSimulateMove(i_GameBoard, currentMove, i_PlayerTurn);
                    checkIfSkippingMoveExist(currentMove, BoardAfterMoveSimulation, ref nextPlayerTurn);
                    lastEvaluate = pickComputerMoveByAI(
                        ref io_MoveToMake, BoardAfterMoveSimulation, nextPlayerTurn, i_CurrentRecurstionDepth - 1, i_OriginalRecurstionDepth);
                    updateMoveToMake(
                        ref io_MoveToMake,
                        currentMove,
                        i_PlayerTurn,
                        ref upPlayerevaluation,
                        ref downPlayerevaluation,
                        lastEvaluate,
                        i_CurrentRecurstionDepth,
                        i_OriginalRecurstionDepth);
                }

                if (i_PlayerTurn == ePlayerSide.UpSidePlayer)
                {
                    finalEvaluation = upPlayerevaluation;
                }
                else
                {
                    finalEvaluation = downPlayerevaluation;
                }
            }

            return finalEvaluation;
        }
    }
}
