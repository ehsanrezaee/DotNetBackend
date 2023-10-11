using MediatR;
using ErSoftDev.Common.Utilities;
using ErSoftDev.Framework.Log;

namespace ErSoftDev.Framework.Behaviors
{
    public class
        LoggingBehavior<TRequest,
            TResponse> : IPipelineBehavior<TRequest, TResponse> //where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling command requests CommandName : {0} , Request : {1}",
                request.GetGenericTypeName(), request);

            var response = await next();

            _logger.LogInformation("Command handled - commandName : {0} , Request : {1} , Response : {2}",
                request.GetGenericTypeName(),
                request, response);

            return response;
        }

    }
}
