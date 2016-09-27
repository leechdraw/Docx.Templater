using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Docx.Templater
{
    internal class NumberingAccessor
    {
        private readonly XDocument _numberingPart;

        private readonly Dictionary<int, int> _lastNumIds;

        private static readonly Random Random = new Random();

        internal NumberingAccessor(XDocument numberingPart, Dictionary<int, int> lastNumIds)
        {
            _numberingPart = numberingPart;

            _lastNumIds = lastNumIds;
        }

        public void ResetNumbering(IEnumerable<XElement> elements)
        {
            var numPrs = elements.Descendants(W.NumPr).Where(d => d.Element(W.NumId) != null).ToArray();

            foreach (var numPr in numPrs.GroupBy(e => (int)e.Element(W.NumId).Attribute(W.Val)))
            {
                var numId = numPr.Key;
                var numIds = numPrs.Elements(W.NumId).Attributes(W.Val).Where(e => (int)e == numId);
                var ilvl = int.Parse(numPr.First().Element(W.Ilvl).Attribute(W.Val).Value);

                if (_lastNumIds.ContainsKey(ilvl))
                {
                    var numElementPrototype = _numberingPart
                    .Descendants(W.Num)
                    .FirstOrDefault(n => (int)n.Attribute(W.NumId) == numId);

                    var abstractNumElementPrototype = _numberingPart
                        .Descendants(W.AbstractNum)
                        .FirstOrDefault(e => e.Attribute(W.AbstractNumId).Value ==
                                numElementPrototype
                                .Element(W.AbstractNumId)
                                .Attribute(W.Val).Value);
                    var lastNumElement = _numberingPart
                        .Descendants(W.Num)
                        .OrderBy(n => (int)n.Attribute(W.NumId))
                        .LastOrDefault();
                    if (lastNumElement == null) break;

                    var nextNumId = (int)lastNumElement.Attribute(W.NumId) + 1;

                    var lastAbstractNumElement = _numberingPart.Descendants(W.AbstractNum).Last();
                    var lastAbstractNumId = (int)lastAbstractNumElement.Attribute(W.AbstractNumId);

                    var newAbstractNumElement = new XElement(abstractNumElementPrototype);
                    newAbstractNumElement.Attribute(W.AbstractNumId).SetValue(lastAbstractNumId + 1);

                    var next = Random.Next(int.MaxValue);
                    var nsid = newAbstractNumElement.Element(W.Nsid);
                    if (nsid != null)
                        nsid.Attribute(W.Val).SetValue(next.ToString("X"));

                    lastAbstractNumElement.AddAfterSelf(newAbstractNumElement);

                    var newNumElement = new XElement(numElementPrototype);
                    newNumElement.Attribute(W.NumId).SetValue(nextNumId);
                    newNumElement.Element(W.AbstractNumId).Attribute(W.Val).SetValue(lastAbstractNumId + 1);
                    lastNumElement.AddAfterSelf(newNumElement);

                    foreach (var xElement in numIds)
                    {
                        xElement.SetValue(nextNumId);
                    }

                    _lastNumIds[ilvl] = nextNumId;
                }
                else
                {
                    _lastNumIds.Add(ilvl, numId);
                }
            }
        }
    }
}