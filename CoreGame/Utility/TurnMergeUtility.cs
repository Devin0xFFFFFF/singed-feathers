using CoreGame.Models;
using System.Collections.Generic;
using System.Linq;

namespace CoreGame.Utility {
    public class TurnMergeUtility {
        private TurnMergeUtility() { }

        public static IList<Delta> MergeDeltas(List<Delta> deltasOne, List<Delta> deltasTwo) {
            return deltasOne.Concat(deltasTwo).OrderBy(delta => delta.Command).ToList();
        }
    }
}
