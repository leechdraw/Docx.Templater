using System.Xml.Linq;

namespace Docx.Templater
{
    internal static class R
    {
        private static readonly XNamespace NameSpace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

        public static readonly XName Embed = NameSpace + "embed";   
    }
}
