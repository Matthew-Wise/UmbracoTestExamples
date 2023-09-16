using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Extensions;
using TestingExamples.Web.Features.Search;
using TestingExamples.Web.Features.Search.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common.Routing;

namespace TestExamples.UnitTests.Features.Search;

[TestFixture]
public class SearchPageControllerUnitTests
{
    /// <summary>
    /// TODO: Clean this up using AutoFixture/builder pattern? 
    /// </summary>
    /// <param name="searchService"></param>
    /// <returns></returns>
    private static SearchPageController SetUpSearchPageController(ISearchService searchService)
    {
        var publishedRequest = Substitute.For<IPublishedRequest>();
        publishedRequest.Configure().PublishedContent.Returns(
            Substitute.For<SearchPage>(Substitute.For<IPublishedContent>(),
            Substitute.For<IPublishedValueFallback>()
            ));

        var umbracoRouteValues = Substitute.For<UmbracoRouteValues>(publishedRequest,
            Substitute.For<ControllerActionDescriptor>(),
            "SearchPage");

        var featureCollection = new FeatureCollection();
        featureCollection.Set(umbracoRouteValues);

        var viewEngine = Substitute.For<ICompositeViewEngine>();
        viewEngine.Configure().FindView(Arg.Any<ActionContext>(), Arg.Any<string>(), Arg.Any<bool>())
            .ReturnsForAnyArgs(x => ViewEngineResult.Found(x.ArgAt<string>(1), Substitute.For<IView>()));

        var controller = new SearchPageController(
            Substitute.For<ILogger<SearchPageController>>(),
            viewEngine,
            Substitute.For<IUmbracoContextAccessor>(),
            searchService)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(featureCollection)
            }
        };
        return controller;
    }

    [Test, AutoNSubstituteData]
    public void SearchPageController_ReturnsExpected_ViewModel([Substitute] IPublishedContent item, string searchTerm)
    {
        var searchService = Substitute.For<ISearchService>();
        searchService.Configure()
            .Search(Arg.Any<SearchRequest>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new SearchResponse
            {
                SearchTerm = searchTerm,
                TotalItemCount = 1,
                Items = new[] { item }
            });

        var controller = SetUpSearchPageController(searchService);
        var result = controller.Index(Substitute.For<SearchRequest>());

        result.Should().BeOfType<ViewResult>();
        var model = ((ViewResult)result).Model as SearchPageViewModel;
        model.Should().BeEquivalentTo(new
        {
            SearchResponse = new
            {
                SearchTerm = searchTerm,
                TotalItemCount = 1,
                Items = new[] { item }
            }
        });
    }
}