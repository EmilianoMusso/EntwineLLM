using EntwineLlm.Commands;
using EntwineLlm.Commands.Interfaces;
using Microsoft.VisualStudio.Shell;
using System;

namespace EntwineLlm
{
    internal sealed class ManualPromptCommand(AsyncPackage package) : BaseCommand(package), IBaseCommand
    {
        public int Id
        {
            get
            {
                return 0;
            }
        }

        public void Execute(object sender, EventArgs e)
        {
            var manualPrompt = ManualPromptTextBox != null ? ManualPromptTextBox.Text : "";
            _ = PerformRefactoringSuggestionAsync(Enums.RequestedCodeType.Manual, manualPrompt);
        }
    }
}
