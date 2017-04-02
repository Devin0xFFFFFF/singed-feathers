using System;

namespace Assets.Editor.TestTools.UnityTestTools.Assertions
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException(string path)
            : base("Invalid path part " + path)
        {
        }
    }
}
