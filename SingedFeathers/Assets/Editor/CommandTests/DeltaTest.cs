using Assets.Scripts.Controllers;
using NUnit.Framework;
using NSubstitute;
using Assets.Scripts.Models.Commands;
using Assets.Scripts.Models;
using System.Collections.Generic;

namespace Assets.Editor.CommandTests {
    [TestFixture]
    public class DeltaTest {
        private Delta _deltaFire0;
        private Delta _deltaFire1;
        private Delta _deltaWater0;
        private Delta _deltaWater1;

        [SetUp]
        public void Init() {
            _deltaFire0 = new Delta(new Position(0, 0), new Command(MoveType.Fire, 1));
            _deltaFire1 = new Delta(new Position(0, 0), new Command(MoveType.Fire, 1));
            _deltaWater0 = new Delta(new Position(0, 0), new Command(MoveType.Water, 1));
            _deltaWater1 = new Delta(new Position(0, 0), new Command(MoveType.Water, 1));
        }

        [Test]
        public void TestCompare() {
            Assert.True(_deltaFire0.CompareTo(_deltaFire1) == 0);
            Assert.True(_deltaFire1.CompareTo(_deltaFire0) == 0);
            Assert.True(_deltaWater0.CompareTo(_deltaWater1) == 0);
            Assert.True(_deltaWater1.CompareTo(_deltaWater0) == 0);
            Assert.True(_deltaFire0.CompareTo(_deltaWater0) == -1);
            Assert.True(_deltaWater0.CompareTo(_deltaFire0) == 1);
        }

        [Test]
        public void TestSort() {
            List<Delta> list = new List<Delta> { _deltaFire0, _deltaWater1, _deltaWater0, _deltaFire1 };

            Assert.True(list[0].Command.MoveType == MoveType.Fire);
            Assert.True(list[1].Command.MoveType == MoveType.Water);
            Assert.True(list[2].Command.MoveType == MoveType.Water);
            Assert.True(list[3].Command.MoveType == MoveType.Fire);

            list.Sort();

            Assert.True(list[0].Command.MoveType == MoveType.Fire);
            Assert.True(list[1].Command.MoveType == MoveType.Fire);
            Assert.True(list[2].Command.MoveType == MoveType.Water);
            Assert.True(list[3].Command.MoveType == MoveType.Water);
        }
    }
}