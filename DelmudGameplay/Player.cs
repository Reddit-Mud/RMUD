using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace DelmudGameplay
{
	public class Player : RMUD.Player
	{
        [Persist]
        public ElementTypes Element { get; set; }

        [Persist]
        public CharacterClasses Class { get; set; }

        [Persist]
        public int Attack { get; set; }

        [Persist]
        public int Defense { get; set; }

        [Persist]
        public int MaxHP { get; set; }

        [Persist]
        public int MaxMP { get; set; }

    }
}
