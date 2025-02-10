/// Defines a strategy interface for translation target languages.
/// This allows switching between different language implementations dynamically.
public interface ITranslateStrategy
{
    /// Retrieves the target language code (e.g., "fr" for French, "en" for English).
    string TargetLanguage();
}

/// Implements the French translation strategy.
/// Returns "fr" as the target language code.
class French : ITranslateStrategy
{
    public string TargetLanguage()
    {
        return "fr"; // French language code
    }
}

/// Implements the English translation strategy.
/// Returns "en" as the target language code.
class English : ITranslateStrategy
{
    public string TargetLanguage()
    {
        return "en"; // English language code
    }
}