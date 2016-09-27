using System;

namespace Docx.Templater.TemplateCustomContent
{
	public interface IContentItem : IEquatable<IContentItem>
	{
		string Name { get; }
	}
}
