﻿using System.Collections;
using React;

namespace Test1
{
    public class SubTrackedResource: TrackedResource
    {
        public bool IsReady { get; set; } = false;
        public bool AllReady { get; set; } = false;

        public SubTrackedResource(IEnumerable items) : base(items)
        {
        }

        public SubTrackedResource(string name, IEnumerable items) : base(name, items)
        {
        }
    }
}