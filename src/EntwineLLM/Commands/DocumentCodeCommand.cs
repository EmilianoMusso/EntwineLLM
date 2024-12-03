using EntwineLlm.Commands;
using EntwineLlm.Commands.Interfaces;
using Microsoft.VisualStudio.Shell;
using System;

namespace EntwineLlm
{
    internal sealed class DocumentCodeCommand(AsyncPackage package) : BaseCommand(package), IBaseCommand
    {
        public int Id
        {
            get
            {
                return 252;
            }
        }

        public void Execute(object sender, EventArgs e)
        {
            _ = PerformRefactoringSuggestionAsync(Enums.CodeType.Documentation);
        }
    }
}
