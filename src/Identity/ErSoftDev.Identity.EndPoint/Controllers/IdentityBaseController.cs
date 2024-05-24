using ErSoftDev.Framework.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ErSoftDev.Identity.EndPoint.Controllers
{
    [Route("Identity/api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiResultFilter]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class IdentityBaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        public IdentityBaseController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
