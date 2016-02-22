using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static class RegisterActorProperties
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            PropertyManifest.RegisterProperty("actor?", typeof(bool), false);
            PropertyManifest.RegisterProperty("preserve?", typeof(bool), false);
            PropertyManifest.RegisterProperty("gender", typeof(Gender), Gender.Male);
            PropertyManifest.RegisterProperty("rank", typeof(int), 0);
            PropertyManifest.RegisterProperty("client", typeof(Client), null);
            PropertyManifest.RegisterProperty("command handler", typeof(ClientCommandHandler), null);
        }
    }

    public partial class MudObject
    {
        public void Actor()
        {
            Container(RelativeLocations.Held | RelativeLocations.Worn, RelativeLocations.Held);

            SetProperty("actor?", true);
            SetProperty("preserve?", true);

            GetProperty<NounList>("nouns").Add("MAN", (a) => a.GetProperty<Gender>("gender") == RMUD.Gender.Male);
            GetProperty<NounList>("nouns").Add("WOMAN", (a) => a.GetProperty<Gender>("gender") == RMUD.Gender.Female);
        }

    }
}
