using NUnit.Framework;
using CoreGame.Models;
using System.Collections.Generic;

namespace Assets.Editor.ControllerTests {
    [TestFixture]
    public class PositionTest {

        [Test]
        public void TestSort() {
            Position pos0 = new Position(0, 0);
            Position pos1 = new Position(1, 0);
            Position pos2 = new Position(0, 1);
            Position pos3 = new Position(1, 1);
            Position pos4 = new Position(5, 0);
            Position pos5 = new Position(0, 5);
            Position pos6 = new Position(5, 5);
            Position pos7 = new Position(10, 0);
            Position pos8 = new Position(0, 10);
            Position pos9 = new Position(10, 10);
            List<Position> positionList = new List<Position> { pos0, pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8, pos9 };

            positionList.Sort();

            Assert.AreEqual(positionList[0], pos0);
            Assert.AreEqual(positionList[1], pos2);
            Assert.AreEqual(positionList[2], pos5);
            Assert.AreEqual(positionList[3], pos8);
            Assert.AreEqual(positionList[4], pos1);
            Assert.AreEqual(positionList[5], pos3);
            Assert.AreEqual(positionList[6], pos4);
            Assert.AreEqual(positionList[7], pos6);
            Assert.AreEqual(positionList[8], pos7);
            Assert.AreEqual(positionList[9], pos9);
        }

        [Test]
        public void TestGetLargestDistanceFrom() {
            Position pos0 = new Position(0, 0);
            Position pos1 = new Position(1, 0);
            Position pos2 = new Position(0, 1);
            Position pos3 = new Position(1, 1);
            Position pos4 = new Position(1, 2);
            Position pos5 = new Position(2, 1);
            Position pos6 = new Position(2, 2);

            Assert.True(pos0.GetLargestDistanceFrom(pos0) == 0);
            Assert.True(pos0.GetLargestDistanceFrom(pos1) == 1);
            Assert.True(pos0.GetLargestDistanceFrom(pos2) == 1);
            Assert.True(pos0.GetLargestDistanceFrom(pos3) == 1);
            Assert.True(pos0.GetLargestDistanceFrom(pos4) == 2);
            Assert.True(pos0.GetLargestDistanceFrom(pos5) == 2);
            Assert.True(pos0.GetLargestDistanceFrom(pos6) == 2);

            Assert.True(pos1.GetLargestDistanceFrom(pos0) == 1);
            Assert.True(pos1.GetLargestDistanceFrom(pos1) == 0);
            Assert.True(pos1.GetLargestDistanceFrom(pos2) == 1);
            Assert.True(pos1.GetLargestDistanceFrom(pos3) == 1);
            Assert.True(pos1.GetLargestDistanceFrom(pos4) == 2);
            Assert.True(pos1.GetLargestDistanceFrom(pos5) == 1);
            Assert.True(pos1.GetLargestDistanceFrom(pos6) == 2);
        }

    }
}