﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ex05.DamkaGameWindowsUI
{
    public partial class FormGameSettings : Form
    {
        private string m_Player1Name;
        private string m_Player2Name;
        private int m_BoardSize;
        private bool m_PlayAgainstComputer = true;
        private bool m_DoneButtonWasClicked = false;
        private bool m_HelpMode = false;

        public FormGameSettings()
        {
            InitializeComponent();
        }

        public void GetParameters(
            out int o_BoardSize,
            out string o_Player1Name,
            out string o_Player2Name,
            out bool o_PlayAgainstComputer,
            out bool o_DoneButtonClicked,
            out bool o_HelpMode)
        {
            o_DoneButtonClicked = m_DoneButtonWasClicked;
            o_Player1Name = m_Player1Name;
            o_Player2Name = m_Player2Name;
            o_BoardSize = m_BoardSize;
            o_PlayAgainstComputer = m_PlayAgainstComputer;
            o_HelpMode = m_HelpMode;
        }

        private void checkBoxPlayer2_Click(object sender, EventArgs e)
        {
            if (!textBoxPlayer2Name.Enabled)
            {
                textBoxPlayer2Name.Enabled = true;
                textBoxPlayer2Name.Text = string.Empty;
                m_PlayAgainstComputer = false;
            }
            else 
            {
                textBoxPlayer2Name.Enabled = false;
                textBoxPlayer2Name.Text = "[Computer]";
                m_PlayAgainstComputer = true;
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if (radioButton1.Enabled || radioButton2.Enabled || radioButton3.Enabled)
            {
                if (textBoxPlayer1Name.Text != string.Empty)
                {
                    if (checkBoxPlayer2.Enabled && textBoxPlayer2Name.Text != string.Empty)
                    {
                        setParameters();
                        m_DoneButtonWasClicked = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("You must enter player 2 name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("You must enter player 1 name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }
            else
            {
                MessageBox.Show("You must choose board size", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private enum BoardSize
        {
            Small = 6,
            Medium = 8,
            Big = 10
        }

        private void setParameters()
        {
            m_Player1Name = textBoxPlayer1Name.Text;
            m_Player2Name = textBoxPlayer2Name.Text == "[Computer]" ? "Computer" : textBoxPlayer2Name.Text;
            if(radioButton1.Checked)
            {
                m_BoardSize = (int)BoardSize.Small;
            }
            else if(radioButton2.Checked)
            {
                m_BoardSize = (int)BoardSize.Medium;
            }
            else
            {
                m_BoardSize = (int)BoardSize.Big;
            }
        }

        private void textBoxPlayer1Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void textBoxPlayer2Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void checkBoxHelpMode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelpMode.Checked)
            {
                m_HelpMode = true;
            }
            else
            {
                m_HelpMode = false;
            }
        }
    }
}
