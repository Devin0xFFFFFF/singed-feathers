using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;

namespace Assests.Scripts.Utility {
    public class TurnMergeUtility {
        private TurnMergeUtility() { }

        public static IList<Delta> MergeDeltas(List<Delta> deltasOne, List<Delta> deltasTwo) {
            return deltasOne.Concat(deltasTwo).OrderBy(delta => delta.Command).ToList();
        }
    }
}
