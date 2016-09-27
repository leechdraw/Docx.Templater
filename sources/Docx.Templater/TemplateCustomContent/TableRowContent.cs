using System;
using System.Collections.Generic;

namespace Docx.Templater.TemplateCustomContent
{
    public class TableRowContent : Container<TableRowContent>, IEquatable<TableRowContent>
    {
        public TableRowContent(IEnumerable<IContentItem> contentItems)
            : base(contentItems)
        {
        }

        public bool Equals(TableRowContent other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}