using Bonsai.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface IUpdateable
    {
        void Update(GameFrame frame);
    }
}
