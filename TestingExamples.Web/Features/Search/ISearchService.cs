using Examine;
using TestingExamples.Web.Features.Search.Models;

namespace TestingExamples.Web.Features.Search;

public interface ISearchService
{
    SearchResponse Search(SearchRequest searchRequest, int id, int currentPageId);
}