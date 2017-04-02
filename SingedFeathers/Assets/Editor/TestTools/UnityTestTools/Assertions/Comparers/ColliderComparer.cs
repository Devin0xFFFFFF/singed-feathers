using System;
using UnityEngine;

namespace Assets.Editor.TestTools.UnityTestTools.Assertions.Comparers
{
    public class ColliderComparer : ComparerBaseGeneric<Bounds>
    {
        public enum CompareType
        {
            Intersects,
            DoesNotIntersect
        };

        public CompareType compareType;

        protected override bool Compare(Bounds a, Bounds b)
        {
            switch (compareType)
            {
                case CompareType.Intersects:
                    return a.Intersects(b);
                case CompareType.DoesNotIntersect:
                    return !a.Intersects(b);
            }
            throw new Exception();
        }
    }
}
