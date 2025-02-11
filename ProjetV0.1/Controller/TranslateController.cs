using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace translation
{
    public class Translation
    {
        private static Translation _instance;
        private static readonly object _lock = new object();
        public ITranslateStrategy _strategy=new English(); // Default translation strategy set to English

        /// Provides a single instance of the Translation class
        public static Translation Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Translation();
                    }
                    return _instance;
                }
            }
        }

        /// Sets the translation strategy dynamically.
        public void Set_strategy(ITranslateStrategy strategy)
        {
            _strategy = strategy;
        }

        /// Translates a given text from French to the selected target language.
        /// Uses Google Translate API for real-time translations.
        public async Task<string> Translate(string text)
        {
            // Constructing the Google Translate API request URL
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=fr&tl={_strategy.TargetLanguage()}&dt=t&q={Uri.EscapeDataString(text)}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Sending the GET request and retrieving the response
                    string doc_json = await client.GetStringAsync(url);
                    // Parsing JSON response
                    var tab_json = JsonDocument.Parse(doc_json).RootElement[0];
                    var tab_response = tab_json[0].EnumerateArray().ToArray();
                    // Extracting translated text
                    string response = tab_response[0].GetString();

                    return response;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la traduction : " + ex.Message);
                    return null;
                }
            }
        }
    }
}