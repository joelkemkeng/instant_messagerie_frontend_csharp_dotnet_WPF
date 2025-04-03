// HETIC - Main entry point for Hetic-Stream application
using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Runtime.InteropServices;

namespace HeticStream.UI
{
    internal class Program
    {
        // Main entry point for the application.
        public static void Main(string[] args)
        {
            bool headlessMode = Array.IndexOf(args, "--headless") >= 0 || 
                               Environment.GetEnvironmentVariable("HEADLESS") == "true";

            try
            {
                if (headlessMode)
                {
                    Console.WriteLine("Démarrage en mode serveur (headless)...");
                    // En mode headless, on lance juste les services principaux et on attend
                    StartHeadlessMode();
                }
                else
                {
                    Console.WriteLine("Démarrage en mode GUI...");
                    BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur au démarrage: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                
                if (!headlessMode && ex.Message.Contains("XOpenDisplay failed"))
                {
                    Console.WriteLine("Pour exécuter en mode GUI, vous devez connecter un affichage X11.");
                    Console.WriteLine("Consultez le fichier DOCKER_README.md pour plus d'informations.");
                    Console.WriteLine("Passage en mode serveur (headless)...");
                    
                    // Si erreur X11, passer automatiquement en mode headless
                    StartHeadlessMode();
                }
            }
        }

        // Mode headless pour les serveurs sans interface graphique
        private static void StartHeadlessMode()
        {
            Console.WriteLine("HeticStream est en cours d'exécution en mode serveur (sans interface graphique).");
            Console.WriteLine("Les services sont disponibles mais aucune interface graphique n'est affichée.");
            Console.WriteLine("Appuyez sur Ctrl+C pour quitter.");
            
            // Boucle principale qui maintient l'application en vie
            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                exitEvent.Set();
            };
            
            // Attendre indéfiniment jusqu'à ce que Ctrl+C soit pressé
            exitEvent.WaitOne();
        }

        // Avalonia configuration, used for platform specific settings.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}