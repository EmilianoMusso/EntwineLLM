using EntwineLlm.Commands;
using EntwineLlm.Commands.Interfaces;
using Microsoft.VisualStudio.Shell;
using System;

namespace EntwineLlm
{
    internal sealed class GenerateTestsCommand(AsyncPackage package) : BaseCommand(package), IBaseCommand
    {
        public int Id
        {
            get
            {
                return 251;
            }
        }

        public void Execute(object sender, EventArgs e)
        {
            _ = PerformRefactoringSuggestionAsync(Enums.RequestedCodeType.Test);
        }
    }
}
