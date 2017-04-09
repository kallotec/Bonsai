using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Actors
{
    public abstract class Actor
    {
        public bool DeleteMe;
        public PhysicalProperties Props = new PhysicalProperties();
    }
}
