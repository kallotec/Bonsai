using Bonsai.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.UI.Widgets
{
    public class VariableTextWidget<T> : TextWidget<T>
    {
        public VariableTextWidget(GameVariable<T> variable, T defaultValue, WidgetSettings settings)
            : base(defaultValue, settings)
        {
            // Validation
            if (variable == null)
                throw new ArgumentNullException("variable");

            this.variable = variable;
        }

        GameVariable<T> variable;


        public override void Load(IContentLoader loader)
        {
            if (variable != null)
            {
                // Initialize the value field
                handleVariableChanged(variable.Value);
                // Subscribe to changes
                variable.Changed += handleVariableChanged;
            }
        }

        public override void Unload()
        {
            if (variable != null)
                variable.Changed -= handleVariableChanged;
        }

        void handleVariableChanged(T newValue)
        {
            // Update text field with new value
            base.UpdateValue(newValue);
        }

    }
}
