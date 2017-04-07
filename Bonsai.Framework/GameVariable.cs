using Bonsai.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public class GameVariable<T> : ILoadable
    {
        public GameVariable()
        {
        }

        T value;

        public T Value
        {
            get { return this.value; }
            set
            {
                this.value = value;

                // Notify subscribers of change
                Changed?.Invoke(this.value);
            }
        }

        public event GameVariableChangedHandler<T> Changed;


        public void Load(IContentLoader loader)
        {
            // Dont need
        }

        public void Unload()
        {
            // Remove handlers
            Changed = null;
        }


    }
}
