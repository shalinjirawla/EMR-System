using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace EMRSystem.Localization;

public static class EMRSystemLocalizationConfigurer
{
    public static void Configure(ILocalizationConfiguration localizationConfiguration)
    {
        localizationConfiguration.Sources.Add(
            new DictionaryBasedLocalizationSource(EMRSystemConsts.LocalizationSourceName,
                new XmlEmbeddedFileLocalizationDictionaryProvider(
                    typeof(EMRSystemLocalizationConfigurer).GetAssembly(),
                    "EMRSystem.Localization.SourceFiles"
                )
            )
        );
    }
}
