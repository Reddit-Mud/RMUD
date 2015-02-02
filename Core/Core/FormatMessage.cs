using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    internal static class MessageFormatParser
    {
        private static bool ReadNumber(StringIterator From, out int Number)
        {
            Number = 0;
            var digitString = "";
            while (!From.AtEnd && IsDigit(From.Next))
            {
                digitString += From.Next;
                From.Advance();
            }
            if (digitString.Length == 0) return false;
            Number = Convert.ToInt32(digitString);
            return true;
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        internal static bool ReadSpecifier(StringIterator From, out String Type, out int Index)
        {
            Type = "";
            Index = 0;

            if (From.Next != '<') return false;

            From.Advance();
            while (!From.AtEnd && From.Next != '>' && !IsDigit(From.Next))
            {
                Type += From.Next;
                From.Advance();
            }

            if (IsDigit(From.Next)) if (!ReadNumber(From, out Index)) return false;
            if (From.AtEnd || From.Next != '>') return false;
            From.Advance();
            return true;
        }
    }

    public static partial class Core
    {
        public static String FormatMessage(Actor Recipient, String Message, params Object[] Objects)
        {
            if (Message[0] == '@') Message = Core.Message(Message.Substring(1));

            var formattedMessage = new StringBuilder();
            var iterator = new StringIterator(Message);

            while (!iterator.AtEnd)
            {
                if (iterator.Next == '<')
                {
                    var type = "";
                    var index = 0;
                    if (MessageFormatParser.ReadSpecifier(iterator, out type, out index))
                    {
                        if (index < 0 || index >= Objects.Length) continue;
                        #region Expand Specifier
                        if (type == "the" && Objects[index] is MudObject)
                        {
                            formattedMessage.Append(GlobalRules.ConsiderValueRule<String>("printed name", Recipient, Objects[index], "the"));
                        }
                        else if (type == "a" && Objects[index] is MudObject)
                        {
                            formattedMessage.Append(GlobalRules.ConsiderValueRule<String>("printed name", Recipient, Objects[index], (Objects[index] as MudObject).Article));
                        }
                        else if (type == "l")
                        {
                            FormatList(Recipient, Objects[index], formattedMessage, "");
                        }
                        else if (type == "lor")
                        {
                            FormatList(Recipient, Objects[index], formattedMessage, "or");
                        }
                        else if (type == "land")
                        {
                            FormatList(Recipient, Objects[index], formattedMessage, "and");
                        }
                        else if (type == "lnor")
                        {
                            FormatList(Recipient, Objects[index], formattedMessage, "nor");
                        }
                        else if (type == "s")
                        {
                            formattedMessage.Append(Objects[index].ToString());
                        }
                        #endregion
                    }
                }
                else
                {
                    formattedMessage.Append(iterator.Next);
                    iterator.Advance();
                }
            }

            Message = formattedMessage.ToString();
            formattedMessage.Clear();
            iterator = new StringIterator(Message);

            while (!iterator.AtEnd)
            {
                if (iterator.Next == '^')
                {
                    iterator.Advance();
                    if (iterator.AtEnd) break;
                    formattedMessage.Append(new String(iterator.Next, 1).ToUpper());
                    iterator.Advance();
                }
                else
                {
                    formattedMessage.Append(iterator.Next);
                    iterator.Advance();
                }
            }
            
            return formattedMessage.ToString();
        }

        private static void FormatList(
            Actor Recipient, 
            Object ListObject, 
            StringBuilder FormattedMessage,
            String CoordinatingConjunction)
        {
            List<MudObject> list = null;
            if (ListObject is MudObject)
            {
                list = new List<MudObject>();
                list.Add(ListObject as MudObject);
            }
            else
                list = ListObject as List<MudObject>;

            if (list == null) return;

            for (int x = 0; x < list.Count; ++x)
            {
                FormattedMessage.Append(GlobalRules.ConsiderValueRule<String>("printed name", Recipient, list[x], list[x].Article));
                if (x != list.Count - 1) FormattedMessage.Append(", ");
                if (x == list.Count - 2 && !String.IsNullOrEmpty(CoordinatingConjunction)) FormattedMessage.Append(CoordinatingConjunction + " ");
            }
        }
    }
}
