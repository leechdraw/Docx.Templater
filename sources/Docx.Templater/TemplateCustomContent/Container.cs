using System;
using System.Collections.Generic;
using System.Linq;

namespace Docx.Templater.TemplateCustomContent
{
    public abstract class Container<T> : IEquatable<Container<T>>
      where T : Container<T>
    {
        private readonly Dictionary<Type, List<IContentItem>> _contentByTypes = new Dictionary<Type, List<IContentItem>>
        {
            {typeof(SingleContent), new List<IContentItem>()},
            {typeof(TableContent), new List<IContentItem>()},
            {typeof(ListContent), new List<IContentItem>()},
            {typeof(FieldContent), new List<IContentItem>()},
            {typeof(ImageContent), new List<IContentItem>()},
        };

        protected Container()
        {
        }

        protected Container(IEnumerable<IContentItem> contentItems)
        {
            if (contentItems == null)
                return;
            var contents = contentItems.ToList();
            if (contents.Count == 0)
                return;
            GetCollectionByType<SingleContent>().AddRange(contents.OfType<SingleContent>());
            GetCollectionByType<ListContent>().AddRange(contents.OfType<ListContent>());
            GetCollectionByType<TableContent>().AddRange(contents.OfType<TableContent>());
            GetCollectionByType<FieldContent>().AddRange(contents.OfType<FieldContent>());
            GetCollectionByType<ImageContent>().AddRange(contents.OfType<ImageContent>());
        }

        public IEnumerable<SingleContent> Singles
        {
            get { return GetCollectionByType<SingleContent>().Cast<SingleContent>(); }
        }

        public IEnumerable<TableContent> Tables
        {
            get { return GetCollectionByType<TableContent>().Cast<TableContent>(); }
        }

        public IEnumerable<ListContent> Lists
        {
            get { return GetCollectionByType<ListContent>().Cast<ListContent>(); }
        }

        public IEnumerable<FieldContent> Fields
        {
            get { return GetCollectionByType<FieldContent>().Cast<FieldContent>(); }
        }

        public IEnumerable<ImageContent> Images
        {
            get { return GetCollectionByType<ImageContent>().Cast<ImageContent>(); }
        }

        public IEnumerable<string> GetFieldNames()
        {
            var result = new List<string>();
            result.AddRange(GetNamesFromCollection(Tables));
            result.AddRange(Tables.SelectMany(t => t.Rows.SelectMany(r => r.GetFieldNames())));
            result.AddRange(GetNamesFromCollection(Lists));
            result.AddRange(Lists.SelectMany(l => l.GetFieldNames()));
            result.AddRange(GetNamesFromCollection(Images));
            result.AddRange(GetNamesFromCollection(Fields));
            result.AddRange(Singles.SelectMany(f => new[] { f.Name, f.ChildName }.Concat(f.Fields.GetFieldNames())));
            return result;
        }

        public IContentItem GetContentItem(string name)
        {
            return GetAll().FirstOrDefault(t => t.Name == name);
        }

        public bool Equals(Container<T> other)
        {
            return other != null && GetAll().SequenceEqual(other.GetAll());
        }

        public override int GetHashCode()
        {
            var hc = 0;

            hc = GetAll().Aggregate(hc, (current, p) => current ^ p.GetHashCode());

            return hc;
        }

        public T AddContent<TContent>(TContent item)
            where TContent : IContentItem
        {
            if (item == null)
                return (T)this;
            var collection = GetCollectionByType<TContent>();
            collection.Add(item);
            return (T)this;
        }

        public IEnumerable<IContentItem> GetAll()
        {
            return _contentByTypes.Values.SelectMany(x => x);
        }

        private List<IContentItem> GetCollectionByType<TContent>()
        {
            return _contentByTypes[typeof(TContent)];
        }

        private static IEnumerable<string> GetNamesFromCollection<TContent>(IEnumerable<TContent> collection)
            where TContent : IContentItem
        {
            return collection.Select(x => x.Name);
        }
    }
}