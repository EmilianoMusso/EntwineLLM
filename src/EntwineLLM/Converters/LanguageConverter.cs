using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace EntwineLlm.Converters
{
    internal class LanguageConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var languageNames = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                                       .Where(c => !string.IsNullOrEmpty(c.Name))
                                       .Select(c => c.EnglishName)
                                       .Distinct()
                                       .OrderBy(name => name)
                                       .ToArray();

            return new StandardValuesCollection(languageNames);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
    }
}
