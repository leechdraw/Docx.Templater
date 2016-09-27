using System.Collections.Generic;


namespace Docx.Templater.Processors
{
    internal class ProcessContext
    {
        internal WordDocumentContainer Document { get; private set; }

        internal Dictionary<int, int> LastNumIds { get; private set; }

        internal ProcessContext(WordDocumentContainer document)
        {
            Document = document;
            LastNumIds = new Dictionary<int, int>();
        }
    }
}