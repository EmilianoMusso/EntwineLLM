using Markdig;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace EntwineLlm
{
    public partial class MarkdownViewer : UserControl
    {
        public MarkdownViewer()
        {
            InitializeComponent();
            MarkdownEditor.TextChanged += UpdateMarkdownPreview;
        }

        public void DisplaySuggestion(string markdownText)
        {
            MarkdownEditor.Text = markdownText;
        }

        private void UpdateMarkdownPreview(object sender, EventArgs e)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            string markdownText = MarkdownEditor.Text;
            string htmlContent = Markdown.ToHtml(markdownText, pipeline);

            string htmlWithStyle = $@"
        <html>
        <head>
            <style>
                body {{ background-color: #2A2A2A; color: #F0F0F0; font-family: Arial, sans-serif; }}
                h1, h2, h3 {{ color: #569CD6; }}
                code {{ background-color: #1E1E1E; color: #D69D85; padding: 2px; border-radius: 3px; }}
                pre {{ background-color: #1E1E1E; padding: 10px; border-radius: 5px; }}
            </style>
        </head>
        <body>
            {htmlContent}
        </body>
        </html>";

            MarkdownPreview.NavigateToString(htmlWithStyle);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Markdown Files|*.md",
                DefaultExt = "md"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, MarkdownEditor.Text);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "HTML Files|*.html",
                DefaultExt = "html"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, Markdown.ToHtml(MarkdownEditor.Text));
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void BtnCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (codeColumn.ActualWidth >= 0.0 && codeColumn.ActualWidth <= 0.9)
            {
                codeColumn.Width = GridLength.Auto;
            }
            else
            {
                codeColumn.Width = new GridLength(0);
            }
        }
    }
}
