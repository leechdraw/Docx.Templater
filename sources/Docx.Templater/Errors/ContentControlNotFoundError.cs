using System;
using Docx.Templater.TemplateCustomContent;

namespace Docx.Templater.Errors
{
    internal class ContentControlNotFoundError : IError, IEquatable<ContentControlNotFoundError>
    {
        private const string ErrorMessageTemplate =
                    "{0} Content Control '{1}' not found.";

        internal ContentControlNotFoundError(IContentItem contentItem)
        {
            ContentItem = contentItem;
        }

        public string Message
        {
            get
            {
                return string.Format(ErrorMessageTemplate, ContentItem.GetContentItemName(), ContentItem.Name);
            }
        }

        public IContentItem ContentItem { get; private set; }

        public bool Equals(ContentControlNotFoundError other)
        {
            return other != null && other.ContentItem.Equals(ContentItem);
        }

        public bool Equals(IError other)
        {
            return Equals(other as ContentControlNotFoundError);
        }

        public override int GetHashCode()
        {
            return ContentItem.GetHashCode();
        }
    }
}