using Docx.Templater.TemplateCustomContent;
using FluentAssertions;
using NUnit.Framework;

namespace Docx.Templater.Tests
{
    [TestFixture]
    public class TableContentTests
    {
        [TestCase("Developer", "Developer", true)]
        [TestCase("Developer", "Developer2", false)]
        public void ShouldCheckEqualutyWithConent(string value1, string value2, bool expectedResult)
        {
            //Given
            var firstTableContent =
                new TableContent("Team Members Table")
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Eric"),
                        new FieldContent("Role", "Program Manager")})
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Bob"),
                        new FieldContent("Role", value1)});

            var secondTableContent =
                new TableContent("Team Members Table")
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Eric"),
                        new FieldContent("Role", "Program Manager")})
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Bob"),
                        new FieldContent("Role", value2)});
            //When
            var result = firstTableContent.Equals(secondTableContent);

            //Then
            result.Should().Be(expectedResult);
        }
    }
}