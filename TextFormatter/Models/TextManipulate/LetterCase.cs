using System;

namespace TextFormatter.Models.TextManipulate
{
    [Flags]
    public enum LetterCase
    {
        Upper = 1 << 0,
        Lower = 1 << 1
    }
}
