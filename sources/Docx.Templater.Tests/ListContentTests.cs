using System.Collections.Generic;
using Docx.Templater.TemplateCustomContent;
using FluentAssertions;
using NUnit.Framework;

namespace Docx.Templater.Tests
{
    [TestFixture]
    public class ListContentTests
    {
        private const string Name = "SomeName";

        [Test]
        public void ShouldSetNameFromConstructor()
        {
            //Given

            //when
            var listContent = new ListContent(Name);

            //Then
            listContent.Name.Should().Be(Name);
        }

        [Test]
        public void ShouldSetNameAndItemsFromConstructor()
        {
            //Given
            //When
            var listContent = new ListContent(Name, new List<ListItemContent>());

            //Then
            listContent.Name.Should().Be(Name);
            listContent.Items.Should().NotBeNull();
        }

        [Test]
        public void ShouldSetNameAndItemsByContructorParams()
        {
            //Given
            var item1 = new ListItemContent();
            var item2 = new ListItemContent();

            //When
            var listContent = new ListContent(Name, new[] { item1, item2 });

            //Then
            listContent.Name.Should().Be(Name);
            listContent.Items.Count.Should().Be(2);
            listContent.Items.Should().Contain(item1);
            listContent.Items.Should().Contain(item2);
        }

        [TestCase("value2", "value2", true)]
        [TestCase("value2", "value2_", false)]
        public void ShouldCheckEquality(string minorValue1, string minorValue2, bool expectedEqualityResult)
        {
            //Given
            var firstListContent = new ListContent("Name", new[]{
                                        new ListItemContent("Header", "value",
                                                new ListItemContent("Subheader", "value").AsArray()),
                                        new ListItemContent("Header", "value",
                                                new []{new ListItemContent("Subheader", "value"),
                                                new ListItemContent("Subheader", "value2",
                                                new ListItemContent("Subsubheader",minorValue1).AsArray())})
        }
        );

            var secondListContent = new ListContent("Name",
                new[]{new ListItemContent("Header", "value",
                    new ListItemContent("Subheader", "value").AsArray()),
                new ListItemContent("Header", "value",
                    new []{
                    new ListItemContent("Subheader", "value"),
                    new ListItemContent("Subheader", "value2",
                        new ListItemContent("Subsubheader", minorValue2).AsArray())
                        })});
            //When
            var result = firstListContent.Equals(secondListContent);

            //Then
            result.Should().Be(expectedEqualityResult);
        }

        [Test]
        public void ShouldNotBeEqualToNull()
        {
            //Given
            var firstListContent = new ListContent("Name",
                new[]{
            new ListItemContent("Header", "value",
                new ListItemContent("Subheader", "value").AsArray()),
            new ListItemContent("Header", "value",
                new []{
                new ListItemContent("Subheader", "value"),
                new ListItemContent("Subheader", "value",
                    new ListItemContent("Subsubheader1", "value").AsArray())})});
            //When
            var result = firstListContent.Equals(null);

            //Then
            result.Should().BeFalse();
        }
    }
}