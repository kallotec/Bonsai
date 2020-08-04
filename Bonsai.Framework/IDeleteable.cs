using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface IDeletable
    {
        bool DeleteMe { get; }
    }
}
