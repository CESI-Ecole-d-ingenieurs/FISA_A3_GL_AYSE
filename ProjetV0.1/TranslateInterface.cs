public interface ITranslateStrategy
{
    string DestinationLanguage();
}

class French : ITranslateStrategy
{
    public string DestinationLanguage()
    {
        return "fr";
    }
}

class English : ITranslateStrategy
{
    public string DestinationLanguage()
    {
        return "en";
    }
}