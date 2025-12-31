using md2visio.vsdx.@base;
using Visio = Microsoft.Office.Interop.Visio;

namespace md2visio.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of IVisioSession for unit testing
    /// Throws NotSupportedException for all operations that require actual Visio
    /// </summary>
    public class MockVisioSession : IVisioSession
    {
        private bool _disposed;

        public List<string> SavedFiles { get; } = new();
        public List<string> ClosedDocuments { get; } = new();
        public int CreateDocumentCallCount { get; private set; }
        public int OpenStencilCallCount { get; private set; }

        public bool Visible { get; }

        public Visio.Application Application =>
            throw new NotSupportedException("MockVisioSession does not support actual Visio operations. Use integration tests for real Visio testing.");

        public MockVisioSession(bool visible = false)
        {
            Visible = visible;
        }

        public Visio.Document CreateDocument()
        {
            CreateDocumentCallCount++;
            throw new NotSupportedException("MockVisioSession does not support actual Visio operations.");
        }

        public Visio.Document OpenStencil(string path)
        {
            OpenStencilCallCount++;
            throw new NotSupportedException("MockVisioSession does not support actual Visio operations.");
        }

        public void SaveDocument(Visio.Document doc, string path, bool overwrite = true)
        {
            SavedFiles.Add(path);
        }

        public void CloseDocument(Visio.Document doc)
        {
            ClosedDocuments.Add("closed");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}
