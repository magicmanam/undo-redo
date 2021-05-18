using System.Collections.Generic;
using System.Linq;

namespace magicmanam.UndoRedo.Tests
{
    class StatefulComponent : IStatefulComponent<List<int>>
    {
        public int A { get; set; }

        public List<int> UndoableState
        {
            get {
                return new List<int> { this.A };
            }
            set
            {
                this.A = value[0];
            }
        }
    }
}
