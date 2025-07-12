using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Test.Service
{
    public class MockHttpMessageHandlerException : HttpMessageHandler
    {
        private readonly Exception _exception;

        public MockHttpMessageHandlerException(Exception exception)
        {
            _exception = exception;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw _exception;
        }
    }
}
