using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Threading.Tasks;

namespace EntwineLlm.Helpers
{
    internal static class WindowHelper
    {
        public static async Task<ToolWindowPane> ShowToolWindowAsync<T>(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var window = await package.FindToolWindowAsync(typeof(T), 0, true, package.DisposalToken);
            if (window == null || window.Frame == null)
            {
                throw new NotSupportedException("Cannot create window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            return window;
        }
    }
}
