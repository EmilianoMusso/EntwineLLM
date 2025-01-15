using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace EntwineLlm.Converters
{
    internal class LlmModelConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            => new(ThreadHelper.JoinableTaskFactory.Run(async () => await EntwineLlmPackage.LlmClient.GetModelListAsync()));

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
    }
}
