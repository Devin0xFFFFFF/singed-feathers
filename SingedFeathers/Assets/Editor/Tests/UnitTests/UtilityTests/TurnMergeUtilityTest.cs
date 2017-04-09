using System.Collections.Generic;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Utility;
using NUnit.Framework;

namespace Assets.Editor.Tests.UnitTests.UtilityTests {
    [TestFixture]
    public class TurnMergeUtilityTest {
        private List<Delta> _deltas0;

        [SetUp]
        public void Init() {
            Position position0 = new Position(0, 0);
            Position position1 = new Position(1, 1);
            Command commandFire = new Command(MoveType.Fire, 1);
            Command commandWater = new Command(MoveType.Water, 1);
            Delta delta0 = new Delta(position0, commandFire);
            Delta delta1 = new Delta(position0, commandWater);
            Delta delta2 = new Delta(position1, commandWater);
            Delta delta3 = new Delta(position1, commandFire);
            _deltas0 = new List<Delta> { delta0, delta1, delta2, delta3 };
        }

        [Test]
        public void TestCorrectMergeSize() {
            IList<Delta> mergedList = TurnMergeUtility.SortDeltas(_deltas0);
            Assert.True(mergedList.Count == 4);
        }

        [Test]
        public void TestCorrectMergeSizeOrder() {
            IList<Delta> mergedList = TurnMergeUtility.SortDeltas(_deltas0);
            Assert.True(mergedList[0].Command.MoveType == MoveType.Fire);
            Assert.True(mergedList[1].Command.MoveType == MoveType.Fire);
            Assert.True(mergedList[2].Command.MoveType == MoveType.Water);
            Assert.True(mergedList[3].Command.MoveType == MoveType.Water);
        }
    }
}