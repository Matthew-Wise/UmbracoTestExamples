using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExamples.Web.Features.Search.Models;

public class SearchPageViewModel : ContentModel<SearchPage>
{
	public SearchPageViewModel(SearchPage content) : base(content)
	{
	}

	public required SearchResponse SearchResponse { get; set; }
}