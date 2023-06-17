using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ex05.DamkaGameWindowsUI
{
    public static class Program
    {
        public static void Main()
        {
            Application.EnableVisualStyles();
            new FormDamkaGame().runGame();
        }
    }
}
