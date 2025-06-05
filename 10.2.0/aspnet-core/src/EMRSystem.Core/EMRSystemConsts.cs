using EMRSystem.Debugging;

namespace EMRSystem;

public class EMRSystemConsts
{
    public const string LocalizationSourceName = "EMRSystem";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "cfedbcbd8c0d42c9aba6545ccbeab3db";
}
