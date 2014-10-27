using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Quest : MudObject
    {
        public Player ActiveQuestor { get; set; }
       
        public virtual bool IsAvailable(Actor To) { return true; }
        public virtual bool IsComplete(Actor Questor) { return false; }
        public virtual void OnAccept(Actor Questor) { }
        public virtual void OnCompletion(Actor Questor) { }
        public virtual void OnAbandon(Actor Questor) { }
    }
}
