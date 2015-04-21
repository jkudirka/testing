using Common;
using DataContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class UserStore
{
    #region Fields
    private List<User> _Users;
    private readonly string _FilePath;
    #endregion

    #region Constructor
    public UserStore(string filePath)
    {
        Debug.Assert(!string.IsNullOrEmpty(filePath));

        _FilePath = filePath;

        if (!File.Exists(_FilePath))
        {
            GenerateDefaultUsers();
            SaveUsers();
        }
        else
        {
            LoadUsers();
        }
    }
    #endregion

    #region Methods
    public IEnumerable<User> GetUsers(string usernameFilter = null)
    {
        Debug.Assert(_Users != null);

        if (!string.IsNullOrEmpty(usernameFilter))
            return _Users.Where(u => u.Username.StartsWith(usernameFilter, StringComparison.OrdinalIgnoreCase)).OrderBy(u => u.Username);
        return _Users;
    }

    public User GetUser(string username)
    {
        Debug.Assert(_Users != null);
        Debug.Assert(!string.IsNullOrEmpty(username));

        return _Users.FirstOrDefault<User>(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public void AddUser(User user)
    {
        Debug.Assert(_Users != null);
        Debug.Assert(user != null);

        _Users.Add(user);
        SaveUsers();
    }

    public void UpdateUser(User user)
    {
        Debug.Assert(_Users != null);
        Debug.Assert(user != null);

        var existingUser = GetUser(user.Username);
        if (existingUser != null)
            _Users.Remove(existingUser);
        AddUser(user);
    }

    public void DeleteUser(User user)
    {
        Debug.Assert(_Users != null);
        Debug.Assert(user != null);

        _Users.Remove(user);
        SaveUsers();
    }
    #endregion

    #region Private Methods

    private void SaveUsers()
    {
        Debug.Assert(!string.IsNullOrEmpty(_FilePath));
        Debug.Assert(_Users != null);

        var fs = new FileStream(_FilePath, FileMode.OpenOrCreate);
        var formatter = new BinaryFormatter();

        try
        {
            formatter.Serialize(fs, _Users);
        }
        finally
        {
            fs.Close();
        }
    }

    private void LoadUsers()
    {
        Debug.Assert(!string.IsNullOrEmpty(_FilePath));

        var fs = new FileStream(_FilePath, FileMode.Open, FileAccess.ReadWrite);
        Debug.Assert(fs != Stream.Null);
        var formatter = new BinaryFormatter();

        try
        {
            _Users = (List<User>)formatter.Deserialize(fs);
        }
        finally
        {
            fs.Close();
        }
    }

    private void GenerateDefaultUsers()
    {
        _Users = new List<User>(1)
        {
		    new User 
            {
                Username = "admin", 
                Password = Helpers.HashPassword("Penlink123"), 
                FirstName="Admin", 
                LastName="User", 
                FailedLoginAttempts=0, 
                IsLocked=false,
                PasswordLastChangedDate = DateTime.UtcNow.Date,
            }
        };
    }

    #endregion
}
