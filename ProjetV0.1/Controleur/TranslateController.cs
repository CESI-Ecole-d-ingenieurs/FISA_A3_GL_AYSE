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
        public ITranslateStrategy _strategy=new English();
         
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
        public void Set_strategy(ITranslateStrategy strategy)
        {
            _strategy = strategy;
        }

        public async Task<string> Translate(string text)
        {
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=fr&tl={_strategy.DestinationLanguage()}&dt=t&q={Uri.EscapeDataString(text)}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string doc_json = await client.GetStringAsync(url);
                    var tab_json = JsonDocument.Parse(doc_json).RootElement[0];
                    var tab_response = tab_json[0].EnumerateArray().ToArray();
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