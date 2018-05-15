using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
