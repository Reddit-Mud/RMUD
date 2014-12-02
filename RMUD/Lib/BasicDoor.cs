using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class BasicDoor : Portal, OpenableRules
    {
        public BasicDoor()
        {
            this.Nouns.Add("DOOR");
            this.Nouns.Add("CLOSED", actor => !Open);
            this.Nouns.Add("OPEN", actor => Open);
            Open = false;
        }

        #region IOpenable

        public bool Open { get; set; }

        CheckRule OpenableRules.CheckOpen(Actor Actor)
        {
            if (Open) return CheckRule.Disallow("It's already open.");
            else return CheckRule.Allow();
        }

        CheckRule OpenableRules.CheckClose(Actor Actor)
        {
            if (Open) return CheckRule.Allow();
            else return CheckRule.Disallow("It's already closed.");
        }

        protected RuleHandlerFollowUp ImplementHandleOpen(Actor Actor)
        { 
            Open = true;

            var location = Actor.Location as Room;
            var otherSide = this.OppositeSide(location);
            if (otherSide != null)
            {
                Mud.SendLocaleMessage(otherSide as Room, "<a0> opens <the1>.", Actor, this);
                Mud.MarkLocaleForUpdate(otherSide);
            }

            return RuleHandlerFollowUp.Continue;
        }

        RuleHandlerFollowUp OpenableRules.HandleOpen(Actor Actor)
        {
            return ImplementHandleOpen(Actor);
        }

        protected RuleHandlerFollowUp ImplementHandleClose(Actor Actor)
        {
            Open = false;

            var location = Actor.Location as Room;
            var otherSide = this.OppositeSide(location);
            if (otherSide != null)
            {
                Mud.SendLocaleMessage(otherSide, "<a0> closes <a1>.", Actor, this);
                Mud.MarkLocaleForUpdate(otherSide);
            }

            return RuleHandlerFollowUp.Continue;
        }


        RuleHandlerFollowUp OpenableRules.HandleClose(Actor Actor)
        {
            return ImplementHandleClose(Actor);
        }

        #endregion
    }
}
