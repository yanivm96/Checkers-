using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ex05.DamkaGameLogic;

namespace Ex05.DamkaGameWindowsUI
{
    public partial class FormDamkaGame : Form
    {
        private readonly DamkaButton[,] r_Board;
        private readonly int r_BoardSize;
        private readonly string r_DownPlayerName;
        private readonly string r_UpPlayerName;
        private readonly bool r_OpponentIsComputer;
        private readonly bool r_ValidSettings;
        private readonly bool r_HelpMode;
        private Square m_LastClickedSquare;
        private Square m_LastEaterSoldier;
        private DamkaGame m_DamkaGame;
        private ePlayerSide m_PlayerTurn;
        private bool m_JumpEatExist = false;
        private bool m_MoveWasMade = false;
        private int m_DownPlayerPoints = 0;
        private int m_UpPlayerPoints = 0;

        public FormDamkaGame()
        {
            FormGameSettings gameSettingsForm = new FormGameSettings();
            gameSettingsForm.ShowDialog();
            gameSettingsForm.GetParameters(out r_BoardSize, out r_DownPlayerName, out r_UpPlayerName, out r_OpponentIsComputer, out r_ValidSettings, out r_HelpMode);
            if (r_ValidSettings)
            {
                InitializeComponent();
                m_LastClickedSquare = new Square(0, 0, ePlayerSide.Empty);
                m_LastEaterSoldier = new Square(0, 0, ePlayerSide.Empty);
                r_Board = new DamkaButton[r_BoardSize, r_BoardSize];
                addDamkaButtonsBoardToForm();
                setNewRound(r_BoardSize, r_DownPlayerName, r_UpPlayerName);
                setNamesAndScoresLable(r_DownPlayerName, r_UpPlayerName);
            }
        }

        public void runGame()
        {
            if(r_ValidSettings)
            {
                this.ShowDialog();
            }
        }

        private void setNamesAndScoresLable(string i_DownPlayerName, string i_UpPlayerName)
        {
            labelPlayer1Name.Text = i_DownPlayerName + ':';
            labelPlayer1Name.Left = (int)eFormPositions.Player1NamePosition;
            labelPlayer2Name.Text = i_UpPlayerName + ':';
            labelPlayer2Name.Left = this.Right - labelPlayer2Name.Width - (int)eFormPositions.SpaceBetweenNameLables;
            labelPlayer1Score.Location = new Point(labelPlayer1Name.Right + (int)eFormPositions.ScoreSpaceFromLable, labelPlayer1Name.Location.Y);
            labelPlayer2Score.Location = new Point(labelPlayer2Name.Right + (int)eFormPositions.ScoreSpaceFromLable, labelPlayer2Name.Location.Y);
        }

        private void setNewRound(int i_BoardSize, string i_DownPlayerName, string i_UpPlayerName)
        {
            m_DamkaGame = new DamkaGame(i_BoardSize, i_DownPlayerName, ePlayerSide.DownSidePlayer, i_UpPlayerName, ePlayerSide.UpSidePlayer);
            m_PlayerTurn = ePlayerSide.DownSidePlayer;
            resetDamkaButtonBoard();
            initGame(i_BoardSize);
            updateClickableDamkaButtons(m_PlayerTurn);
            markPlayerTurnName();
            m_DamkaGame.SoldierWasEat += this.damkaButton_Changed;
            m_DamkaGame.Board.KingEnterSquare += this.damkaButton_SetKing;
            m_DamkaGame.Board.KingLeftSquare += this.damkaButton_RemoveKing;
        }

        private void resetDamkaButtonBoard()
        {
            for (int i = 0; i < r_BoardSize; i++)
            {
                for (int j = 0; j < r_BoardSize; j++)
                {
                    r_Board[i, j].ResetSettings();
                    r_Board[i, j].Click -= soldierButton_Click;
                    r_Board[i, j].Click -= optionalMoveButton_Click;
                    r_Board[i, j].Click -= emptyButton_Click;
                }
            }
        }

        private void initGame(int i_BoardSize)
        {
            int upSidePlayerStartRowInBoardInit = 0;
            int upSidePlayerEndRowInBoardInit = (r_BoardSize - 2) / 2;
            int downSidePlayerStartRowInBoardInit = r_BoardSize - upSidePlayerEndRowInBoardInit;
            int downSidePlayerEndRowInBoardInit = r_BoardSize;
            int emptySquresStartRow = upSidePlayerEndRowInBoardInit;
            int emptySquresEndRow = downSidePlayerStartRowInBoardInit;
            int buttonYPosition = labelPlayer1Name.Location.Y - (int)eFormPositions.BoardSpaceFromLable;
            int buttonXPosition = (int)eFormPositions.BoardLeftPosition;

            boardInitArranging(
                upSidePlayerStartRowInBoardInit, upSidePlayerEndRowInBoardInit, ePlayerSide.UpSidePlayer, ref buttonXPosition, ref buttonYPosition);
            boardInitArranging(emptySquresStartRow, emptySquresEndRow, 0, ref buttonXPosition, ref buttonYPosition);
            boardInitArranging(
                downSidePlayerStartRowInBoardInit, downSidePlayerEndRowInBoardInit, ePlayerSide.DownSidePlayer, ref buttonXPosition, ref buttonYPosition);

            int right = r_Board[i_BoardSize - 1, i_BoardSize - 1].Right + (int)eFormPositions.BoardSpaceFromRight;
            int down = r_Board[i_BoardSize - 1, i_BoardSize - 1].Bottom + (int)eFormPositions.BoardSpaceFromBottom;
            this.Size = new Size(right, down);
        }

        private void addDamkaButtonsBoardToForm()
        {
            for (int i = 0; i < r_BoardSize; i++) 
            {
                for (int j = 0; j < r_BoardSize; j++)
                {
                    r_Board[i, j] = new DamkaButton(i, j);
                    this.Controls.Add(this.r_Board[i, j]);
                }
            }
        }

        private void boardInitArranging(int i_StartRow, int i_LastRow, ePlayerSide i_PlayerSide, ref int io_ButtonXPosition, ref int io_ButtonYPosition)
        {
            bool v_IsEnabled = true;
            int buttonStartXPosition = io_ButtonXPosition;

            for (int i = i_StartRow; i < i_LastRow; i++)
            {
                io_ButtonYPosition += DamkaButton.ButtonSize;
                for (int j = 0; j < r_BoardSize; j++)
                {
                    io_ButtonXPosition += DamkaButton.ButtonSize;
                    r_Board[i, j].Location = new Point(io_ButtonXPosition, io_ButtonYPosition);
                    if (isButtonInBoard(i, j)) 
                    {
                        r_Board[i, j].SetButtonSettings(i_PlayerSide, v_IsEnabled);
                        r_Board[i, j].Click += emptyButton_Click;
                        if (i_PlayerSide == ePlayerSide.DownSidePlayer || i_PlayerSide == ePlayerSide.UpSidePlayer) 
                        {
                            r_Board[i, j].Click += soldierButton_Click;
                        }
                    }
                    else
                    {
                        r_Board[i, j].SetButtonSettings(ePlayerSide.NotUsed, !v_IsEnabled);
                    }
                }

                io_ButtonXPosition = buttonStartXPosition;
            }
        }

        private bool isButtonInBoard(int i_Row, int i_Col)
        {
            bool inBoard = (i_Row + i_Col) % 2 == 1;

            return inBoard;
        }

        private void updateClickableDamkaButtons(ePlayerSide i_PlayerTurn)
        {
            for (int i = 0; i < r_BoardSize; i++)
            {
                for (int j = 0; j < r_BoardSize; j++)
                {
                    if ((r_Board[i, j].Owner != i_PlayerTurn && r_Board[i, j].Owner != ePlayerSide.Empty) ||
                        (r_OpponentIsComputer && m_PlayerTurn == ePlayerSide.UpSidePlayer))
                    {
                        r_Board[i, j].Enabled = false;
                    }
                    else
                    {
                        r_Board[i, j].Enabled = true;
                    }
                }
            }
        }

        private void soldierButton_Click(object sender, EventArgs e)
        {
            bool v_MarkSoldier = true;
            DamkaButton senderButton = sender as DamkaButton;

            if (senderButton != null)
            {
                m_LastClickedSquare.RowIndex = senderButton.Row;
                m_LastClickedSquare.ColIndex = senderButton.Col;
                changeMarkedAndUnmarkedBackground(senderButton, v_MarkSoldier);
                updateButtonsStateAfterClick();
                if (senderButton.State >= eDamkaButtonState.StartOfMarkedValues)
                {
                    updatePossibleMoves(v_MarkSoldier);
                }
                else
                {
                    updatePossibleMoves(!v_MarkSoldier);
                }

                updateDamkaButtonBoardChanges();
            }
        }

        private void updatePossibleMoves(bool i_MarkSoldier)
        {
            List<Square> listOfValidMoves = m_DamkaGame.GetListOfValidMoves(
                m_LastClickedSquare.ColIndex,
                m_LastClickedSquare.RowIndex,
                m_PlayerTurn, 
                m_JumpEatExist);

            updateListOfValidMoves(ref listOfValidMoves);
            foreach (Square position in listOfValidMoves)
            {
                if (i_MarkSoldier)
                {
                    r_Board[position.RowIndex, position.ColIndex].State = eDamkaButtonState.OptionalForMove;
                    r_Board[position.RowIndex, position.ColIndex].Click += optionalMoveButton_Click;
                }
                else
                {
                    r_Board[position.RowIndex, position.ColIndex].State = eDamkaButtonState.Empty;
                    r_Board[position.RowIndex, position.ColIndex].Click -= optionalMoveButton_Click;
                }
            }
        }

        private void updateButtonsStateAfterClick()
        {
            for (int i = 0; i < r_BoardSize; i++)
            {
                for (int j = 0; j < r_BoardSize; j++)
                {
                    if (!(i == m_LastClickedSquare.RowIndex && j == m_LastClickedSquare.ColIndex)) 
                    {
                        if (r_Board[i, j].Owner == m_PlayerTurn)
                        {
                            changeMarkedAndUnmarkedBackground(r_Board[i, j], false);
                        }
                        else if (r_Board[i, j].Owner == ePlayerSide.Empty && r_Board[i, j].State >= eDamkaButtonState.StartOfMarkedValues)
                        {
                            r_Board[i, j].State = eDamkaButtonState.Empty;
                            changeMarkedAndUnmarkedBackground(r_Board[i, j], false);
                        }

                        r_Board[i, j].Click -= optionalMoveButton_Click;
                    }
                }
            }
        }

        private void emptyButton_Click(object sender, EventArgs e)
        {
            DamkaButton senderButton = sender as DamkaButton;

            if (senderButton != null) 
            {
                if(senderButton.State == eDamkaButtonState.Empty)
                {
                    MessageBox.Show("invalid choice", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void updateDamkaButtonBoardChanges()
        {
            for (int i = 0; i < r_BoardSize; i++)
            {
                for (int j = 0; j < r_BoardSize; j++) 
                {
                    r_Board[i, j].UpdateBackground(r_HelpMode);
                }
            }
        }

        private void changeMarkedAndUnmarkedBackground(DamkaButton i_DamkaButton, bool i_Mark)
        {
            if (i_DamkaButton.State < eDamkaButtonState.StartOfMarkedValues && i_Mark)
            {
                i_DamkaButton.State += (int)eDamkaButtonState.StartOfMarkedValues;
            }
            else if (i_DamkaButton.State >= eDamkaButtonState.StartOfMarkedValues && i_DamkaButton.State <= eDamkaButtonState.EndOfMarkedValues)
            {
                i_DamkaButton.State -= (int)eDamkaButtonState.StartOfMarkedValues;
            }
        }

        private void optionalMoveButton_Click(object sender, EventArgs e)
        {
            DamkaButton senderButton = sender as DamkaButton;
            bool v_moveWasMade = true;

            if (senderButton != null)
            {
                updatePossibleMoves(!v_moveWasMade);
                changeStateAfterMove(r_Board[m_LastClickedSquare.RowIndex, m_LastClickedSquare.ColIndex], senderButton);
                m_DamkaGame.RunPlayerTurn(
                    m_LastClickedSquare.ColIndex, m_LastClickedSquare.RowIndex, senderButton.Col, senderButton.Row, m_PlayerTurn, ref m_JumpEatExist);
                m_MoveWasMade = true;
                if(m_JumpEatExist)
                {
                    m_LastEaterSoldier.RowIndex = senderButton.Row;
                    m_LastEaterSoldier.ColIndex = senderButton.Col;
                }

                handleTurn();
            }
        }

        private void changeStateAfterMove(DamkaButton i_DamkaButtonMoveFrom, DamkaButton i_DamkaButtonMoveTo)
        {
            switch(i_DamkaButtonMoveFrom.State)
            {
                case eDamkaButtonState.MarkedBlackKing:
                    i_DamkaButtonMoveTo.State = eDamkaButtonState.BlackKing;
                        break;
                case eDamkaButtonState.MarkedRedKing:
                    i_DamkaButtonMoveTo.State = eDamkaButtonState.RedKing;
                    break;
                case eDamkaButtonState.RegularBlackMarked:
                    i_DamkaButtonMoveTo.State = eDamkaButtonState.RegularBlack;
                    break;
                case eDamkaButtonState.RegularRedMarked:
                    i_DamkaButtonMoveTo.State = eDamkaButtonState.RegularRed;
                    break;
            }

            i_DamkaButtonMoveFrom.State = eDamkaButtonState.Empty;
            i_DamkaButtonMoveFrom.Owner = ePlayerSide.Empty;
            i_DamkaButtonMoveFrom.Click -= soldierButton_Click;
            i_DamkaButtonMoveTo.Owner = m_PlayerTurn;
            if (m_PlayerTurn == ePlayerSide.DownSidePlayer || !r_OpponentIsComputer)
            {
                i_DamkaButtonMoveFrom.Click -= soldierButton_Click;
                i_DamkaButtonMoveTo.Click += soldierButton_Click;
                i_DamkaButtonMoveTo.Enabled = true;
            }
        }

        private void handleTurn()
        {
            if (!r_OpponentIsComputer)
            {
                updatesAfterPlayerTurn();
                updateDamkaButtonBoardChanges();
            }
            else
            {
                if(m_PlayerTurn == ePlayerSide.DownSidePlayer)
                {
                    updatesAfterPlayerTurn();
                    updateDamkaButtonBoardChanges();
                    if (m_PlayerTurn == ePlayerSide.UpSidePlayer) 
                    {
                        timerComputerMoveActivate.Start();
                    }
                }
            }
        }

        private void updatesAfterPlayerTurn()
        {
            bool isGameFinishInTie = false;

            if (m_MoveWasMade)
            {
                nextPlayerTurn();
                updateClickableDamkaButtons(m_PlayerTurn);
                m_MoveWasMade = false;
                m_JumpEatExist = false;
                ePlayerSide winnerPlayer = m_DamkaGame.WinCheck(m_PlayerTurn, ref isGameFinishInTie);
                updateDamkaButtonBoardChanges();
                checkWin(winnerPlayer, isGameFinishInTie);
            }
        }

        private void checkWin(ePlayerSide i_WinnerPlayer, bool i_IsGameFinishInTie)
        {
            int upPlayerPoints = 0;
            int downPlayerPoints = 0;
            string massage = string.Empty;

            if (checkIfWinnerOrTieExist(i_WinnerPlayer, i_IsGameFinishInTie, ref massage))
            {
                m_DamkaGame.CalcPlayersTotalPoints(ref upPlayerPoints, ref downPlayerPoints);
                updatePointsAfterGameFinished(upPlayerPoints, downPlayerPoints);
                if (MessageBox.Show(massage, "Damka", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    setNewRound(r_BoardSize, labelPlayer1Name.Text, labelPlayer2Name.Text);
                }
                else
                {
                    this.Close();
                }
            }
        }

        private bool checkIfWinnerOrTieExist(ePlayerSide i_WinnerPlayer, bool i_IsGameFinishInTie, ref string io_Message)
        {
            bool gameFinished = true;

            if (i_WinnerPlayer != ePlayerSide.Empty)
            {
                string winnerName = i_WinnerPlayer == ePlayerSide.DownSidePlayer ? r_DownPlayerName : r_UpPlayerName;
                io_Message = string.Format("{0} Won! {1}Another Round?", winnerName, Environment.NewLine);
            }
            else if (i_IsGameFinishInTie)
            {
                io_Message = string.Format("Tie! {0}Another Round?", Environment.NewLine);
            }
            else
            {
                gameFinished = false;
            }

            return gameFinished;
        }

        private void updatePointsAfterGameFinished(int i_UpPlayerPoints, int i_DownPlayerPoints)
        {
            if(i_UpPlayerPoints > i_DownPlayerPoints)
            {
                m_UpPlayerPoints += i_UpPlayerPoints - i_DownPlayerPoints;
            }
            else
            {
                m_DownPlayerPoints += i_DownPlayerPoints - i_UpPlayerPoints;
            }

            labelPlayer1Score.Text = m_DownPlayerPoints.ToString();
            labelPlayer2Score.Text = m_UpPlayerPoints.ToString();
        }

        private void makeComputerMove()
        {
            int fromCol = 0;
            int fromRow = 0;
            int toCol = 0;
            int toRow = 0;
            List<ComputerMove> computerListMoves = m_DamkaGame.GetAllValidMovesByPlayerSide(ePlayerSide.UpSidePlayer);
            bool isNextMoveExist = ComputerMove.PickComputerNextMove(
                computerListMoves, m_DamkaGame.Board, ref fromCol, ref fromRow, ref toCol, ref toRow);

            if (isNextMoveExist == true)
            {
                if (m_DamkaGame.RunPlayerTurn(fromCol, fromRow, toCol, toRow, m_PlayerTurn, ref m_JumpEatExist))
                {
                    updateDamkaButtonsAfterComputerMove(fromCol, fromRow, toCol, toRow);
                    nextPlayerTurn();
                }
            }
        }

        private void updateDamkaButtonsAfterComputerMove(int i_FromCol, int i_FromRow, int i_ToCol, int i_ToRow)
        {
            if(r_Board[i_ToRow, i_ToCol].State == eDamkaButtonState.BlackKing)
            {
                r_Board[i_FromRow, i_FromCol].State = eDamkaButtonState.BlackKing;
            }

            changeComputerToMarked(r_Board[i_FromRow, i_FromCol]);
            changeStateAfterMove(r_Board[i_FromRow, i_FromCol], r_Board[i_ToRow, i_ToCol]);
        }

        private void changeComputerToMarked(DamkaButton i_DamkaButton)
        {
            switch (i_DamkaButton.State)
            {
                case eDamkaButtonState.BlackKing:
                    i_DamkaButton.State = eDamkaButtonState.MarkedBlackKing;
                    break;
                case eDamkaButtonState.RegularBlack:
                    i_DamkaButton.State = eDamkaButtonState.RegularBlackMarked;
                    break;
                default:
                    break;
            }
        }

        private void nextPlayerTurn()
        {
            if (m_JumpEatExist == false)
            {
                if (m_PlayerTurn == ePlayerSide.UpSidePlayer)
                {
                    m_PlayerTurn++;
                }
                else
                {
                    m_PlayerTurn--;
                }
            }
            else
            {
                if (r_OpponentIsComputer && m_PlayerTurn == ePlayerSide.UpSidePlayer)
                {
                    timerComputerMoveActivate.Start();
                }
            }

            markPlayerTurnName();
        }

        private void markPlayerTurnName()
        {
            if(m_PlayerTurn == ePlayerSide.DownSidePlayer)
            {
                labelPlayer1Name.ForeColor = Color.Gold;
                labelPlayer1Score.ForeColor = Color.Gold;
                labelPlayer2Name.ForeColor = Color.Black;
                labelPlayer2Score.ForeColor = Color.Black;
            }
            else
            {
                labelPlayer2Name.ForeColor = Color.Gold;
                labelPlayer2Score.ForeColor = Color.Gold;
                labelPlayer1Name.ForeColor = Color.Black;
                labelPlayer1Score.ForeColor = Color.Black;
            }
        }

        private void damkaButton_Changed(object sender, SoldierEventArgs e)
        {
            r_Board[e.Position.RowIndex, e.Position.ColIndex].State = eDamkaButtonState.Empty;
            r_Board[e.Position.RowIndex, e.Position.ColIndex].Click -= soldierButton_Click;
            r_Board[e.Position.RowIndex, e.Position.ColIndex].Owner = ePlayerSide.Empty;
        }

        private void damkaButton_SetKing(object sender, SoldierEventArgs e)
        {
            r_Board[e.Position.RowIndex, e.Position.ColIndex].State = m_PlayerTurn == ePlayerSide.DownSidePlayer ? eDamkaButtonState.RedKing : eDamkaButtonState.BlackKing;
            r_Board[e.Position.RowIndex, e.Position.ColIndex].SetKingButton(m_PlayerTurn);
        }

        private void updateListOfValidMoves(ref List<Square> io_ListOfValidMoves)
        {
            bool deleteMove = true;
            List<ComputerMove> validMoves = m_DamkaGame.GetAllValidMovesByPlayerSide(m_PlayerTurn);

            for (int i = 0; i < io_ListOfValidMoves.Count; i++) 
            {
                deleteMove = true;
                for (int j = 0; j < validMoves.Count; j++)
                {
                    if(compareMoves(validMoves[j], io_ListOfValidMoves[i]))
                    {
                        deleteMove = false;
                    }
                }

                if(deleteMove)
                {
                    io_ListOfValidMoves.RemoveAt(i);
                    i--;
                }
            }
        }

        private bool compareMoves(ComputerMove i_ValidMove, Square i_OptionalMove)
        {
            bool isMovesEqual = false;

            if (i_ValidMove.MoveFromCol == m_LastClickedSquare.ColIndex &&
                i_ValidMove.MoveFromRow == m_LastClickedSquare.RowIndex &&
                i_ValidMove.DestinationSquare.ColIndex == i_OptionalMove.ColIndex &&
                i_ValidMove.DestinationSquare.RowIndex == i_OptionalMove.RowIndex)
            {
                isMovesEqual = true;
            }

            return isMovesEqual;
        }

        private void damkaButton_RemoveKing(object sender, SoldierEventArgs e)
        {
            r_Board[e.Position.RowIndex, e.Position.ColIndex].State = eDamkaButtonState.Empty;
        }

        private void timerComputerMoveActivate_Tick(object sender, EventArgs e)
        {
            bool isGameFinishInTie = false;
            Timer timer = sender as Timer;
            timer.Stop();
            makeComputerMove();
            updateClickableDamkaButtons(m_PlayerTurn);
            ePlayerSide winnerPlayer = m_DamkaGame.WinCheck(m_PlayerTurn, ref isGameFinishInTie);
            updateDamkaButtonBoardChanges();
            checkWin(winnerPlayer, isGameFinishInTie);
        }
    }
}