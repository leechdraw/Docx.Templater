using Docx.Templater.TemplateCustomContent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Docx.Templater.Processors
{
    internal class ContentProcessor
    {
        private bool _isNeedToRemoveContentControls;

        private readonly List<IProcessor> _processors;

        internal ContentProcessor(ProcessContext context)
        {
            _processors = new List<IProcessor>
			{
				new FieldsProcessor(),
				new TableProcessor(context),
				new ListProcessor(context),
                new ImagesProcessor(context),
				new SingleProcessor(context),
			};
        }

        public ContentProcessor SetRemoveContentControls(bool isNeedToRemove)
        {
            _isNeedToRemoveContentControls = isNeedToRemove;
            foreach (var processor in _processors)
            {
                processor.SetRemoveContentControls(_isNeedToRemoveContentControls);
            }
            return this;
        }

        public ProcessResult FillContent(XElement content, IEnumerable<IContentItem> data)
        {
            var result = ProcessResult.NotHandledResult;
            var processedItems = new List<IContentItem>();
            data = data.ToList();

            foreach (var contentItems in data.GroupBy(d => d.Name))
            {
                if (processedItems.Any(i => i.Name == contentItems.Key)) continue;

                var contentControls = FindContentControls(content, contentItems.Key).ToList();

                ////Need to get error message from processor.
                if (!contentControls.Any())
                    contentControls.Add(null);

                foreach (var xElement in contentControls)
                {
                    if (contentItems.Any(item => item is TableContent) && xElement != null)
                    {
                        var processTableFieldsResult = ProcessTableFields(data.OfType<FieldContent>(), xElement);
                        processedItems.AddRange(processTableFieldsResult.HandledItems);

                        result.Merge(processTableFieldsResult);
                    }

                    foreach (var processor in _processors)
                    {
                        var processorResult = processor.FillContent(xElement, contentItems);

                        processedItems.AddRange(processorResult.HandledItems);
                        result.Merge(processorResult);
                    }
                }
            }

            return result;
        }

        public ProcessResult FillContent(XElement content, Content data)
        {
            return FillContent(content, data.GetAll());
        }

        public ProcessResult FillContent(XElement content, IContentItem data)
        {
            return FillContent(content, new List<IContentItem> { data });
        }

        /// <summary>
        /// Processes table data that should not be duplicated
        /// </summary>
        /// <param name="fields">Possible fields</param>
        /// <param name="xElement">Table content control</param>
        /// <returns>List of content items that were processed</returns>
        private ProcessResult ProcessTableFields(IEnumerable<FieldContent> fields, XContainer xElement)
        {
            var processResult = ProcessResult.NotHandledResult;
            foreach (var fieldContentControl in fields)
            {
                var innerContentControls = FindContentControls(xElement.Element(W.SdtContent), fieldContentControl.Name);
                foreach (var innerContentControl in innerContentControls)
                {
                    var processor = _processors.OfType<FieldsProcessor>().FirstOrDefault();
                    if (processor != null)
                    {
                        var result = FieldsProcessor.FillContent(innerContentControl, fieldContentControl);
                        processResult.Merge(result);
                    }
                }
            }

            return processResult;
        }

        private static IEnumerable<XElement> FindContentControls(XElement content, string tagName)
        {
            return content
                //top level content controls
                .FirstLevelDescendantsAndSelf(W.Sdt)
                //with specified tagName
                .Where(sdt => tagName == sdt.SdtTagName());
        }
    }
}