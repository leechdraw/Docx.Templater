using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Docx.Templater
{
    internal static class XElementExtensions
    {
        // Set content control value the new value
        public static void ReplaceContentControlWithNewValue(this XElement sdt, string newValue)
        {
            var sdtContentElement = sdt.Element(W.SdtContent);
            
            if (sdtContentElement != null)
            {
                var elementsWithText = sdtContentElement.Elements()
                    .Where(e =>
                        e.DescendantsAndSelf(W.T).Any() &&
                        !e.DescendantsAndSelf(W.Sdt).Any())
                    .ToList();

                var firstContentElementWithText = elementsWithText.FirstOrDefault(d => d.DescendantsAndSelf(W.T).Any());

                if (firstContentElementWithText != null)
                {
                    var firstTextElement = firstContentElementWithText
                        .Descendants(W.T)
                        .First();

                    firstTextElement.Value = newValue;

                    //remove all text elements with its ancestors from the first contentElement
                    var firstElementAncestors = firstTextElement.AncestorsAndSelf().ToList();

                    foreach (var descendants in elementsWithText.DescendantsAndSelf().ToList())
                    {
                        if (!firstElementAncestors.Contains(descendants) && descendants.DescendantsAndSelf(W.T).Any())
                        {
                            descendants.Remove();
                        }
                    }

                    var contentReplacementElement = new XElement(firstContentElementWithText);
                    firstContentElementWithText.AddAfterSelf(contentReplacementElement);
                    firstContentElementWithText.Remove();
                }
                else
                {
                    var element = sdtContentElement.Element(W.P);
                    if (element != null)
                    {
                        element.Add(new XElement(W.R, new XElement(W.T, newValue)));
                    }
                    else
                    {
                        sdtContentElement.Add(new XElement(W.P), new XElement(W.R, new XElement(W.T, newValue)));
                    }
                }
            }
            else
            {
                sdt.Add(new XElement(W.SdtContent, new XElement(W.P), new XElement(W.R, new XElement(W.T, newValue))));
            }

            ReplaceNewLinesWithBreaks(sdt);
        }

        public static void RemoveContentControl(this XElement sdt)
        {
            var sdtContentElement = sdt.Element(W.SdtContent);
            if (sdtContentElement == null)
            {
                sdt.Remove();
                return;
            }

            var parent = new XElement("parent");
            if (sdt.Parent == null)
            {
                //add newElement to fake parent for remove content control
                parent.Add(sdt);
            }
            // Remove the content control, and replace it with its contents.
            sdt.ReplaceWith(sdtContentElement.Elements());

            if (sdt.Parent == parent)
            {
                sdt.Remove();
            }
        }

        public static IEnumerable<XElement> FirstLevelDescendantsAndSelf(this XElement element, XName name)
        {
            var allDescendantsAndSelf = element.DescendantsAndSelf(name).ToList();

            return allDescendantsAndSelf
                .Where(d => !d.Ancestors(name).Intersect(allDescendantsAndSelf).Any());
        }

        public static IEnumerable<XElement> FirstLevelDescendantsAndSelf(this IEnumerable<XElement> element, XName name)
        {
            var allDescendantsAndSelf = element
                //content controls
                .DescendantsAndSelf(name).ToList();

            return allDescendantsAndSelf
                .Where(d => !d.Ancestors().Any(allDescendantsAndSelf.Contains));
        }

        public static string SdtTagName(this XElement sdt)
        {
            if (sdt.Name != W.Sdt) return null;
            try
            {
                return (from e in sdt.Elements(W.SdtPr)
                        from q in e.Elements(W.Tag)
                        from a in q.Attributes(W.Val)
                        select a.Value).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void ReplaceNewLinesWithBreaks(XElement xElem)
        {
            if (xElem == null) return;

            var textWithBreaks = xElem.Descendants(W.T).Where(t => t.Value.Contains("\r\n"));
            foreach (var textWithBreak in textWithBreaks)
            {
                var text = textWithBreak.Value;
                var split = text.Replace("\r\n", "\n").Split(new[] { "\n" }, StringSplitOptions.None);
                textWithBreak.Value = string.Empty;
                foreach (var s in split)
                {
                    textWithBreak.Add(new XElement(W.T, s));
                    textWithBreak.Add(new XElement(W.Br));
                }
                textWithBreak.Descendants(W.Br).Last().Remove();
            }
        }
    }
}