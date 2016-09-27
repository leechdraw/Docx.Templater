using System;
using System.IO;
using System.Linq;


namespace Docx.Templater.TemplateCustomContent
{
    [ContentItemName("Image")]
    public class ImageContent : IContentItem, IEquatable<ImageContent>
    {
        private byte[] _binary;
        private readonly string _fileName;

        public ImageContent(string name, string fileName)
        {
            _fileName = fileName;
            Name = name;
        }

        public ImageContent(string name, byte[] binary)
        {
            _binary = binary;
            Name = name;
        }

        public string Name { get; private set; }

        public byte[] Binary { get { return _binary ?? (_binary = File.ReadAllBytes(_fileName)); } }

        public bool Equals(ImageContent other)
        {
            if (other == null) return false;

            return Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase) &&
                   Binary.SequenceEqual(other.Binary);
        }

        public bool Equals(IContentItem other)
        {
            return Equals(other as ImageContent);
        }

        public override int GetHashCode()
        {
            return new { Name, Binary }.GetHashCode();
        }
    }
}