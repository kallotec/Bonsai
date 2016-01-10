using Bonsai.Framework.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface IGame
    {
        ILevel Level { get; }
    }
}
