using EntwineLlm.Models;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace EntwineLlm
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(EntwineLlmPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(RefactorSuggestionWindow))]
    [ProvideOptionPage(typeof(EntwineLlmOptions), "EntwineLlm", "Configuration", 0, 0, true)]
    public sealed class EntwineLlmPackage : AsyncPackage
    {
        public const string PackageGuidString = "3c995b0e-1f37-4cef-9ac7-9771b3fb6162";

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await RequestRefactorCommand.InitializeAsync(this);
            await GenerateTestsCommand.InitializeAsync(this);
        }

        #endregion
    }
}
