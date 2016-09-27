using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Docx.Templater.Errors;
using Docx.Templater.TemplateCustomContent;


namespace Docx.Templater.Processors
{
    internal class SingleProcessor : IProcessor
    {
        private readonly ProcessContext _context;
        private bool _isNeedToRemoveContentControls;

        public SingleProcessor(ProcessContext context)
        {
            _context = context;
        }

        public IProcessor SetRemoveContentControls(bool isNeedToRemove)
        {
            _isNeedToRemoveContentControls = isNeedToRemove;
            return this;
        }

        public ProcessResult FillContent(XElement contentControl, IEnumerable<IContentItem> items)
        {
            var processResult = ProcessResult.NotHandledResult;

            foreach (var contentItem in items)
            {
                processResult.Merge(FillContent(contentControl, contentItem));
            }

            if (processResult.Success && _isNeedToRemoveContentControls)
                contentControl.RemoveContentControl();

            return processResult;
        }

        private ProcessResult FillContent(XContainer contentControl, IContentItem item)
        {
            var processResult = ProcessResult.NotHandledResult;
            if (!(item is SingleContent))
            {
                processResult = ProcessResult.NotHandledResult;
                return processResult;
            }

            var single = (SingleContent)item;

            // If there isn't a field with that name, add an error to the error string, and continue
            // with next field.
            if (contentControl == null)
            {
                processResult.AddError(new ContentControlNotFoundError(single));
                return processResult;
            }

            var content = contentControl.Element(W.SdtContent);

            var childElement = content.FirstLevelDescendantsAndSelf(W.Sdt)
                                      .First(sdt => single.ChildName == sdt.SdtTagName());
            childElement.ElementsAfterSelf(W.Sdt).ToList().ForEach(e => e.Remove());
            childElement.ElementsAfterSelf(W.R).ToList().ForEach(e => e.Remove());
            childElement.ElementsBeforeSelf(W.Sdt).ToList().ForEach(e => e.Remove());
            childElement.ElementsBeforeSelf(W.R).ToList().ForEach(e => e.Remove());
            processResult.AddItemToHandled(item);

            var contentProcessResult = new ContentProcessor(_context)
                .SetRemoveContentControls(_isNeedToRemoveContentControls)
                .FillContent(childElement.Element(W.SdtContent), single.Fields);

            processResult.Merge(contentProcessResult);

            processResult.AddItemToHandled(item);

            if (_isNeedToRemoveContentControls) childElement.RemoveContentControl();

            return processResult;
        }
    }
}