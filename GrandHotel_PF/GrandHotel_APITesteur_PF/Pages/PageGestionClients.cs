using Outils.TConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GrandHotel_APITesteur_PF.Pages
{
    public class PageGestionClients : MenuPage
    {
        private static HttpClient client;


        public PageGestionClients() : base("Clients")
        {
            Menu.AddOption("1", "Afficher la liste des Clients", AfficherClients);
            Menu.AddOption("2", "Récupérer les infos d'un client identifié par son id", ClientInfo);
            Menu.AddOption("3", "Rechercher des clients dont le nom contient un texte donné", RechercherClients);
            Menu.AddOption("4", "Créer un nouveau client", CreerClient);
        }

        // Afficher la liste des clients
        private void AfficherClients()
        {

            using (client = new HttpClient())
            {
                List<Client> clients = RunAsyncListeClients().Result;
                foreach (Client cl in clients)
                {
                    Console.WriteLine("Id: " + cl.Id + " Nom: " + cl.Nom + " Prenom: " + cl.Prenom + " Email:" + cl.Email + "\n");
                }
            }
        }

        // Afficher la liste des clients
        private void ClientInfo()
        {
            int id = Input.Read<int>("Id Client : ");

            string url = "http://localhost:52119/api/Clients/" + id;


            using (client = new HttpClient())
            {
                Client cl = RunAsyncClient(url).Result;
                Console.WriteLine("\n" + "Id: " + cl.Id + "\n" + "Nom: " + cl.Nom + "\n" + "Prenom: " + cl.Prenom + "\n" + "Email:" + cl.Email + "\n"); //  + "Adresse:" + cl.Adresse + "\n"); // + "No Telephone: " + cl.Telephone + "\n");
            }

        }

        // Afficher la liste des clients
        private void RechercherClients()
        {
            string texte = Input.Read<string>("Texte (au minimum trois lettres) : ");

            string url = "http://localhost:52119/api/Clients/Nom/" + texte;
            using (client = new HttpClient())
            {
                List<Client> clients = RunAsyncClientsRecherches(url).Result;
                foreach (Client cl in clients)
                {
                    Console.WriteLine("Id: " + cl.Id + " Nom: " + cl.Nom + " Prenom: " + cl.Prenom + " Email:" + cl.Email + "\n");
                }
            }
        }

        // Afficher la liste des clients
        private void CreerClient()
        {
            Client c = new Client();

            c.Civilite = Input.Read<string>("Civilité ( M ou Mme ou Mlle) : ");
            c.Nom = Input.Read<string>("Nom : ");
            c.Prenom = Input.Read<string>("Prenom : ");
            c.Email = Input.Read<string>("Email : ");
            c.CarteFidelite = Input.Read<bool>("Carte de Fidelité (true ou false) : ");
            c.Societe = Input.Read<string>("Société : ");

            //string url = "http://localhost:52119/api/Clients/" + id.ToString();

            // RunAsyncCreateClient(c);

            using (client = new HttpClient())
            {
                // Modifier le port selon les besoins
                client.BaseAddress = new Uri("http://localhost:52119/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = RunAsyncCreateClient(c).Result;
                Console.WriteLine($"Client créé à l'url {url}");
                Console.WriteLine("\n");
            }

        }


        private static async Task<List<Client>> RunAsyncListeClients()
        {
            // Modifier le port selon les besoins
            client.BaseAddress = new Uri("http://localhost:52119/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<Client> clientsTemp = new List<Client>();
            try
            {
                var response = await client.GetAsync("http://localhost:52119/api/Clients");
                //$"api/Clients");
                if (response.IsSuccessStatusCode)
                {
                    clientsTemp = await response.Content.ReadAsAsync<List<Client>>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return clientsTemp;
            //Console.ReadLine();

        }


        private static async Task<Client> RunAsyncClient(string url)
        {
            // Modifier le port selon les besoins
            client.BaseAddress = new Uri("http://localhost:52119/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client clientTemp = new Client();
            try
            {
                var response = await client.GetAsync(url);
                //$"api/Clients");
                if (response.IsSuccessStatusCode)
                {
                    clientTemp = await response.Content.ReadAsAsync<Client>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return clientTemp;
            //Console.ReadLine();

        }

        private static async Task<List<Client>> RunAsyncClientsRecherches(string url)
        {
            // Modifier le port selon les besoins
            client.BaseAddress = new Uri("http://localhost:52119/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<Client> clientsTemp = new List<Client>();
            try
            {
                var response = await client.GetAsync(url);
                //$"api/Clients");
                if (response.IsSuccessStatusCode)
                {
                    clientsTemp = await response.Content.ReadAsAsync<List<Client>>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return clientsTemp;
            //Console.ReadLine();

        }

        private static async Task<Uri> RunAsyncCreateClient(Client c)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/Clients", c);
            response.EnsureSuccessStatusCode();

            // retourne l'uri de la ressource créée
            return response.Headers.Location;
        }


    }
}
