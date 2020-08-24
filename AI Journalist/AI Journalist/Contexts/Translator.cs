using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Unicode;

namespace AI_Journalist.Contexts
{
    class Translator
    {
        Settings.ContextsNode.TranslatorNode Settings;

        public Translator(Settings.ContextsNode.TranslatorNode settings)
        {
            Settings = settings;
        }

        public void AddContext(Context context)
        {
            if (string.IsNullOrEmpty(context.Source.Caption))
                return;

            context.IsCaptionKorean = IsKorean(context.Source.Caption);
            if (!context.IsCaptionKorean)
                return;

            context.TranslatedCaption = GetTranslation(context.Source.Caption);
        }

        public string GetTranslation(string source)
        {
            var request = new HttpRequestMessage {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://openapi.naver.com/v1/papago/n2mt"),
                Headers = {
                    { "X-Naver-Client-Id", Settings.ClientId },
                    { "X-Naver-Client-Secret", Settings.ClientSecret }
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    { "source", "ko" },
                    { "target", "en" },
                    { "text", source }
                })
            };
            var response = new Internet().RequestText(request);
            dynamic parsed = JsonConvert.DeserializeObject(response);
            return parsed.message.result.translatedText;
        }

        public bool IsKorean(string text)
        {
            foreach (var c in text) {
                if (IsCharInRange(c, UnicodeRanges.HangulJamo))
                    return true;
                if (IsCharInRange(c, UnicodeRanges.HangulSyllables))
                    return true;
            }
            return false;
        }

        public bool IsCharInRange(char c, UnicodeRange range)
        {
            if (c < range.FirstCodePoint)
                return false;
            if (c >= range.FirstCodePoint + range.Length)
                return false;
            return true;
        }
    }
}
