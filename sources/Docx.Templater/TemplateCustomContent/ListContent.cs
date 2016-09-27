using System;
using System.Collections.Generic;
using System.Linq;


namespace Docx.Templater.TemplateCustomContent
{
    [ContentItemName("List")]
    public class ListContent : IContentItem, IEquatable<ListContent>
    {
        private readonly List<ListItemContent> _items = new List<ListItemContent>();

        public ListContent(string name)
        {
            Name = name;
        }

        public ListContent(string name, IEnumerable<ListItemContent> items)
            : this(name)
        {
            _items.AddRange(items);
        }

        public string Name { get; private set; }

        public ICollection<ListItemContent> Items
        {
            get { return _items; }
        }

        public List<string> GetFieldNames()
        {
            return GetFieldNames(Items);
        }

        public ListContent AddItem(ListItemContent item)
        {
            Items.Add(item);
            return this;
        }

        public ListContent AddItem(IEnumerable<IContentItem> contentItems)
        {
            Items.Add(new ListItemContent(contentItems));
            return this;
        }

        public bool Equals(ListContent other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name) &&
                   Items.SequenceEqual(other.Items);
        }

        public bool Equals(IContentItem other)
        {
            return Equals(other as ListContent);
        }

        public override int GetHashCode()
        {
            var hc = 0;
            hc = Items.Aggregate(hc, (current, p) => current ^ p.GetHashCode());

            return new { Name, hc }.GetHashCode();
        }

        private static List<string> GetFieldNames(IEnumerable<ListItemContent> items)
        {
            var result = new List<string>();

            foreach (var item in items)
            {
                result.AddRange(item.GetFieldNames());
                result.AddRange(GetFieldNames(item.NestedFields));
            }
            return result.Distinct().ToList();
        }
    }
}