using TestingExamples.Web.Features.Search;
using Umbraco.Cms.Core.Composing;

namespace TestingExamples.Web.Features;

public class FeatureComposer : IComposer
{
	public void Compose(IUmbracoBuilder builder)
	{
		builder.Services.AddScoped<ISearchService, SearchService>();
	}
}