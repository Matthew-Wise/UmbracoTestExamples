using System.Net;
using DiffEngine;
using FluentAssertions;
using TestExamples.IntegrationTests.Application;

namespace TestExamples.IntegrationTests.Features.Search;


[TestFixture]
public class SearchPageControllerIntegrationTests : IntegrationTestBase
{
	
	[TestCase(TestName = "Search returns 200 OK")]
    public async Task GetRootPage_HappyFlow_ReturnsOK()
	{
		DiffTools.UseOrder(DiffTool.VisualStudio);

		var response = await Client.GetAsync("/search/?searchTerm=content");

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		await Verify(await response.Content.ReadAsStringAsync());
	}
}