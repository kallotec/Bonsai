using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Content
{
    public interface IContentLoader
    {
        T Load<T>(string path, bool ignoreCache = false);
    }
}
