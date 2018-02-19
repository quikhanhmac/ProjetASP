using GrandHotel_APITesteur_PF.Pages;
using Outils.TConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GrandHotel_APITesteur_PF
{
    class Program
    {

            static void Main(string[] args)
            {
                GrandHotelApp app = GrandHotelApp.Instance;
                //app.Title = "GrandHotel";

                // Ajout des pages
                Page accueil = new PageAccueil();
                app.AddPage(accueil);
                app.AddPage(new PageGestionClients());
                //app.AddPage(new PageGestionFactures());
                //app.AddPage(new PageResultatsHotel());

                // Affichage de la page d'accueil
                app.NavigateTo(accueil);

                // Lancement de l'appli
                app.Run();
            }

    }
}
