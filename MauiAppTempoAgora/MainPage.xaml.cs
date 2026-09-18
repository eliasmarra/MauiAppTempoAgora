using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

       private async void Button_Clicked(object sender, EventArgs e)
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
    }
}
