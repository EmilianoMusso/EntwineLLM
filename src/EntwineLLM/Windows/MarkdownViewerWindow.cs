using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace EntwineLlm
{
    [Guid("22dcf2f8-c8c1-4ca4-b1aa-cde7897a63a8")]
    public class MarkdownViewerWindow : ToolWindowPane
    {
        public MarkdownViewerWindow() : base(null)
        {
            this.Caption = "Markdown viewer";
            this.Content = new MarkdownViewer();
        }
    }
}
