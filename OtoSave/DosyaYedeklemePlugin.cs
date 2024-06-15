using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace OtoSave
{
    public class DosyaYedeklemePlugin : RocketPlugin<Config>
    {
        private string _kaynakKlasor;
        private Timer _timer;

        public override void LoadPlugin()
        {
            _kaynakKlasor = Configuration.Instance.DosyaYolu;
            Logger.Log("Dosya Yedekleme Plugin'i başarıyla yüklendi!", ConsoleColor.Yellow);
            int zaman = Configuration.Instance.Süre;
            TimeSpan interval = TimeSpan.FromSeconds(zaman); 
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += (sender, e) => Yedekle(_kaynakKlasor);
            _timer.Start();
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            _timer?.Stop();
            _timer?.Dispose();

            Logger.Log("Dosya Yedekleme Plugin'i kaldırıldı.", ConsoleColor.Yellow);
        }

        private void Yedekle(string kaynakKlasor)
        {
            string masaustuYedeklerKlasoru = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Yedekler");
            System.IO.Directory.CreateDirectory(masaustuYedeklerKlasoru);

            string bugununTarihiKlasoru = Path.Combine(masaustuYedeklerKlasoru, DateTime.Today.ToString("dd.MM.yyyy"));
            System.IO.Directory.CreateDirectory(bugununTarihiKlasoru);

            try
            {
                if (!System.IO.Directory.Exists(kaynakKlasor))
                {
                    Logger.LogError($"Hata: {kaynakKlasor} adında bir klasör bulunamadı.");
                    return;
                }
                YedekleKlasor(kaynakKlasor, bugununTarihiKlasoru);

                Logger.Log("Yedekleme başarıyla tamamlandı.", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Hata oluştu: {ex.Message}");
            }
        }

        private void YedekleKlasor(string kaynakKlasor, string hedefKlasor)
        {
            foreach (string dosyaYolu in System.IO.Directory.GetFiles(kaynakKlasor, "*", SearchOption.AllDirectories))
            {
                string hedefDosyaYolu = dosyaYolu.Replace(kaynakKlasor, hedefKlasor);
                string hedefKlasorAdi = Path.GetDirectoryName(hedefDosyaYolu);

                System.IO.Directory.CreateDirectory(hedefKlasorAdi);

                File.Copy(dosyaYolu, hedefDosyaYolu, true);
            }
        }
    }
}
