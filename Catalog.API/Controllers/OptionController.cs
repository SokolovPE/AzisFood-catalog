using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    /// <summary>
    /// Controller to operate options
    /// </summary>
    public class OptionController : BaseController<Option, OptionDto, OptionRequestDto>
    {
        /// <inheritdoc />
        public OptionController(ILogger<OptionController> controllerLogger,
            IService<Option, OptionDto, OptionRequestDto> service) : base(controllerLogger,
            service)
        {
        }
    }
}