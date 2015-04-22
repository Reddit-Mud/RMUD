using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    /// <summary>
    /// Contains helper functions for parsing messages.
    /// </summary>
    internal static class MessageFormatParser
    {
        /// <summary>
        /// Parses a number, which may be multiple digits.
        /// </summary>
        /// <param name="From"></param>
        /// <param name="Number"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// A specifier is some block of text contained in < >, such as '<a1>'. While specifiers always follow the form 
        /// 'labelNUMBER', this parser will happily accept specifiers missing one or both parts.
        /// </summary>
        /// <param name="From"></param>
        /// <param name="Type">Out parameter. Set to the label parsed from the specifier. Blank if no label found.</param>
        /// <param name="Index">Out parameter. Set to the value of the integer index parsed from the specifier. 0 if no index found.</param>
        /// <returns>True if parsing succeeded, false if parsing failed</returns>
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
        /// <summary>
        /// Parse and format a message intended for a specific recipient.
        /// </summary>
        /// <param name="Recipient"></param>
        /// <param name="Message">The message, possibly containing specifiers.</param>
        /// <param name="Objects">Specifier indicies refer to this list of objects.</param>
        /// <returns></returns>
        public static String FormatMessage(Actor Recipient, String Message, params Object[] Objects)
        {
            //A leading @ indicates that the message should be interpretted as an entry in the global message table.
            if (Message[0] == '@') Message = Core.Message(Message.Substring(1));

            var formattedMessage = new StringBuilder();
            var iterator = new StringIterator(Message);

            while (!iterator.AtEnd)
            {
                if (iterator.Next == '<') //We have located a specifier.
                {
                    var type = "";
                    var index = 0;
                    if (MessageFormatParser.ReadSpecifier(iterator, out type, out index))
                    {
                        if (index < 0 || index >= Objects.Length) continue; //A blank in the output is preferable to crashing.

                        #region Expand Specifier
                        if (type == "the" && Objects[index] is MudObject)
                        {
                            //'the' overrides the object's article property.
                            formattedMessage.Append(GlobalRules.ConsiderValueRule<String>("printed name", Recipient, Objects[index], "the"));
                        }
                        else if (type == "a" && Objects[index] is MudObject)
                        {
                            formattedMessage.Append(GlobalRules.ConsiderValueRule<String>("printed name", Recipient, Objects[index], (Objects[index] as MudObject).Article));
                        }
                        else if (type == "l") //No connective clause is used for this style of list. eg 1, 2, 3.
                        {
                            FormatList(Recipient, Objects[index], formattedMessage, "");
                        }
                        else if (type == "lor") //Use or. eg 1, 2, or 3.
                        {
                            FormatList(Recipient, Objects[index], formattedMessage, "or");
                        }
                        else if (type == "land") //Use and. eg 1, 2, and 3.
                        {
                            FormatList(Recipient, Objects[index], formattedMessage, "and");
                        }
                        else if (type == "lnor") //Use nor. eg 1, 2, nor 3.
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

            //Apply the ^ transform: Capitalize the letter following the ^ and remove the ^.
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

        /// <summary>
        /// Format a list of mud objects using commas and a coordinating conjunction. EG, into the form 'a, b, and c'.
        /// </summary>
        /// <param name="Recipient">The actor that will, eventually, see the output.</param>
        /// <param name="ListObject">Either a MudObject or a List<MudObject>. The actual items to format.</param>
        /// <param name="FormattedMessage">Append the formatted message to this StringBuilder.</param>
        /// <param name="CoordinatingConjunction">The word that separates the final item of a list from those proceeding it. EG, and, or, nor.</param>
        private static void FormatList(
            Actor Recipient, 
            Object ListObject, 
            StringBuilder FormattedMessage,
            String CoordinatingConjunction)
        {
            // ListObject can be a MudObject or a List<MudObject>. The algorithm expects a list, so transform it.
            List<MudObject> list = null;
            if (ListObject is MudObject)
            {
                list = new List<MudObject>();
                list.Add(ListObject as MudObject);
            }
            else
                list = ListObject as List<MudObject>;

            // If ListObject was neither a MudObject nor a List<MudObject>...
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
