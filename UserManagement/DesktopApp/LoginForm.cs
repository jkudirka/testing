using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopApp
{
    public partial class LoginForm : Form
    {
        public string Username { get { return _Username.Text; } }
        public string Password { get { return _Password.Text; } }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void Okay_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.OK;
        }
    }
}
