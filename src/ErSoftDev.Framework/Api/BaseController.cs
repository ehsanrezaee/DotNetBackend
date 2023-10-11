using ErSoftDev.Framework.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ErSoftDev.Framework.Api
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiResultFilter]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
