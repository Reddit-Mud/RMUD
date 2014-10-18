using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
            SaveAccount(newAccount);
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

        private static void SaveAccount(Account account)
        {
            try
            {
                var filename = Mud.AccountsPath + account.UserName + "/account.txt";
                var json = JsonConvert.SerializeObject(account, Formatting.Indented);
                File.WriteAllText(filename, json);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR", e.ToString());
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
                Console.WriteLine("ERROR", e.ToString());
            }

            return account;
        }
    }
}