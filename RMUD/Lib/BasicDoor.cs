using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class BasicDoor : Portal
    {
        public BasicDoor()
        {
            this.Nouns.Add("DOOR");
            this.Nouns.Add("CLOSED", actor => !Open);
            this.Nouns.Add("OPEN", actor => Open);
            Open = false;

            AddValueRule<MudObject, bool>("openable").Do(a => true);
            AddValueRule<MudObject, bool>("is-open").Do(a => Open);

            AddCheckRule<MudObject, MudObject>("can-open").Do((a, b) => 
                {
                    if (Open)
                    {
                        Mud.SendMessage(a, "It is already open.");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                });

            AddCheckRule<MudObject, MudObject>("can-close").Do((a, b) =>
            {
                if (!Open)
                {
                    Mud.SendMessage(a, "It is already closed.");
                    return CheckResult.Disallow;
                }
                return CheckResult.Allow;
            });
            
            AddPerformRule<MudObject, MudObject>("on-opened").Do((a, b) =>
            {
                Open = true;

                var location = a.Location as Room;
                var otherSide = this.OppositeSide(location);
                if (otherSide != null)
                {
                    Mud.SendLocaleMessage(otherSide as Room, "<a0> opens <the1>.", a, this);
                    Mud.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            });

            AddPerformRule<MudObject, MudObject>("on-closed").Do((a, b) =>
            {
                Open = false;

                var location = a.Location as Room;
                var otherSide = this.OppositeSide(location);
                if (otherSide != null)
                {
                    Mud.SendLocaleMessage(otherSide as Room, "<a0> closes <the1>.", a, this);
                    Mud.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            });
        }

        public bool Open { get; set; }

    }
}
