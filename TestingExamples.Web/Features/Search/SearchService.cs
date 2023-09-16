using Examine;
using Examine.Search;
using TestingExamples.Web.Features.Search.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;

namespace TestingExamples.Web.Features.Search;

public class SearchService : ISearchService
{
	private readonly IExamineManager _examineManager;

	private readonly IUmbracoContextAccessor _umbracoContextAccessor;

	public SearchService(IExamineManager examineManager, IUmbracoContextAccessor umbracoContextAccessor)
	{
		_examineManager = examineManager;
		_umbracoContextAccessor = umbracoContextAccessor;
	}

	public SearchResponse Search(SearchRequest searchRequest, int parentId, int currentPageId)
	{
		if (searchRequest.PageSize < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(searchRequest.PageSize), "PageSize must be at least 1");
		}

		if (_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out var index) == false)
		{
			return new SearchResponse() {  SearchTerm = searchRequest.SearchTerm};
		}

		var query = index.Searcher.CreateQuery(IndexTypes.Content)
			.ParentId(parentId)
			.AndNot(nq => nq.Field("id", currentPageId.ToInvariantString()));
		if (!string.IsNullOrWhiteSpace(searchRequest.SearchTerm))
		{
			query.And().NodeName(searchRequest.SearchTerm.Fuzzy());
		}

		var page = searchRequest.Page < 1 ? 1 : searchRequest.Page;
		var skip = (page - 1) * searchRequest.PageSize;
		var response = query.Execute(new QueryOptions(skip, searchRequest.PageSize));

		if (response == null)
        {
            return new SearchResponse() { SearchTerm = searchRequest.SearchTerm };
        }

		var context = _umbracoContextAccessor.GetRequiredUmbracoContext();
		var items = response.Select(r => int.TryParse(r.Id, out var pageId) ? context.Content?.GetById(pageId) : null);
		return new SearchResponse
		{
			TotalItemCount = response?.TotalItemCount ?? 0,
			SearchTerm = searchRequest.SearchTerm,
			PageSize = searchRequest.PageSize,
			Items = items,
			Page = page
		};
	}
}