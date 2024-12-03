using EntwineLlm.Commands.Interfaces;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace EntwineLlm
{
    internal class CommandsMenu
    {
        private static readonly Guid CommandSet = new("714b6862-aad7-434e-8415-dd928555ba0e");
        private OleMenuCommandService CommandService;

        public OleMenuCommandService Service
        {
            get
            {
                return CommandService;
            }
        }

        public async Task InitializeAsync(AsyncPackage package)
        {
            CommandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
        }

        public void AddCommand(IBaseCommand command)
        {
            var menuCommandID = new CommandID(CommandSet, command.Id);
            var menuItem = new MenuCommand(command.Execute, menuCommandID);
            CommandService.AddCommand(menuItem);
        }

        public void AddCommands(IEnumerable<IBaseCommand> commands)
        {
            foreach (var command in commands)
            {
                AddCommand(command);
            }
        }
    }
}
