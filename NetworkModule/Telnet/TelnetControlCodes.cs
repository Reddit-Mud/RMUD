using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModule.Telnet
{
    // http://www.faqs.org/rfcs/rfc854.html
    public enum TelnetControlCodes : byte
    {
        NULL = 0,
        Echo = 1,   // http://www.faqs.org/rfcs/rfc857.html
        LineFeed = 10,
        CarriageReturn = 13,

        Bell = 7,
        BackSpace = 8,
        HorizontalTab = 9,
        VerticalTab = 11,
        FormFeed = 12,

        MCCPCompress = 86,

        EndSubParameters = 240,
        Nop = 241,
        DataMark = 242,
        Break = 243,
        SuspendInterrupt = 244,
        AbortOutput = 245,
        AreYouThere = 246,
        EraseCharacter = 247,
        EraseLine = 248,
        GoAhead = 249,
        SubParameters = 250,

        Will = 251,
        Wont = 252,
        Do = 253,
        Dont = 254,
        IAC = 255
    }
}
