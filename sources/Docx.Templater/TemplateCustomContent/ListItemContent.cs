using System;
using System.Collections.Generic;
using System.Linq;

namespace Docx.Templater.TemplateCustomContent
{
    public class ListItemContent : Container<ListItemContent>, IEquatable<ListItemContent>
    {
        private readonly List<ListItemContent> _nestedFields = new List<ListItemContent>();

        public ListItemContent()
        {
        }

        public ListItemContent(IEnumerable<IContentItem> contentItems)
            : base(contentItems)
        {
        }

        public ListItemContent(string name, string value)
        {
            AddContent(new FieldContent(name, value));
        }

        public ListItemContent(string name, string value, IEnumerable<ListItemContent> nestedfields)
        {
            AddContent(new FieldContent(name, value));
            _nestedFields.AddRange(nestedfields);
        }

        public ICollection<ListItemContent> NestedFields
        {
            get { return _nestedFields; }
        }

        public ListItemContent AddNestedItem(ListItemContent nestedItem)
        {
            NestedFields.Add(nestedItem);
            return this;
        }

        public ListItemContent AddNestedItem(IContentItem nestedItem)
        {
            NestedFields.Add(new ListItemContent(new[] { nestedItem }));
            return this;
        }

        public bool Equals(ListItemContent other)
        {
            if (other == null) return false;

            var equals = base.Equals(other);

            if (NestedFields != null)
                return equals && NestedFields.SequenceEqual(other.NestedFields);

            return equals;
        }

        public override int GetHashCode()
        {
            var nestedHc = 0;

            nestedHc = NestedFields.Aggregate(nestedHc, (current, p) => current ^ p.GetHashCode());
            var baseHc = base.GetHashCode();

            return new { baseHc, nestedHc }.GetHashCode();
        }
    }
}