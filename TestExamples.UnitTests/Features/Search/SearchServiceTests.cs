using Examine;
using FluentAssertions;
using NSubstitute;
using TestingExamples.Web.Features.Search;
using TestingExamples.Web.Features.Search.Models;
using Umbraco.Cms.Core.Web;

namespace TestExamples.UnitTests.Features.Search;

[TestFixture]
public class SearchServiceTests
{
    [TestCase(-1, TestName = "Search Throws when PageSize is less than 0")]
    [TestCase(0, TestName = "Search Throws when PageSize is 0")]
    public void Ensure_PageSize_BelowMinRange_Throws(int pageSize)
    {
        var service = new SearchService(Substitute.For<IExamineManager>(), Substitute.For<IUmbracoContextAccessor>());


        service.Invoking(s => s.Search(new SearchRequest
        {
            PageSize = pageSize
        }, 1, 1))
            .Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(SearchRequest.PageSize));
    }

    [TestCase(1)]
    [TestCase(10)]
    public void Ensure_PageSize_MinRangeAndAbove_DoesNotThrow(int pageSize)
    {
        var service = new SearchService(Substitute.For<IExamineManager>(), Substitute.For<IUmbracoContextAccessor>());


        service.Invoking(s => s.Search(new SearchRequest
        {
            PageSize = pageSize
        }, 1, 1))
            .Should().NotThrow<ArgumentOutOfRangeException>();
    }
}