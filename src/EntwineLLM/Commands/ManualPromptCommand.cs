using EntwineLlm.Commands;
using EntwineLlm.Commands.Interfaces;
using Microsoft.VisualStudio.Shell;
using System;

namespace EntwineLlm
{
    internal sealed class ManualPromptCommand : BaseCommand, IBaseCommand
    {
        public int Id
        {
            get
            {
                return 0;
            }
        }

        public ManualPromptCommand(AsyncPackage package) : base(package) { }

        public void Execute(object sender, EventArgs e)
        {
            var manualPrompt = ManualPromptTextBox != null ? ManualPromptTextBox.Text : "";
            _ = PerformRefactoringSuggestionAsync(Enums.RequestedCodeType.Manual, manualPrompt);
        }
    }
}
