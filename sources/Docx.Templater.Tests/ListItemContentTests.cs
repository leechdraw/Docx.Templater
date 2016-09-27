using Docx.Templater.TemplateCustomContent;
using FluentAssertions;
using NUnit.Framework;

namespace Docx.Templater.Tests
{
    [TestFixture]
    public class ListItemContentTests
    {
        private const string SomeName = "SomeName";
        private const string SomeValue = "Some Value";
        private const string SomeName2 = "Some  Name2";
        private const string SomeValue2 = "Some Value_2";

        private const string SomeValue3 = "Some Value_2_3";

        /// <summary>
        /// если используется конструктор для имени и значения, должне создавать один элемент в
        /// коллекции полей, коллекция вложенных полей должна быть пустой
        /// </summary>
        [Test]
        public void ShouldCreateSingleItemByNameValueInConstructor()
        {
            //Given
            //When
            var listItemContent = new ListItemContent(SomeName, SomeValue);

            //Then
            listItemContent.Fields.Should().ContainSingle(f => f.Name == SomeName && f.Value == SomeValue);
            listItemContent.NestedFields.Should().BeEmpty();
        }

        [Test]
        public void ShouldUseConstructorInFactoryMethod()
        {
            //Given
            //When
            var listItemContent = new ListItemContent(SomeName, SomeValue);

            //Then
            listItemContent.Fields.Should().ContainSingle(f => f.Name == SomeName && f.Value == SomeValue);
            listItemContent.NestedFields.Should().BeEmpty();
        }

        [TestCase(SomeValue2, SomeValue2, true)]
        [TestCase(SomeValue2, SomeValue3, false)]
        public void ShouldCheckEqualotyWithNestedFields(string nestedValue1, string nestedValue2, bool expectedEqualityResult)
        {
            //Given
            var firstItemContent = new ListItemContent(SomeName, SomeValue)
                .AddNestedItem(new ListItemContent(SomeName2, nestedValue1));

            var secondItemContent = new ListItemContent(SomeName, SomeValue)
                .AddNestedItem(new ListItemContent(SomeName2, nestedValue2));

            //When
            var result = firstItemContent.Equals(secondItemContent);

            //Then
            result.Should().Be(expectedEqualityResult);
        }
    }
}