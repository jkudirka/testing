using DataContracts;
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
    public partial class UserForm : Form
    {
        public User User { get; private set; }

        public UserForm(User user)
        {
            InitializeComponent();
            this.User = user;

            Username.Text = user.Username;
            Password.Text = user.Password;
            FirstName.Text = user.FirstName;
            LastName.Text = user.LastName;
            IsLocked.Checked = user.IsLocked;
            if (user.PasswordLastChangedDate.HasValue)
                PasswordLastChanged.Text = user.PasswordLastChangedDate.Value.ToString();
            if (user.FailedLoginAttempts.HasValue)
                FailedLoginAttempts.Text = user.FailedLoginAttempts.Value.ToString();
        }

        private void Okay_Click(object sender, EventArgs e)
        {
            User.Username = Username.Text;
            User.Password = Password.Text;
            User.FirstName = FirstName.Text;
            User.LastName = LastName.Text;
            User.IsLocked = IsLocked.Checked;
            
            int attempts = 0;
            if (int.TryParse(FailedLoginAttempts.Text, out attempts))
                User.FailedLoginAttempts = attempts;

            this.DialogResult = DialogResult.OK;
        }
    }
}
