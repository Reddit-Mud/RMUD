using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace RMUD
{
    public static partial class Mud
    {
        public static List<Account> Accounts = new List<Account>();

        public static Account FindAccount(String UserName)
        {
            return Accounts.FirstOrDefault(a => a.UserName == UserName);
        }

        private static String HashPassword(String Password, String Salt)
        {
            var sha = System.Security.Cryptography.SHA512.Create();
            var bytes = System.Text.Encoding.ASCII.GetBytes(Salt + Password);
            var hash = sha.ComputeHash(bytes);
            return System.Convert.ToBase64String(hash);
        }

        public static string GenerateRandomSalt()
        {
            var bytes = new Byte[64];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyAccount(Account Account, String Password)
        {
            var test = HashPassword(Password, Account.Salt);

            return Account.HashedPassword == test;
        }

        public static Account CreateAccount(String UserName, String Password)
        {
            if (FindAccount(UserName) != null)
            {
                throw new InvalidOperationException("Account already exists");
            }

            if (String.IsNullOrWhiteSpace(Password))
            {
                throw new InvalidOperationException("A password must be specified when creating an account");
            }

            var salt = GenerateRandomSalt();
            var hash = HashPassword(Password, salt);
            var newAccount = new Account { UserName = UserName, HashedPassword = hash, Salt = salt };
            Accounts.Add(newAccount);
            return newAccount;
        }

        public static Actor GetAccountCharacter(Account Account)
        {
            var character = new Actor();
            character.Path = "account/" + Account.UserName;
            character.Instance = "main";

            character.Short = Account.UserName;
            character.Nouns.Add(Account.UserName.ToUpper());
            return character;
        }
    }
}