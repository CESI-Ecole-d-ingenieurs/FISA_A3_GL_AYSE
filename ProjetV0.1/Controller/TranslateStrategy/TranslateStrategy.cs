
public interface ITranslateStrategy
{
    /// Retrieves the target language code (e.g., "fr" for French, "en" for English).
    string TargetLanguage();
}

/// Implements the French translation strategy.
class French : ITranslateStrategy
{
    public string TargetLanguage()
    {
        return "fr"; // French language code
    }
}

/// Implements the English translation strategy.
class English : ITranslateStrategy
{
    public string TargetLanguage()
    {
        return "en"; // English language code
    }
}