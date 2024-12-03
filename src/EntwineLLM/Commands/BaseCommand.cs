using EntwineLlm.Enums;
using EntwineLlm.Helpers;
using ICSharpCode.AvalonEdit;
using Microsoft.VisualStudio.Shell;
using System;
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

            if (!(Package.GetGlobalService(typeof(EnvDTE.DTE)) is EnvDTE.DTE dte))
            {
                return string.Empty;
            }

            var activeDocument = dte.ActiveDocument;
            if (activeDocument == null)
            {
                return string.Empty;
            }

            if (!(activeDocument.Selection is EnvDTE.TextSelection textSelection))
            {
                return string.Empty;
            }

            ActiveDocumentPath = dte.ActiveDocument.FullName;
            return textSelection.Text;
        }

        public async Task PerformRefactoringSuggestionAsync(CodeType codeType, string manualPrompt = "")
        {
            var message = "Waiting for LLM response (task requested: " + Enum.GetName(typeof(CodeType), codeType) + ") ...";

            var progressBarHelper = new ProgressBarHelper(ServiceProvider.GlobalProvider);
            progressBarHelper.StartIndeterminateDialog(message);

            var methodCode = SuggestedCodeEditor != null ?
                SuggestedCodeEditor.Text
                : GetCurrentMethodCode();

            var refactoringHelper = new RefactoringHelper(package);
            await refactoringHelper.RequestCodeSuggestionsAsync(methodCode, ActiveDocumentPath, codeType, manualPrompt);

            progressBarHelper.StopDialog();
        }
    }
}
