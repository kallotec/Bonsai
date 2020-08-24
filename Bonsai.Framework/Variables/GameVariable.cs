using Bonsai.Framework.ContentLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Variables
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
                var isChanged = ((object)value != (object)this.value);

                this.value = value;

                // Notify subscribers of change
                if (isChanged)
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
