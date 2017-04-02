using System;
using UnityEngine;

namespace Assets.Editor.TestTools.UnityTestTools.Assertions.Comparers
{
    public class TransformComparer : ComparerBaseGeneric<Transform>
    {
        public enum CompareType { Equals, NotEquals }

        public CompareType compareType;

        protected override bool Compare(Transform a, Transform b)
        {
            if (compareType == CompareType.Equals)
            {
                return a.position == b.position;
            }
            if (compareType == CompareType.NotEquals)
            {
                return a.position != b.position;
            }
            throw new Exception();
        }
    }
}
