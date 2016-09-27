using Docx.Templater.TemplateCustomContent;
using FluentAssertions;
using NUnit.Framework;

namespace Docx.Templater.Tests
{
    [TestFixture]
    public class FieldContentTests
    {
        private const string Name = "Some Name";
        private const string Value = "Some Value";
        private const string OtherValue = "Other Value";

        [Test]
        public void FieldContentConstructorWithArgumentsShouldFillNameAndValue()
        {
            //Given
            //When
            var fieldContent = new FieldContent(Name, Value);

            //Then
            fieldContent.Name.Should().Be(Name);
            fieldContent.Value.Should().Be(Value);
        }

        [TestCase(Name, Value, Value, true)]
        [TestCase(Name, Value, OtherValue, false)]
        public void ShouldCompareTwoObjects(string name, string firstValue, string secondValue, bool expectedCompareResult)
        {
            //Given
            var firstFieldContent = new FieldContent(name, firstValue);
            var secondFieldContent = new FieldContent(name, secondValue);

            //When
            var result = firstFieldContent.Equals(secondFieldContent);

            //Then
            result.Should().Be(expectedCompareResult);
        }

        [Test]
        public void EqualsTest_CompareWithNull_NotEquals()
        {
            //Given
            var firstFieldContent = new FieldContent(Name, Value);

            //When
            var result = firstFieldContent.Equals(null);

            //Then
            result.Should().BeFalse();
        }
    }
}