using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ex05.DamkaGameLogic;

namespace Ex05.DamkaGameWindowsUI
{
    public class DamkaButton : Button
    {
        private static readonly int sr_ButtonSize = 75;
        private readonly int r_Row;
        private readonly int r_Col;
        private ePlayerSide m_PlayerSideButton;
        private eDamkaButtonState m_State;

        public DamkaButton(int i_Row, int i_Col)
        {
            r_Row = i_Row;
            r_Col = i_Col;
            Width = sr_ButtonSize;
            Height = sr_ButtonSize;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public static int ButtonSize
        {
            get { return sr_ButtonSize; }
        }

        public int Row
        {
            get { return r_Row; }
        }

        public int Col
        {
            get { return r_Col; }
        }

        public ePlayerSide Owner
        {
            get { return m_PlayerSideButton; }
            set { m_PlayerSideButton = value; }
        }

        public eDamkaButtonState State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        public void SetButtonSettings(ePlayerSide i_PlayerSide, bool i_IsEnabled)
        {
            this.Enabled = i_IsEnabled;
            m_PlayerSideButton = i_PlayerSide;
            switch (i_PlayerSide)
            {
                case ePlayerSide.DownSidePlayer:
                    this.BackgroundImage = Ex05.DamkaGameWindowsUI.Properties.Resources.RedNotSelected;
                    m_State = eDamkaButtonState.RegularRed;
                    break;
                case ePlayerSide.UpSidePlayer:
                    this.BackgroundImage = Ex05.DamkaGameWindowsUI.Properties.Resources.blackNotSelected;
                    m_State = eDamkaButtonState.RegularBlack;
                    break;
                case ePlayerSide.Empty:
                    this.BackgroundImage = Ex05.DamkaGameWindowsUI.Properties.Resources.grey;
                    m_State = eDamkaButtonState.Empty;
                    break;
                case ePlayerSide.NotUsed:
                    this.BackgroundImage = Ex05.DamkaGameWindowsUI.Properties.Resources.Blank;
                    m_State = eDamkaButtonState.NotUsed;
                    break;
            }
        }

        public void UpdateBackground(bool i_HelpMode)
        {
            switch(m_State)
            {
                case eDamkaButtonState.BlackKing:
                    this.BackgroundImage = global::Ex05.DamkaGameWindowsUI.Properties.Resources.BlackKing;
                    break;
                case eDamkaButtonState.RedKing:
                    this.BackgroundImage = global::Ex05.DamkaGameWindowsUI.Properties.Resources.RedKing;
                    break;
                case eDamkaButtonState.RegularBlack:
                    this.BackgroundImage = global::Ex05.DamkaGameWindowsUI.Properties.Resources.blackNotSelected;
                    break;
                case eDamkaButtonState.RegularRed:
                    this.BackgroundImage = global::Ex05.DamkaGameWindowsUI.Properties.Resources.RedNotSelected;
                    break;
                case eDamkaButtonState.Empty:
                    this.BackgroundImage = Ex05.DamkaGameWindowsUI.Properties.Resources.grey;
                    m_PlayerSideButton = ePlayerSide.Empty;
                    break;
                case eDamkaButtonState.RegularRedMarked:
                    this.BackgroundImage = global::Ex05.DamkaGameWindowsUI.Properties.Resources.Selected;
                    break;
                case eDamkaButtonState.RegularBlackMarked:
                    this.BackgroundImage = global::Ex05.DamkaGameWindowsUI.Properties.Resources.Selected;
                    break;
                case eDamkaButtonState.MarkedRedKing:
                    this.BackgroundImage = global::Ex05.DamkaGameWindowsUI.Properties.Resources.BlueKing;
                    break;
                case eDamkaButtonState.MarkedBlackKing:
                    this.BackgroundImage = global::Ex05.DamkaGameWindowsUI.Properties.Resources.BlueKing;
                    break;
                case eDamkaButtonState.OptionalForMove:
                    setOptionalMovePicture(i_HelpMode);
                    break;
                default:
                    this.BackgroundImage = Ex05.DamkaGameWindowsUI.Properties.Resources.Blank;
                    break;
            }
        }

        private void setOptionalMovePicture(bool i_HelpMode)
        {
            if (i_HelpMode)
            {
                this.BackgroundImage = Ex05.DamkaGameWindowsUI.Properties.Resources.Optional;
            }
            else
            {
                this.BackgroundImage = Ex05.DamkaGameWindowsUI.Properties.Resources.grey;
            }
        }

        public void ResetSettings()
        {
            m_State = eDamkaButtonState.Empty;
            m_PlayerSideButton = ePlayerSide.Empty;
        }

        public void SetKingButton(ePlayerSide i_Player)
        {
            switch(i_Player)
            {
                case ePlayerSide.UpSidePlayer:
                    m_State = eDamkaButtonState.BlackKing;
                    break;
                case ePlayerSide.DownSidePlayer:
                    m_State = eDamkaButtonState.RedKing;
                    break;
            }
        }
    }
}
