using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;

namespace EntwineLlm.Helpers
{
    internal static class ProjectHelper
    {
        public static void AddFileToSolution(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2
                ?? throw new InvalidOperationException("DTE not available");

            var activeProject = GetActiveProject(dte) ?? throw new InvalidOperationException("No active project found.");
            activeProject.ProjectItems.AddFromFileCopy(filePath);
        }

        public static Project GetActiveProject(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Array activeSolutionProjects = (Array)dte.ActiveSolutionProjects;
            return activeSolutionProjects.Length == 0 ? null : activeSolutionProjects.GetValue(0) as Project;
        }
    }
}
