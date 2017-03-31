using System;
using UnityEngine;

namespace Assets.Editor.TestTools.UnityTestTools.Assertions.Comparers
{
    public class Vector3Comparer : VectorComparerBase<Vector3>
    {
        public enum CompareType
        {
            MagnitudeEquals,
            MagnitudeNotEquals
        }

        public CompareType compareType;
        public double floatingPointError = 0.0001f;

        protected override bool Compare(Vector3 a, Vector3 b)
        {
            switch (compareType)
            {
                case CompareType.MagnitudeEquals:
                    return AreVectorMagnitudeEqual(a.magnitude,
                                                   b.magnitude, floatingPointError);
                case CompareType.MagnitudeNotEquals:
                    return !AreVectorMagnitudeEqual(a.magnitude,
                                                    b.magnitude, floatingPointError);
            }
            throw new Exception();
        }
    }
}
