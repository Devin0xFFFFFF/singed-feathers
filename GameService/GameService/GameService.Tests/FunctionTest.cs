using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.TestUtilities;

using CoreGame.Models.API.GameService;

namespace GameService.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToUpperFunction()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var output = function.CommitTurn(new CommitTurnRequest("Test", "Player1", null));

            Assert.Equal(true, output);
        }
    }
}
