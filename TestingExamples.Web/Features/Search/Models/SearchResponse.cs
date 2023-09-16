using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExamples.Web.Features.Search.Models;

public class SearchResponse
{
	public long TotalItemCount { get; set; }
	
	public string? SearchTerm { get; set; }
	
	public int PageSize { get; set; }

	public IEnumerable<IPublishedContent?> Items { get; set; } = Enumerable.Empty<IPublishedContent?>();
	
	public int Page { get; set; }
}