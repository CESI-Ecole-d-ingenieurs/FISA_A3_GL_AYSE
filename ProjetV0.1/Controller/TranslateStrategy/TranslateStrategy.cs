public interface ITranslateStrategy
{
    string TargetLanguage();
}

class French : ITranslateStrategy
{
    public string TargetLanguage()
    {
        return "fr";
    }
}

class English : ITranslateStrategy
{
    public string TargetLanguage()
    {
        return "en";
    }
}