using System;

namespace EntwineLlm.Commands.Interfaces
{
    internal interface IBaseCommand
    {
        int Id {  get; }
        void Execute(object sender, EventArgs e);
    }
}
