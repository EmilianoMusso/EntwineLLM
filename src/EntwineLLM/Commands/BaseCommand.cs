using EntwineLlm.Enums;
using EntwineLlm.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;

namespace EntwineLlm.Commands
{
    internal class BaseCommand
    {
        public AsyncPackage package;

        public string ActiveDocumentPath;

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

        public async Task PerformRefactoringSuggestionAsync(RequestedCodeType codeType)
        {
            var message = "Waiting for LLM response (" + Enum.GetName(typeof(RequestedCodeType), codeType) + ") ...";
            
            var progressBarHelper = new ProgressBarHelper(ServiceProvider.GlobalProvider);
            progressBarHelper.StartIndeterminateDialog(message);

            var methodCode = GetCurrentMethodCode();
            var refactoringHelper = new RefactoringHelper(package);
            await refactoringHelper.RequestCodeSuggestionsAsync(methodCode, ActiveDocumentPath, codeType);

            progressBarHelper.StopDialog();
        }
    }
}
