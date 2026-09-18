using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "a6fe51fbec199cd485ff2da0d1314ac1";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                          $"q={cidade}&units=metric&appid={chave}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp;

                try
                {
                    resp = await client.GetAsync(url);
                }
                catch (HttpRequestException)
                {
                    throw new Exception("Não foi possível conectar ao servidor. Verifique sua conexão com a internet.");
                }

                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"Cidade \"{cidade}\" não encontrada. Verifique o nome digitado.");
                }

                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro ao consultar o servidor (código {(int)resp.StatusCode}).");
                }

                string json = await resp.Content.ReadAsStringAsync();

                var rascunho = JObject.Parse(json);

                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime sunrise = epoch.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                DateTime sunset = epoch.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                t = new()
                {
                    lat = (double)rascunho["coord"]["lat"],
                    lon = (double)rascunho["coord"]["lon"],
                    description = (string)rascunho["weather"][0]["description"],
                    main = (string)rascunho["weather"][0]["main"],
                    temp_max = (double)rascunho["main"]["temp_max"],
                    temp_min = (double)rascunho["main"]["temp_min"],
                    speed = (double)rascunho["wind"]["speed"],
                    visibility = (int)rascunho["visibility"],
                    sunrise = sunrise.ToString(),
                    sunset = sunset.ToString(),
                };
            }

            return t;
        }
    }
}