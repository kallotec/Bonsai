﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Variables
{
    public delegate void GameVariableChangedHandler<T>(T newValue);
}
