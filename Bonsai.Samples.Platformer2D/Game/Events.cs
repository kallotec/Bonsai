using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    public static class Events
    {
        public const string BackToStartScreen = nameof(BackToStartScreen);
        public const string PlayerPickedUpCoin = nameof(PlayerPickedUpCoin);
        public const string PlayerJumped = nameof(PlayerJumped);
        public const string PlayerEnteredDoor = nameof(PlayerEnteredDoor);
        public const string PlayerDied = nameof(PlayerDied);
        public const string CreateProjectile = nameof(CreateProjectile);
    }
}
