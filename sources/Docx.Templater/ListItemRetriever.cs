using System.Linq;
using System.Xml.Linq;

namespace Docx.Templater
{
    public static class ListItemRetriever
    {
        public static ListItem RetrieveListItem(XDocument numbering, XDocument styles,
            XElement paragraph)
        {
            // The following is an optimization - only determine ListItemInfo once for a paragraph.
            var listItem = paragraph.Annotation<ListItem>();
            if (listItem != null)
                return listItem;

            var paragraphNumberingProperties = paragraph.Elements(W.PPr)
                .Elements(W.NumPr).FirstOrDefault();

            var paragraphStyle = (string)paragraph.Elements(W.PPr).Elements(W.PStyle)
                .Attributes(W.Val).FirstOrDefault();

            ListItemInfo listItemInfo;
            if (paragraphNumberingProperties != null &&
                paragraphNumberingProperties.Element(W.NumId) != null)
            {
                // Paragraph numbering properties must contain a numId.
                var numId = (int)paragraphNumberingProperties.Elements(W.NumId)
                    .Attributes(W.Val).FirstOrDefault();

                var ilvl = (int?)paragraphNumberingProperties.Elements(W.Ilvl)
                    .Attributes(W.Val).FirstOrDefault();

                if (ilvl != null)
                {
                    listItemInfo = GetListItemInfoByNumIdAndIlvl(numbering, styles, numId,
                        (int)ilvl);
                    paragraph.AddAnnotation(listItemInfo);
                    return new ListItem(paragraph, listItemInfo.AbstractNumId, numId, ilvl, listItemInfo.IsListItem);
                }
                if (paragraphStyle != null)
                {
                    listItemInfo = GetListItemInfoByNumIdAndStyleId(numbering, styles,
                        numId, paragraphStyle);
                    paragraph.AddAnnotation(listItemInfo);
                    return new ListItem(paragraph, listItemInfo.AbstractNumId, numId, null, listItemInfo.IsListItem);
                }
                listItemInfo = new ListItemInfo(false);
                paragraph.AddAnnotation(listItemInfo);
                return new ListItem(paragraph, listItemInfo.AbstractNumId, numId, null, listItemInfo.IsListItem);
            }
            if (paragraphStyle != null)
            {
                var style = styles.Root.Elements(W.Style)
                    .FirstOrDefault(s =>
                            (string)s.Attribute(W.Type) == "paragraph" &&
                            (string)s.Attribute(W.StyleId) == paragraphStyle);

                if (style != null)
                {
                    var styleNumberingProperties = style.Elements(W.PPr)
                        .Elements(W.NumPr).FirstOrDefault();
                    if (styleNumberingProperties != null &&
                        styleNumberingProperties.Element(W.NumId) != null)
                    {
                        var numId = (int)styleNumberingProperties.Elements(W.NumId)
                            .Attributes(W.Val).FirstOrDefault();

                        var ilvl = (int?)styleNumberingProperties.Elements(W.Ilvl)
                            .Attributes(W.Val).FirstOrDefault();

                        if (ilvl == null)
                            ilvl = 0;

                        listItemInfo = GetListItemInfoByNumIdAndIlvl(numbering, styles,
                            numId, (int)ilvl);
                        paragraph.AddAnnotation(listItemInfo);
                        return new ListItem(paragraph, listItemInfo.AbstractNumId, numId, ilvl, listItemInfo.IsListItem);
                    }
                }
            }
            listItemInfo = new ListItemInfo(false);
            paragraph.AddAnnotation(listItemInfo);
            return new ListItem(paragraph, listItemInfo.AbstractNumId, null, null, listItemInfo.IsListItem);
        }

        private static ListItemInfo GetListItemInfoByNumIdAndIlvl(XDocument numbering,
            XDocument styles, int numId, int ilvl)
        {
            if (numId == 0)
                return new ListItemInfo(false);
            var listItemInfo = new ListItemInfo(true);
            var num = numbering.Root.Elements(W.Num)
                     .FirstOrDefault(e => (int)e.Attribute(W.NumId) == numId);
            if (num == null)
                return new ListItemInfo(false);

            listItemInfo.AbstractNumId = (int?)num.Elements(W.AbstractNumId)
                .Attributes(W.Val).FirstOrDefault();
            var lvlOverride = num.Elements(W.LvlOverride)
                            .FirstOrDefault(e => (int)e.Attribute(W.Ilvl) == ilvl);
            // If there is a w:lvlOverride element, and if the w:lvlOverride contains a w:lvl
            // element, then return it. Otherwise, go look in the abstract numbering definition.
            if (lvlOverride != null)
            {
                // Get the startOverride, if there is one.
                listItemInfo.Start = (int?)num.Elements(W.LvlOverride)
                    .Where(o => (int)o.Attribute(W.Ilvl) == ilvl).Elements(W.StartOverride)
                    .Attributes(W.Val).FirstOrDefault();
                listItemInfo.Lvl = lvlOverride.Element(W.Lvl);
                if (listItemInfo.Lvl != null)
                {
                    if (listItemInfo.Start == null)
                        listItemInfo.Start = (int?)listItemInfo.Lvl.Elements(W.Start)
                            .Attributes(W.Val).FirstOrDefault();
                    return listItemInfo;
                }
            }
            var a = listItemInfo.AbstractNumId;
            var abstractNum = numbering.Root.Elements(W.AbstractNum)
                            .FirstOrDefault(e => (int)e.Attribute(W.AbstractNumId) == a);
            var numStyleLink = (string)abstractNum.Elements(W.NumStyleLink)
                .Attributes(W.Val).FirstOrDefault();
            if (numStyleLink != null)
            {
                var style = styles.Root.Elements(W.Style)
                            .FirstOrDefault(e => (string)e.Attribute(W.StyleId) == numStyleLink);
                var numPr = style.Elements(W.PPr).Elements(W.NumPr).FirstOrDefault();
                var lNumId = (int)numPr.Elements(W.NumId).Attributes(W.Val)
                    .FirstOrDefault();
                return GetListItemInfoByNumIdAndIlvl(numbering, styles, lNumId, ilvl);
            }
            for (var l = ilvl; l >= 0; --l)
            {
                listItemInfo.Lvl = abstractNum.Elements(W.Lvl)
                    .FirstOrDefault(e => (int)e.Attribute(W.Ilvl) == l);
                if (listItemInfo.Lvl == null)
                    continue;
                if (listItemInfo.Start == null)
                    listItemInfo.Start = (int?)listItemInfo.Lvl.Elements(W.Start)
                        .Attributes(W.Val).FirstOrDefault();
                return listItemInfo;
            }
            return new ListItemInfo(false);
        }

        private static ListItemInfo GetListItemInfoByNumIdAndStyleId(XDocument numbering,
            XDocument styles, int numId, string paragraphStyle)
        {
            // If you have to find the w:lvl by style id, then we can't find it in the w:lvlOverride,
            // as that requires that you have determined the level already.
            var listItemInfo = new ListItemInfo(true);
            var num = numbering.Root.Elements(W.Num)
                    .FirstOrDefault(e => (int)e.Attribute(W.NumId) == numId);

            listItemInfo.AbstractNumId = (int)num.Elements(W.AbstractNumId)
                .Attributes(W.Val).FirstOrDefault();
            var a = listItemInfo.AbstractNumId;
            var abstractNum = numbering.Root.Elements(W.AbstractNum)
                            .FirstOrDefault(e => (int)e.Attribute(W.AbstractNumId) == a);
            var numStyleLink = (string)abstractNum.Element(W.NumStyleLink)
                .Attributes(W.Val).FirstOrDefault();
            if (numStyleLink != null)
            {
                var style = styles.Root.Elements(W.Style)
                            .FirstOrDefault(e => (string)e.Attribute(W.StyleId) == numStyleLink);
                var numPr = style.Elements(W.PPr).Elements(W.NumPr).FirstOrDefault();
                var lNumId = (int)numPr.Elements(W.NumId).Attributes(W.Val).FirstOrDefault();
                return GetListItemInfoByNumIdAndStyleId(numbering, styles, lNumId,
                    paragraphStyle);
            }
            listItemInfo.Lvl = abstractNum.Elements(W.Lvl)
                            .FirstOrDefault(e => (string)e.Element(W.PStyle) == paragraphStyle);
            listItemInfo.Start = (int?)listItemInfo.Lvl
                .Elements(W.Start).Attributes(W.Val)
                .FirstOrDefault();
            return listItemInfo;
        }

        private class ListItemInfo
        {
            public ListItemInfo(bool isListItem)
            {
                IsListItem = isListItem;
            }

            public bool IsListItem { get; set; }

            public XElement Lvl { get; set; }

            public int? Start { get; set; }

            public int? AbstractNumId { get; set; }
        }
    }
}