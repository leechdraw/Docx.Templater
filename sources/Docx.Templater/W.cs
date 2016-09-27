using System.Xml.Linq;

namespace Docx.Templater
{
    internal static class W
    {
        private static readonly XNamespace NameSpace =
            "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        public static readonly XName Body = NameSpace + "body";
        public static readonly XName Header = NameSpace + "hdr";
        public static readonly XName Footer = NameSpace + "ftr";
        public static readonly XName Sdt = NameSpace + "sdt";
        public static readonly XName SdtPr = NameSpace + "sdtPr";
        public static readonly XName Tag = NameSpace + "tag";
        public static readonly XName Val = NameSpace + "val";
        public static readonly XName SdtContent = NameSpace + "sdtContent";
        public static readonly XName Tbl = NameSpace + "tbl";
        public static readonly XName Tr = NameSpace + "tr";
        public static readonly XName Tc = NameSpace + "tc";
        public static readonly XName TcPr = NameSpace + "tcPr";
        public static readonly XName P = NameSpace + "p";
        public static readonly XName R = NameSpace + "r";
        public static readonly XName T = NameSpace + "t";
        public static readonly XName RPr = NameSpace + "rPr";
        public static readonly XName Highlight = NameSpace + "highlight";
        public static readonly XName PPr = NameSpace + "pPr";
        public static readonly XName Color = NameSpace + "color";
        public static readonly XName Sz = NameSpace + "sz";
        public static readonly XName SzCs = NameSpace + "szCs";
        public static readonly XName VMerge = NameSpace + "vMerge";
        public static readonly XName NumId = NameSpace + "numId";
        public static readonly XName NumPr = NameSpace + "numPr";
        public static readonly XName Ilvl = NameSpace + "ilvl";
        public static readonly XName Num = NameSpace + "num";
        public static readonly XName AbstractNumId = NameSpace + "abstractNumId";
        public static readonly XName AbstractNum = NameSpace + "abstractNum";
        public static readonly XName Nsid = NameSpace + "nsid";
        public static readonly XName LvlOverride = NameSpace + "lvlOverride";
        public static readonly XName StartOverride = NameSpace + "startOverride";
        public static readonly XName Lvl = NameSpace + "lvl";
        public static readonly XName Start = NameSpace + "start";
        public static readonly XName Style = NameSpace + "style";
        public static readonly XName StyleId = NameSpace + "styleId";
        public static readonly XName NumStyleLink = NameSpace + "numStyleLink";
        public static readonly XName PStyle = NameSpace + "pStyle";
        public static readonly XName LvlRestart = NameSpace + "lvlRestart";
        public static readonly XName NumFmt = NameSpace + "numFmt";
        public static readonly XName LvlText = NameSpace + "lvlText";
        public static readonly XName Type = NameSpace + "type";
        public static readonly XName IsLgl = NameSpace + "isLgl";
        public static readonly XName RStyle = NameSpace + "rStyle";
        public static readonly XName Br = NameSpace + "br";
    }
}