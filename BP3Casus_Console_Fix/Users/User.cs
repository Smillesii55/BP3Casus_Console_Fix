using BP3Casus_Console_Fix.Users.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP3Casus_Console_Fix.Users
{
    public class User
    {
        UserDataAccesLayer UserDataAccesLayer = UserDataAccesLayer.Instance;

        public int Id { get; set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public string Email { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public DateTime DateOfBirth { get; private set; }
        public DateTime CreationDate { get; private set; }

        public UserType Type { get; set; }
        public enum UserType
        {
            Participant,
            Coach,
            Admin
        }

        public User(string username, string password, string email, string firstName, string lastName, DateTime dateOfBirth)
        {
            Username = username;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }

        public void ChangeUsername(string newUsername)
        {
            Username = newUsername;
            UserDataAccesLayer.UpdateUser(this);
        }
        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
            UserDataAccesLayer.UpdateUser(this);
        }

        public void ChangeFirstName(string newFirstName)
        {
            FirstName = newFirstName;
            UserDataAccesLayer.UpdateUser(this);
        }
        public void ChangeLastName(string newLastName)
        {
            LastName = newLastName;
            UserDataAccesLayer.UpdateUser(this);
        }
        public void ChangeEmail(string newEmail)
        {
            Email = newEmail;
            UserDataAccesLayer.UpdateUser(this);
        }

        public void ChangeDateOfBirth(DateTime newDateOfBirth)
        {
            DateOfBirth = newDateOfBirth;
            UserDataAccesLayer.UpdateUser(this);
        }

        public void ChangeType(UserType newType)
        {
            Type = newType;
            UserDataAccesLayer.UpdateUser(this);
        }

        public void DELETEACCOUNT()
        {
            UserDataAccesLayer.RemoveUser(this);
        }

    }
}
