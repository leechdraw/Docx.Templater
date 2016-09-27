using System;
using System.Collections.Generic;

namespace Docx.Templater.TemplateCustomContent
{
    public class Content : Container<Content>, IEquatable<Content>
    {
        public Content(IEnumerable<IContentItem> contentItems)
            : base(contentItems)
        {
        }

        public bool Equals(Content other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}