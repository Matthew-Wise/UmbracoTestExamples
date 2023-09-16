using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using TestingExamples.Web.Features.Search.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExamples.Web.Features.Search;

public class SearchPageController : RenderController
{
    private readonly ISearchService _searchService;

    public SearchPageController(ILogger<SearchPageController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, ISearchService searchService) : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
        _searchService = searchService;
    }

    [NonAction]
    public override IActionResult Index() => throw new InvalidOperationException();

    public IActionResult Index([FromQuery] SearchRequest searchRequest)
    {
        if (CurrentPage is not SearchPage searchPage)
        {
            return NotFound();
        }

        var response = _searchService.Search(searchRequest, searchPage.Root().Id, searchPage.Id);
        return CurrentTemplate(new SearchPageViewModel(searchPage)
        {
            SearchResponse = response
        });
    }
}