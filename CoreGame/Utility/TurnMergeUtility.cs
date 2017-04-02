using CoreGame.Models;
using System.Collections.Generic;
using System.Linq;

namespace CoreGame.Utility {
    public class TurnMergeUtility {
        private TurnMergeUtility() { }

        public static List<Delta> SortDeltas(List<Delta> deltas) { return deltas.OrderBy(delta => delta.Command).ToList(); }
    }
}
