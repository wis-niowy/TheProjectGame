﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameArea
{
    public class Utf8Writer : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
