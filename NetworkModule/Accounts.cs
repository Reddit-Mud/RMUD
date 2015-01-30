using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using RMUD;

namespace NetworkModule
{
    public static class Accounts
    {
        private static String AccountsPath = "database/accounts/";

        private static string GenerateRandomSalt()
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

        internal static bool VerifyAccount(Account Account, String Password)
        {
            var hashedPassword = HashPassword(Password, Account.Salt);

            return Account.HashedPassword == hashedPassword;
        }

        internal static Account CreateAccount(String UserName, String Password)
        {
            if (LoadAccount(UserName) != null)
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
            SaveAccount(newAccount);
            return newAccount;
        }

        public static Player GetAccountCharacter(Account Account)
        {
            Core.CommandTimeoutEnabled = false;
            var playerObject = Core.Database.GetObject(Core.SettingsObject.PlayerBaseObject + "@" + Account.UserName) as Player;

            playerObject.Short = Account.UserName;
            playerObject.Nouns.Add(Account.UserName.ToUpper());
            MudObject.PersistInstance(playerObject);
            return playerObject;
        }

        internal static void SaveAccount(Account account)
        {
            try
            {
                var directory = AccountsPath + account.UserName;
                var filename = directory + "/account.txt";
                var json = JsonConvert.SerializeObject(account, Formatting.Indented);
                System.IO.Directory.CreateDirectory(directory);
                File.WriteAllText(filename, json);
            }
            catch (Exception e)
            {
                Core.LogError(String.Format("While saving account {0} - {1}", account.UserName, e.Message));
            }
        }

        internal static Account LoadAccount(String UserName)
        {
            Account account = null;
            try 
            {
                var filename = AccountsPath + UserName + "/account.txt";
                if (File.Exists(filename))
                {
                    var json = File.ReadAllText(filename);
                    account = JsonConvert.DeserializeObject<Account>(json);
                }
            }
            catch (Exception e)
            {
                Core.LogError(String.Format("While loading account {0} - {1}", UserName, e.Message));
            }

            return account;
        }
    }
}
