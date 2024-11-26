using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace EntwineLlm.Helpers
{
    internal class ProgressBarHelper
    {
        private readonly IVsThreadedWaitDialogFactory _dialogFactory;
        private IVsThreadedWaitDialog2 _dialog;

        public ProgressBarHelper(IServiceProvider serviceProvider)
        {
            _dialogFactory = serviceProvider.GetService(typeof(SVsThreadedWaitDialogFactory)) as IVsThreadedWaitDialogFactory;
            if (_dialogFactory == null)
            {
                throw new InvalidOperationException("Failed to get IVsThreadedWaitDialogFactory service.");
            }
        }

        public void StartIndeterminateDialog(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _dialogFactory.CreateInstance(out _dialog);
            _dialog?.StartWaitDialog(
                szWaitCaption: "EntwineLLM",
                szWaitMessage: message,
                szProgressText: null,
                varStatusBmpAnim: null,
                szStatusBarText: null,
                iDelayToShowDialog: 0,
                fIsCancelable: false,
                fShowMarqueeProgress: true);
        }

        public void StopDialog()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _dialog?.EndWaitDialog(out _);
            _dialog = null;
        }
    }
}