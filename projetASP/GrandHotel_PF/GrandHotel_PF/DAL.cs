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


        public static List<Chambre> GetChambre(double NbNuits, DateTime Jour, byte NbPersonnes)
        {
            List<Chambre> chambres = new List<Chambre>();

            DateTime DateArrivee = Jour;
            //double jours = System.Convert.ToDouble(NbNuits);
            DateTime DateDepart = Jour.AddDays(NbNuits);

            //string req1 = @"select distinct C.Numero, C.NbLits
            //                from Chambre C
            //                left outer join Reservation R on R.NumChambre = C.Numero
            //                where Jour!=@Jour and c.NbLits=@NbPersonnes";

            string req = @"select c.Numero, c.NbLits
                           from Reservation r
                           inner join Chambre c on c.Numero = r.NumChambre
                           where c.NbLits = @NbPersonnes
                           group by c.Numero, c.NbLits
                           except
                           select c.Numero, c.NbLits
                           from Reservation r
                           inner join Chambre c on c.Numero = r.NumChambre
                           where r.Jour between @DateArrivee and @DateDepart
                           group by c.Numero, c.NbLits";


            var param1 = new SqlParameter("@DateArrivee", SqlDbType.Date);
            param1.Value = DateArrivee;
            var param2 = new SqlParameter("@DateDepart", SqlDbType.Date);
            param2.Value = DateDepart;
            var param3 = new SqlParameter("@NbPersonnes", SqlDbType.TinyInt);
            param3.Value = NbPersonnes;



            using (var conn = new SqlConnection(GrandHotelConnect))
            {
                var cmd = new SqlCommand(req, conn);
                cmd.Parameters.Add(param1);
                cmd.Parameters.Add(param2);
                cmd.Parameters.Add(param3);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //double.tryparse(char.valeurmoy, out moy)
                        //totalenergie += (double)reader["valeurmoy"];
                        var c = new Chambre();
                        c.Numero = (short)reader["Numero"];
                        c.NbLits = (byte)reader["NbLits"];
                        chambres.Add(c);
                    }
                }
            }
            return chambres;
        }
    }
}

