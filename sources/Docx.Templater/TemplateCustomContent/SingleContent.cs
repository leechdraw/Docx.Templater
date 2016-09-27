using System;
using System.Collections.Generic;


namespace Docx.Templater.TemplateCustomContent
{
    [ContentItemName("Single")]
    public class SingleContent : IContentItem, IEquatable<SingleContent>
    {
        public SingleContent(string name, string childName, IEnumerable<IContentItem> fields)
        {
            Name = name;
            ChildName = childName;
            Fields = new Content(fields);
        }

        public string Name { get; private set; }

        public string ChildName { get; private set; }

        public Content Fields { get; private set; }

        public bool Equals(SingleContent other)
        {
            if (other == null) return false;

            return Name.Equals(other.Name) &&
                   ChildName.Equals(other.ChildName);
        }

        public bool Equals(IContentItem other)
        {
            return Equals(other as SingleContent);
        }

        public override int GetHashCode()
        {
            return new { Name, SubName = ChildName }.GetHashCode();
        }
    }
}