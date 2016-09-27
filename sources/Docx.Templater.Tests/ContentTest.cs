using System;
using Docx.Templater.TemplateCustomContent;
using FluentAssertions;
using NUnit.Framework;

namespace Docx.Templater.Tests
{
    [TestFixture]
    public class ContentTest
    {
        [Test]
        public void ShouldCompareToSimilarObject()
        {
            //Given
            var firstValuesToFill = new Content(
                new IContentItem[]{
                // Add field.
                new FieldContent("Report date", new DateTime(2000, 01, 01).ToShortDateString()),
                // Add table.
                new TableContent("Team Members Table")
                    .AddRow(
                    new []{    
                    new FieldContent("Name", "Eric"),
                        new FieldContent("Role", "Program Manager")}
                        )
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Bob"),
                        new FieldContent("Role", "Developer")}),
                // Add nested list.
                new ListContent("Team Members Nested List")
                    .AddItem(new ListItemContent("Role", "Program Manager")
                        .AddNestedItem(new FieldContent("Name", "Eric"))
                        .AddNestedItem(new FieldContent("Name", "Ann")))
                    .AddItem(new ListItemContent("Role", "Developer")
                        .AddNestedItem(new FieldContent("Name", "Bob"))
                        .AddNestedItem(new FieldContent("Name", "Richard"))),
                // Add image
                new ImageContent("photo", new byte[] { 1, 2, 3 })}
                );

            var secondValuesToFill = new Content(
                new IContentItem[]{
                // Add field.
                new FieldContent("Report date", new DateTime(2000, 01, 01).ToShortDateString()),
                // Add table.
                new TableContent("Team Members Table")
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Eric"),
                        new FieldContent("Role", "Program Manager")})
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Bob"),
                        new FieldContent("Role", "Developer")}),
                // Add nested list.
                new ListContent("Team Members Nested List")
                    .AddItem(new ListItemContent("Role", "Program Manager")
                        .AddNestedItem(new FieldContent("Name", "Eric"))
                        .AddNestedItem(new FieldContent("Name", "Ann")))
                    .AddItem(new ListItemContent("Role", "Developer")
                        .AddNestedItem(new FieldContent("Name", "Bob"))
                        .AddNestedItem(new FieldContent("Name", "Richard"))),
                // Add image
                new ImageContent("photo", new byte[] { 1, 2, 3 })}
                );

            //When
            var result = firstValuesToFill.Equals(secondValuesToFill);

            //Then
            result.Should().BeTrue();
        }

        [Test]
        public void ShouldCompareToDifferentObject()
        {
            //Given
            var firstValuesToFill = new Content(
                new IContentItem[]{
                // Add field.
                new FieldContent("Report date", new DateTime(2000, 01, 01).ToShortDateString()),
                // Add table.
                new TableContent("Team Members Table")
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Eric"),
                        new FieldContent("Role", "Program Manager")})
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Bob"),
                        new FieldContent("Role", "Developer")})}
                );

            var secondValuesToFill = new Content(
                // Add field.
               new []{ new FieldContent("Report date", new DateTime(2000, 01, 01).ToShortDateString())}
                );

            //When
            var result = firstValuesToFill.Equals(secondValuesToFill);

            //Then
            result.Should().BeFalse();
        }

        [Test]
        public void ShouldCompareWithNull()
        {
            //Given
            var firstValuesToFill = new Content(
                new IContentItem[]{
                // Add field.
                new FieldContent("Report date", new DateTime(2000, 01, 01).ToShortDateString()),
                // Add table.
                new TableContent("Team Members Table")
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Eric"),
                        new FieldContent("Role", "Program Manager")})
                    .AddRow(
                    new []{
                        new FieldContent("Name", "Bob"),
                        new FieldContent("Role", "Developer")})}
                );

            //When
            var result = firstValuesToFill.Equals(null);
            //Then
            result.Should().BeFalse();
        }
    }
}