using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.Contract.Category;
using Catalog.DataAccess.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OpenTracing;

namespace Catalog.Grpc.Services;

/// <inheritdoc />
public class CategoryService : Contract.Category.CategoryService.CategoryServiceBase
{
    // private readonly ILogger<CategoryService> _logger;
    private readonly ICachedBaseRepository<Category> _repository;
    private readonly ITracer _tracer;

    /// <inheritdoc />
    public CategoryService(/*ILogger<CategoryService> logger, */ICachedBaseRepository<Category> repository, ITracer tracer)
    {
        // _logger = logger;
        _repository = repository;
        _tracer = tracer;
    }

    /// <inheritdoc />
    public override async Task<GetAllResponse> GetAll(Empty request, ServerCallContext context)
    {
        var span = _tracer.BuildSpan("category.get-all").AsChildOf(_tracer.ActiveSpan).Start();
        var hashGetSpan = _tracer.BuildSpan("hash-get").AsChildOf(span).Start();
        var items = await _repository.GetHashAsync(token: context.CancellationToken);
        hashGetSpan.Finish();

        var conversionSpan = _tracer.BuildSpan("conversion").AsChildOf(span).Start();
        var result = items.Select(x => new CategoryResponseItem
        {
            Id = x.Id.ToString(),
            Title = x.Title,
            Order = x.Order,
            SubCategories = {x.SubCategories.Select(s => s.ToString())}
        });
        var response = new GetAllResponse {Items = {result}};
        conversionSpan.Finish();
        span.Finish();
        return response;
    }
}