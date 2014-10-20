using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace RMUD
{
    public static partial class Mud
    {
        public static List<Account> Accounts = new List<Account>();

        public static Account FindAccount(String UserName)
        {
            var account = Accounts.FirstOrDefault(a => a.UserName == UserName);
            if (account == null)
            {
                account = LoadAccount(UserName);
                if (account != null)
                {
                    Accounts.Add(account);
                }
            }
            return account;
        }

        public static string GenerateRandomSalt()
        {
            var bytes = new Byte[64];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static String HashPassword(String Password, String Salt)
        {
            var sha = System.Security.Cryptography.SHA512.Create();
            var bytes = System.Text.Encoding.ASCII.GetBytes(Salt + Password);
            var hash = sha.ComputeHash(bytes);
            return System.Convert.ToBase64String(hash);
        }

        public static bool VerifyAccount(Account Account, String Password)
        {
            var hashedPassword = HashPassword(Password, Account.Salt);

            return Account.HashedPassword == hashedPassword;
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
            SaveAccount(newAccount);
            return newAccount;
        }

        public static Actor GetAccountCharacter(Account Account)
        {
            var character = new Actor();
            character.Path = "account/" + Account.UserName;
            character.Instance = "main";
            character.Short = Account.UserName;
            character.Nouns.Add(Account.UserName.ToUpper());
            Mud.PersistInstance(character);            
            return character;
        }

        private static void SaveAccount(Account account)
        {
            try
            {
                var directory = Mud.AccountsPath + account.UserName;
                var filename = directory + "/account.txt";
                var json = JsonConvert.SerializeObject(account, Formatting.Indented);
                System.IO.Directory.CreateDirectory(directory);
                File.WriteAllText(filename, json);
            }
            catch (Exception e)
            {
                Mud.LogError(String.Format("While saving account {0} - {1}", account.UserName, e.Message));
            }
        }

        private static Account LoadAccount(String UserName)
        {
            Account account = null;
            try 
            {
                var filename = Mud.AccountsPath + UserName + "/account.txt";
                if (File.Exists(filename))
                {
                    var json = File.ReadAllText(filename);
                    account = JsonConvert.DeserializeObject<Account>(json);
                }
            }
            catch (Exception e)
            {
                Mud.LogError(String.Format("While loading account {0} - {1}", UserName, e.Message));
            }

            return account;
        }
    }
}
