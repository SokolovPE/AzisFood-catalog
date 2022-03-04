using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.ProtoServices;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace Catalog.Services;

/// <inheritdoc />
public class CategoryService : Catalog.ProtoServices.CategoryService.CategoryServiceBase
{
    private readonly ICachedBaseRepository<Category> _repository;
    private readonly ILogger _logger;
    private readonly ITracer _tracer;

    /// <inheritdoc />
    public CategoryService(ILogger<CategoryService> logger, ICachedBaseRepository<Category> repository, ITracer tracer)
    {
        _logger = logger;
        _repository = repository;
        _tracer = tracer;
    }

    /// <inheritdoc />
    public override async Task<CategoryAllResponse> GetAll(Empty request, ServerCallContext context)
    {
        var span = _tracer.BuildSpan("grpc-category.get-all").AsChildOf(_tracer.ActiveSpan).Start();
        var hashGetSpan = _tracer.BuildSpan("hash-get").AsChildOf(span).Start();
        var items = await _repository.GetHashAsync(CancellationToken.None);
        hashGetSpan.Finish();
        var conversionSpan = _tracer.BuildSpan("conversion").AsChildOf(span).Start();
        var result = items.Select(x => new CategoryAllResponseItem()
        {
            Id = x.Id.ToString(),
            Title = x.Title,
            Order = x.Order,
            SubCategories = {x.SubCategories.Select(s => s.ToString())}
        });
        var response = new CategoryAllResponse() {Items = {result}};
        conversionSpan.Finish();
        span.Finish();
        return response;
    }
}