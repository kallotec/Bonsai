using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.ContentLoading
{
    public class SimpleContentLoader : IContentLoader
    {
        public SimpleContentLoader(ContentManager contentManager)
        {
            this.cache = new Dictionary<string, object>();
            this.contentManager = contentManager;
            this.contentManager.RootDirectory = "Content";
        }

        Dictionary<string, object> cache;
        ContentManager contentManager;


        public T Load<T>(string path, bool ignoreCache = false)
        {
            if (ignoreCache)
                return contentManager.Load<T>(path);

            // Return cache instance
            if (cache.ContainsKey(path))
                return (T)cache[path];

            // Load resource
            var instance = contentManager.Load<T>(path);

            // Add to cache
            cache.Add(path, instance);

            return instance;
        }

        public void Cleanup()
        {
            this.contentManager.Unload();
        }

    }
}
