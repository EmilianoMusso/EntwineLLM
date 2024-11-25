using EntwineLlm.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace EntwineLlm
{
    internal sealed class GenerateTestsCommand
    {
        public const int CommandId = 251;

        public static readonly Guid CommandSet = new Guid("714b6862-aad7-434e-8415-dd928555ba0e");

        private readonly AsyncPackage package;

        private static string _activeDocumentPath;

        private GenerateTestsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static GenerateTestsCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GenerateTestsCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            _ = PerformRefactoringSuggestionAsync();
        }

        private async Task PerformRefactoringSuggestionAsync()
        {
            var progressBarHelper = new ProgressBarHelper(ServiceProvider.GlobalProvider);
            progressBarHelper.StartIndeterminateDialog();

            var methodCode = GetCurrentMethodCode();
            var refactoringHelper = new RefactoringHelper(package);
            await refactoringHelper.RequestCodeSuggestionsAsync(methodCode, _activeDocumentPath, Enums.RequestedCodeType.Test);

            progressBarHelper.StopDialog();
        }

        private static string GetCurrentMethodCode()
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

            _activeDocumentPath = dte.ActiveDocument.FullName;
            return textSelection.Text;
        }
    }
}
