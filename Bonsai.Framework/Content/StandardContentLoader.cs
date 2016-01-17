using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Content
{
    public class StandardContentLoader : IContentLoader
    {
        public StandardContentLoader(ContentManager manager)
        {
            this.cache = new Dictionary<string, object>();
            this.manager = manager;
        }

        Dictionary<string, object> cache;
        ContentManager manager;


        public T Load<T>(string path, bool ignoreCache = false)
        {
            if (ignoreCache)
                return manager.Load<T>(path);

            // Return cache instance
            if (cache.ContainsKey(path))
                return (T)cache[path];

            // Load resource
            var instance = manager.Load<T>(path);

            // Add to cache
            cache.Add(path, instance);

            return instance;
        }

    }
}
