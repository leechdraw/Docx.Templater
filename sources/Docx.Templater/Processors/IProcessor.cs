using System.Collections.Generic;
using System.Xml.Linq;
using Docx.Templater.TemplateCustomContent;

namespace Docx.Templater.Processors
{
    internal interface IProcessor
    {
        IProcessor SetRemoveContentControls(bool isNeedToRemove);

        ProcessResult FillContent(XElement contentControl, IEnumerable<IContentItem> items);
    }
}