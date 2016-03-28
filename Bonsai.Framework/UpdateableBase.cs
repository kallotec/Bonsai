using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface IUpdateable
    {
        bool IsDisabled { get; }

        void Update(GameTime time);

    }
}
