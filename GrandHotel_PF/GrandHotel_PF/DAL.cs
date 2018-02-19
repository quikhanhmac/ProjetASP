using GrandHotel_PF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GrandHotel_PF
{
    public static class DAL
    {
        private static string GrandHotelConnect = @"Server=(localdb)\mssqllocaldb;Database=GrandHotel;Trusted_Connection=True;";


        public static List<ChambreVM> GetChambre(double NbNuits, DateTime Jour, byte NbPersonnes)
        {
            List<ChambreVM> chambres = new List<ChambreVM>();

            DateTime DateArrivee = Jour;
            DateTime DernierJour = Jour.AddDays(NbNuits - 1);
            // Le dernier jour de la réservation est le jour avant le départ car la nuit de la date de départ la chambre devrait être libre

            // The list of rooms available for the requested dates deduced by subtraction
            string req = @"select c.Numero, c.NbLits, c.Bain, c.Douche, c.Etage, c.WC, t.Prix, t.Prix*@NbNuits PrixTotal
                           from Reservation r
                           inner join Chambre c on c.Numero = r.NumChambre
                           inner join TarifChambre tc on tc.NumChambre=c.Numero
                           inner join Tarif t on t.Code=tc.CodeTarif
                           where c.NbLits = @NbPersonnes and YEAR(t.DateDebut)=YEAR(@DateArrivee)
                           group by c.Numero, c.NbLits, c.Bain, c.Douche, c.Etage, c.WC, t.Prix, t.Prix*@NbNuits, YEAR(t.DateDebut)
                           except
                           select c.Numero, c.NbLits, c.Bain, c.Douche, c.Etage, c.WC, t.Prix, t.Prix*@NbNuits PrixTotal
                           from Reservation r
                           inner join Chambre c on c.Numero = r.NumChambre
                           inner join TarifChambre tc on tc.NumChambre=c.Numero
                           inner join Tarif t on t.Code=tc.CodeTarif
                           where r.Jour between @DateArrivee and @DernierJour
                           group by c.Numero, c.NbLits, c.Bain, c.Douche, c.Etage, c.WC, t.Prix, t.Prix*@NbNuits";

            var param0 = new SqlParameter("@NbNuits", SqlDbType.Int);
            param0.Value = NbNuits;
            var param1 = new SqlParameter("@DateArrivee", SqlDbType.Date);
            param1.Value = DateArrivee;
            var param2 = new SqlParameter("@DernierJour", SqlDbType.Date);
            param2.Value = DernierJour;
            var param3 = new SqlParameter("@NbPersonnes", SqlDbType.TinyInt);
            param3.Value = NbPersonnes;




            using (var conn = new SqlConnection(GrandHotelConnect))
            {
                var cmd = new SqlCommand(req, conn);
                cmd.Parameters.Add(param0);
                cmd.Parameters.Add(param1);
                cmd.Parameters.Add(param2);
                cmd.Parameters.Add(param3);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cVM = new ChambreVM(); // We had to create a ViewModel to add the property PrixTotal
                        cVM.Chambre = new Chambre();
                        cVM.Chambre.Numero = (short)reader["Numero"];
                        cVM.Chambre.Etage = (byte)reader["Etage"];
                        cVM.Chambre.NbLits = (byte)reader["NbLits"];
                        cVM.Chambre.Bain = (bool)reader["Bain"];
                        cVM.Chambre.Douche = (bool)reader["Douche"];
                        cVM.Chambre.Wc = (bool)reader["Wc"];
                        cVM.Prix = (decimal)reader["Prix"];
                        cVM.PrixTotal = (decimal)reader["PrixTotal"];
                        chambres.Add(cVM);
                    }
                }
            }
            return chambres;
        }
    }
}

