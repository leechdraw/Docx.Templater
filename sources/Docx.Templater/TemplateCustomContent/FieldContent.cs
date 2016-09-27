using System;

namespace Docx.Templater.TemplateCustomContent
{
    [ContentItemName("Field")]
    public class FieldContent : IContentItem, IEquatable<FieldContent>
    {
        public FieldContent(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; private set; }

        public bool Equals(FieldContent other)
        {
            if (other == null) return false;

            return Name.Equals(other.Name) &&
                   Value.Equals(other.Value);
        }

        public bool Equals(IContentItem other)
        {
            return Equals(other as FieldContent);
        }

        public override int GetHashCode()
        {
            return new { Name, Value }.GetHashCode();
        }
    }
}