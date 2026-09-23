using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

     private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await DisplayAlertAsync("Sem conexão", "Você está sem acesso à internet. Verifique sua rede e tente novamente.", "OK");
                    return;
                }

                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Temp Máx: {t.temp_max} \n" +
                                         $"Temp Min: {t.temp_min} \n" +
                                         $"Descrição: {t.description} \n" +
                                         $"Vento: {t.speed} m/s \n" +
                                         $"Visibilidade: {t.visibility} m \n";

                        lbl_res.Text = dados_previsao;

                        string mapa = $"<html><body style='margin:0'>" +
                                      $"<iframe width=\"100%\" height=\"100%\" frameborder=\"0\" src=\"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}\">" +
                                      $"</iframe></body></html>";

                        wv_mapa.Source = new HtmlWebViewSource { Html = mapa };

                    }
                    else
                    {
                        lbl_res.Text = "Sem dados de previsão.";
                    }

                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }

            } catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(
                        GeolocationAccuracy.Medium,
                        TimeSpan.FromSeconds(10)
                    );

                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if(local != null)
                {
                    string local_disp = $"Latitude: {local.Latitude} \n" +
                                        $"Longitude: {local.Longitude} \n";

                    lbl_coords.Text = local_disp;

                    GetCidade(local.Latitude, local.Longitude);

                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlertAsync("Erro: Dispositivo não suporta.", fnsEx.Message, "OK");
            }  
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlertAsync("Erro: Localização Desabilitada.", fneEx.Message, "OK");
            }
            catch (PermissionException peEx)
            {
                await DisplayAlertAsync("Erro: Permissão da Localização negada.", peEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erro", ex.Message, "OK");
            }
        }

        private async void GetCidade(double lat, double lon)
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);

                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }

            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erro: Obtenção de cidade falhou", ex.Message, "OK");
            }
        }
    }
}
