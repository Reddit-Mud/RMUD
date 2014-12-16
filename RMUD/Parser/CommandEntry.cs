using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class CommandEntry
    {
        internal CommandTokenMatcher Matcher;
        internal CommandProcessor Processor;
        internal String HelpText;
    }
}
