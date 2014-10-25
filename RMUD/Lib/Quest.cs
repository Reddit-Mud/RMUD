using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Quest
    {
        public String Name;
        public String Description;

        public Func<Actor, Quest, bool> IsComplete;
        public Action<Actor, Quest> OnCompletion;
    }
}
