namespace Baxter.ClusterScriptExtensions.Editor.Localization
{
    public enum Language
    {
        English,
        Japanese,
    }

    public static class LanguageChecker
    {
        //日英だけ考える
        public static Language Lang =>
#if cck_ja
            Language.Japanese;
#else
            Language.English;
#endif
    }
}

