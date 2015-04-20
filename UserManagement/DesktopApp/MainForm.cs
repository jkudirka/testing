using DataContracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Transactions;
using System.Windows.Forms;

namespace DesktopApp
{
    public partial class MainForm : Form
    {
        #region Fields
        private readonly ChannelFactory<IUserManager> _Factory;
        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            _Factory = new ChannelFactory<IUserManager>("UserManagerService");
        } 
        #endregion

        #region Event Handlers
        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshUsers(object sender, EventArgs e)
        {
            var proxy = _Factory.CreateChannel();
            RefreshUserListView(proxy.GetUsers());
            (proxy as ICommunicationObject).Close();
        }

        private void AddUser(object sender, EventArgs e)
        {
            var userForm = new UserForm(new User());

            if (userForm.ShowDialog() == DialogResult.OK)
            {
                var proxy = _Factory.CreateChannel();
                proxy.UpdateUser(userForm.User);
                (proxy as ICommunicationObject).Close();
            }
        }

        private void EditUser(object sender, EventArgs e)
        {
            if (_UsersListView.SelectedItems.Count == 0)
                return;

            var user = _UsersListView.SelectedItems[0].Tag as User;
            if (user == null)
                return;

            var userForm = new UserForm(user);

            if (userForm.ShowDialog() == DialogResult.OK)
            {
                using (TransactionScope socpe = new TransactionScope())
                {
                    var proxy = _Factory.CreateChannel();
                    proxy.UpdateUser(user);
                    (proxy as ICommunicationObject).Close();
                }
            }
        }

        private void DeleteUser(object sender, EventArgs e)
        {
            if (_UsersListView.SelectedItems.Count == 0)
                return;

            var user = _UsersListView.SelectedItems[0].Tag as User;
            if (user == null)
                return;

            var proxy = _Factory.CreateChannel();
            proxy.DeleteUser(user);
            (proxy as ICommunicationObject).Close();
        }

        private void ChangeUserPassword(object sender, EventArgs e)
        {

        }

        private void Login(object sender, EventArgs e)
        {

        }
        #endregion

        #region Private Methods
        private void RefreshUserListView(IEnumerable<User> users)
        {
            _UsersListView.Items.Clear();

            foreach (var user in users)
            {
                var item = _UsersListView.Items.Add(new ListViewItem(new string[]
                {
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.IsLocked.ToString(),
                    user.FailedLoginAttempts.ToString(),
                    user.PasswordLastChangedDate.ToString()
                }));
                item.Tag = user;
            }
        }
        #endregion
    }
}
