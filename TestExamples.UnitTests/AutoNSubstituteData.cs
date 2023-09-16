using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;

namespace TestExamples.UnitTests
{
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute() :
            base(() => new Fixture().Customize(new AutoNSubstituteCustomization()))
        {
        }
    }
}
