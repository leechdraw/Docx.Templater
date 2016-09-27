using System.Xml.Linq;

namespace Docx.Templater
{
    internal static class A
    {
        private static readonly XNamespace NameSpace = "http://schemas.openxmlformats.org/drawingml/2006/main";

        public static readonly XName Blip = NameSpace + "blip";   
    }
}
