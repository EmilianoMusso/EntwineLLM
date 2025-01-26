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

        public static void MsgBox(byte boxType, string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string title = $"EntwineLLM - ";
            OLEMSGICON msgIcon = OLEMSGICON.OLEMSGICON_NOICON;

            switch (boxType)
            {
                case 0:
                    title += "Warning";
                    msgIcon = OLEMSGICON.OLEMSGICON_WARNING;
                    break;

                case 1:
                    title += "Error";
                    msgIcon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    break;
            }

            VsShellUtilities.ShowMessageBox(
                ServiceProvider.GlobalProvider,
                message,
                title,
                msgIcon,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public static void WarningBox(string message)
        {
            MsgBox(boxType: 0, message);
        }

        public static void ErrorBox(string message)
        {
            MsgBox(boxType: 1, message);
        }
    }
}
