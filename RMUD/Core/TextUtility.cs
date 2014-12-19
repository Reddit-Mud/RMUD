using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public static partial class Mud
    {
        public static String CapFirst(String str)
        {
            if (str.Length > 1)
                return str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
            if (str.Length == 1)
                return str.ToUpper();
            return str;
        }

        public static void AssembleText(LinkedListNode<String> Node, StringBuilder Builder)
        {
            for (; Node != null; Node = Node.Next)
            {
                Builder.Append(Node.Value);
                Builder.Append(" ");
            }

            Builder.Remove(Builder.Length - 1, 1);
        }

        public static String RestText(Object Node)
        {
            var _node = Node as LinkedListNode<String>;
            if (_node == null) return "";
            var builder = new StringBuilder();
            AssembleText(_node, builder);
            return builder.ToString();
        }       
    }
}
