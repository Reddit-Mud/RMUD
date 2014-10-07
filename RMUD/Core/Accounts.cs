using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static partial class Mud
    {
        public static List<Account> Accounts = new List<Account>();

        public static Account FindAccount(String UserName)
        {
            return Accounts.FirstOrDefault(a => a.UserName == UserName);
        }

        private static String HashPassword(String Password)
        {
            var sha = System.Security.Cryptography.SHA512.Create();
            var bytes = System.Text.Encoding.ASCII.GetBytes(Password);
            var hash = sha.ComputeHash(bytes);
            return System.Convert.ToBase64String(hash);
        }

        public static bool VerifyAccount(Account Account, String Password)
        {
            var saltedPassword = Password + Account.UserName + "SECURITAS";
            var hashedPassword = HashPassword(saltedPassword);

            return Account.HashedPassword == hashedPassword;
        }

        public static Account CreateAccount(String UserName, String Password)
        {
            if (FindAccount(UserName) != null) throw new InvalidOperationException();
            var hashedPassword = HashPassword(Password + UserName + "SECURITAS");
            var newAccount = new Account { UserName = UserName, HashedPassword = hashedPassword };
            Accounts.Add(newAccount);
            return newAccount;
        }

        public static Actor CreateCharacter(Account Account, String CharacterName)
        {
            var newCharacter = new Actor();
            newCharacter.Short = CharacterName;
            newCharacter.Nouns.Add(CharacterName.ToUpper());
            newCharacter.Path = "account/" + Account.UserName;
            newCharacter.Instance = Guid.NewGuid().ToString();
            Account.Character = newCharacter.Instance;
            return newCharacter;
        }

        public static Actor GetAccountCharacter(Account Account)
        {
            return Account.LoggedInCharacter;

            //This should actually create a new actor and load it's data from the database.
        }
    }
}