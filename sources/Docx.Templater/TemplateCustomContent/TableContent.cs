using System;
using System.Collections.Generic;
using System.Linq;


namespace Docx.Templater.TemplateCustomContent
{
    [ContentItemName("Table")]
    public class TableContent : IContentItem, IEquatable<TableContent>
    {
        private readonly List<TableRowContent> _rows = new List<TableRowContent>();

        public TableContent(string name)
        {
            Name = name;
        }

        public TableContent(string name, IEnumerable<TableRowContent> rows)
            : this(name)
        {
            _rows.AddRange(rows);
        }

        public string Name { get; private set; }

        public IEnumerable<TableRowContent> Rows
        {
            get { return _rows; }
        }

        public IEnumerable<string> FieldNames
        {
            get
            {
                return Rows.SelectMany(r => r.GetFieldNames()).Distinct().ToList();
            }
        }

        public TableContent AddRow(TableRowContent row)
        {
            _rows.Add(row);
            return this;
        }
        public TableContent AddRow(IEnumerable<IContentItem> contentItems)
        {
            _rows.Add(new TableRowContent(contentItems));
            return this;
        }

        public bool Equals(TableContent other)
        {
            if (other == null) return false;

            return Name.Equals(other.Name) &&
               Rows.SequenceEqual(other.Rows);
        }

        public bool Equals(IContentItem other)
        {
            return Equals(other as TableContent);
        }

        public override int GetHashCode()
        {
            var hc = Rows.Aggregate(0, (current, p) => current ^ p.GetHashCode());

            return new { Name, hc }.GetHashCode();
        }
    }
}