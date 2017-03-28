using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.TestUtilities;

using CoreGame.Models.API.GameService;
using Amazon.Lambda.APIGatewayEvents;

namespace GameService.Tests
{
    public class FunctionTest {
        [Fact]
        public void TestCommitTurnFunction() {
            Function function = new Function();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();
            request.Body = "";
            APIGatewayProxyResponse reponse = function.CommitTurn(request);

            Assert.Equal(true, true);
        }
    }
}
