class wolf : NPC
{
    public bool IsFed = false;

    public override void Initialize()
    {
        Perform<MudObject, MudObject, MudObject>("topic response")
            .Do((actor, npc, topic) =>
            {
                MudObject.SendLocaleMessage(actor, "The wolf snarls and howls, showing its large sharp teeth.");
                return PerformResult.Stop;
            });

        Short = "wolf";
        Long = "This is a snarling grey beast with a long snout and upright ears. It has a tail that does not wag even a little.";

        Nouns.Add("wolf");

        Perform<MudObject, MudObject>("handle-entrail-drop").Do((wolf, entrails) =>
            {
                MudObject.SendLocaleMessage(this, "The wolf snatches up the entrails.");
                IsFed = true;
                MudObject.Move(entrails, MudObject.GetObject("palantine/soranus"), RelativeLocations.Worn);
                return PerformResult.Stop;
            });

        Value<MudObject, MudObject, string, string>("printed name").First.Do((viewer, item, article) => article + " wolf");

        Value<MudObject, bool>("entrail-quest-is-fed").Do(wolf => IsFed);

        Perform<MudObject, MudObject>("quest reset")
            .When((quest, item) => quest.Path == "palantine/entrail_quest")
            .Do((quest, item) => { IsFed = false; return PerformResult.Stop; });

        Core.GlobalRules.Perform("heartbeat").Do(() =>
        {
            if (!IsFed)
                MudObject.SendLocaleMessage(this, "The wolf whines for food.");
            return PerformResult.Continue;
        });
    }

}