using System;

namespace TextFormatter.Core.Models
{
    [Flags]
    public enum LetterCase
    {
        Upper = 1 << 0,
        Lower = 1 << 1
    }
}
