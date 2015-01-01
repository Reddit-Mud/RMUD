﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Portal : MudObject
    {
        public MudObject FrontSide;
        public MudObject BackSide;

        public MudObject OppositeSide(MudObject Side)
        {
            if (Object.ReferenceEquals(Side, FrontSide)) return BackSide;
            return FrontSide;
        }

        public void AddSide(MudObject Side)
        {
            if (FrontSide == null || FrontSide.State == ObjectState.Destroyed || Object.ReferenceEquals(FrontSide, Side)) FrontSide = Side;
            else if (BackSide == null || BackSide.State == ObjectState.Destroyed || Object.ReferenceEquals(BackSide, Side)) BackSide = Side;
            else throw new InvalidOperationException();
        }
    }
}