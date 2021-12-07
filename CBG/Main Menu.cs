using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CBG
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
              InitializeComponent();
        }

        private void LoadGame(object sender, EventArgs e)
        {
            frmGame gameWindow = new frmGame(this);

            Hide();
            gameWindow.Show();
        }
    }
}
