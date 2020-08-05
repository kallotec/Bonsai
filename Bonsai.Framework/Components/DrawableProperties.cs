using System;
using System.Collections.Generic;
using System.Text;

namespace Bonsai.Framework.Components
{
    public class DrawableProperties
    {
        bool IsHidden { get; set; }
        DrawOrderPosition DrawOrder { get; }
        bool IsAttachedToCamera { get; }
    }
}
