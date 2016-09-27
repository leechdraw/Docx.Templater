using System.Linq;
using Docx.Templater.TemplateCustomContent;

namespace Docx.Templater
{
    internal static class AttributesExtensions
    {
        public static string GetContentItemName(this IContentItem value)
        {
            var contentItemNameAttribute = value.GetType()
                .GetCustomAttributes(typeof(ContentItemNameAttribute), true)
                   .FirstOrDefault() as ContentItemNameAttribute;

            return contentItemNameAttribute != null ? contentItemNameAttribute.Name : null;
        }
    }
}