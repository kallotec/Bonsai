using Bonsai.Framework.Content;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface ILoadable
    {
        void Load(IContentLoader loader);
        void Unload();
    }
}
