using EntwineLlm.Enums;
using EntwineLlm.Helpers;
using ICSharpCode.AvalonEdit;
using Microsoft.VisualStudio.Shell;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EntwineLlm.Commands
{
    internal class BaseCommand
    {
        public AsyncPackage package;

        public string ActiveDocumentPath;
        public TextBox ManualPromptTextBox;
        public TextEditor SuggestedCodeEditor;

        public BaseCommand(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }

        public string GetCurrentMethodCode()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (Package.GetGlobalService(typeof(EnvDTE.DTE)) is not EnvDTE.DTE dte)
            {
                return string.Empty;
            }

            var activeDocument = dte.ActiveDocument;
            if (activeDocument == null)
            {
                return string.Empty;
            }

            ActiveDocumentPath = dte.ActiveDocument.FullName;

            if (activeDocument.Selection is not EnvDTE.TextSelection textSelection)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(textSelection.Text))
            {
                textSelection.SelectLine();
                return textSelection.Text.Trim();
            }

            var selectedLines = textSelection.Text
                .Split('\r', '\n')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l));

            return string.Join("\r\n", selectedLines);
        }

        public async Task PerformRefactoringSuggestionAsync(CodeType codeType, string manualPrompt = "")
        {
            var message = "Waiting for LLM response (task requested: " + Enum.GetName(typeof(CodeType), codeType) + ") ...";

            var progressBarHelper = new ProgressBarHelper(ServiceProvider.GlobalProvider);
            progressBarHelper.StartIndeterminateDialog(message);

            var methodCode = SuggestedCodeEditor != null ?
                SuggestedCodeEditor.Text
                : GetCurrentMethodCode();

            if (string.IsNullOrEmpty(methodCode))
            {
                progressBarHelper.StopDialog();
                WindowHelper.WarningBox("It is necessary to select the source code to be processed from the editor");
                return;
            }

            var refactoringHelper = new RefactoringHelper(package);
            await refactoringHelper.RequestCodeSuggestionsAsync(methodCode, ActiveDocumentPath, codeType, manualPrompt);

            progressBarHelper.StopDialog();
        }
    }
}
