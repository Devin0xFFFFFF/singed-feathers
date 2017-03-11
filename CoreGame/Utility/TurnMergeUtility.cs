using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using System.Collections.Generic;

namespace Assests.Scripts.Utility {
    public class TurnMergeUtility {
        private TurnMergeUtility() { }

        public static List<Delta> MergeDeltas(List<Delta> deltasOne, List<Delta> deltasTwo) {
            List<Delta> combinedDeltas = new List<Delta>();
            combinedDeltas.AddRange(deltasOne);
            combinedDeltas.AddRange(deltasTwo);
            combinedDeltas.Sort();
            return combinedDeltas;
        }
    }
}
