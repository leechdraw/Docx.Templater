using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using Docx.Templater.Errors;
using Docx.Templater.Processors;
using Docx.Templater.TemplateCustomContent;


namespace Docx.Templater
{
    public class TemplateProcessor : IDisposable
    {
        private readonly WordDocumentContainer _wordDocument;
        private bool _isNeedToRemoveContentControls;
        private bool _isNeedToNoticeAboutErrors;

        private TemplateProcessor(WordprocessingDocument wordDocument)
        {
            _wordDocument = new WordDocumentContainer(wordDocument);
            _isNeedToNoticeAboutErrors = true;
        }

        public TemplateProcessor(string fileName)
            : this(WordprocessingDocument.Open(fileName, true))
        {
        }

        public TemplateProcessor(Stream stream)
            : this(WordprocessingDocument.Open(stream, true))
        {
        }

        public TemplateProcessor(XDocument templateSource, XDocument stylesPart = null, XDocument numberingPart = null)
        {
            _isNeedToNoticeAboutErrors = true;
            _wordDocument = new WordDocumentContainer(templateSource, stylesPart, numberingPart);
        }

        public XDocument Document { get { return _wordDocument.MainDocumentPart; } }

        public XDocument NumberingPart { get { return _wordDocument.NumberingPart; } }

        public XDocument StylesPart { get { return _wordDocument.StylesPart; } }

        public IEnumerable<ImagePart> ImagesPart { get { return _wordDocument.ImagesPart; } }

        public Dictionary<string, XDocument> HeaderParts { get { return _wordDocument.HeaderParts; } }

        public Dictionary<string, XDocument> FooterParts { get { return _wordDocument.FooterParts; } }

        public TemplateProcessor SetRemoveContentControls(bool isNeedToRemove)
        {
            _isNeedToRemoveContentControls = isNeedToRemove;
            return this;
        }

        public TemplateProcessor SetNoticeAboutErrors(bool isNeedToNotice)
        {
            _isNeedToNoticeAboutErrors = isNeedToNotice;
            return this;
        }

        public TemplateProcessor FillContent(Content content)
        {
            var processor = new ContentProcessor(
                new ProcessContext(_wordDocument))
                .SetRemoveContentControls(_isNeedToRemoveContentControls);

            var processResult = processor.FillContent(Document.Root.Element(W.Body), content);

            if (_wordDocument.HasFooters)
            {
                foreach (var footer in _wordDocument.FooterParts.Values)
                {
                    var footerProcessResult = processor.FillContent(footer.Element(W.Footer), content);
                    processResult.Merge(footerProcessResult);
                }
            }

            if (_wordDocument.HasHeaders)
            {
                foreach (var header in _wordDocument.HeaderParts.Values)
                {
                    var headerProcessResult = processor.FillContent(header.Element(W.Header), content);
                    processResult.Merge(headerProcessResult);
                }
            }

            if (_isNeedToNoticeAboutErrors)
                AddErrors(processResult.Errors);

            return this;
        }

        public void SaveChanges()
        {
            _wordDocument.SaveChanges();
        }

        public void Dispose()
        {
            if (_wordDocument != null)
                _wordDocument.Dispose();
        }

        /// <summary>
        /// Adds a list of errors as red text on yellow at the beginning of the document.
        /// </summary>
        /// <param name="errors">List of errors.</param>
        private void AddErrors(IList<IError> errors)
        {
            if (errors.Any())
                Document.Root
                    .Element(W.Body)
                    .AddFirst(errors.Select(s =>
                        new XElement(W.P,
                            new XElement(W.R,
                                new XElement(W.RPr,
                                    new XElement(W.Color,
                                        new XAttribute(W.Val, "red")),
                                    new XElement(W.Sz,
                                        new XAttribute(W.Val, "28")),
                                    new XElement(W.SzCs,
                                        new XAttribute(W.Val, "28")),
                                    new XElement(W.Highlight,
                                        new XAttribute(W.Val, "yellow"))),
                                new XElement(W.T, s.Message)))));
        }
    }
}