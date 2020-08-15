using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Skavenger.Game
{
    public static class Events
    {
        public const string BackToStartScreen = nameof(BackToStartScreen);
        public const string PlayerDied = nameof(PlayerDied);
        public const string CreateProjectile = nameof(CreateProjectile);
        public const string PlayerPickedUpCoin = nameof(PlayerPickedUpCoin);
    }
}
