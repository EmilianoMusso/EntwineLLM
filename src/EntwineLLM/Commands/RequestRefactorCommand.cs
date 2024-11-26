using EntwineLlm.Commands;
using EntwineLlm.Commands.Interfaces;
using Microsoft.VisualStudio.Shell;
using System;

namespace EntwineLlm
{
    internal sealed class RequestRefactorCommand : BaseCommand, IBaseCommand
    {
        public int Id
        {
            get
            {
                return 250;
            }
        }

        public RequestRefactorCommand(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }

        public void Execute(object sender, EventArgs e)
        {
            _ = PerformRefactoringSuggestionAsync(Enums.RequestedCodeType.Refactor);
        }
    }
}
