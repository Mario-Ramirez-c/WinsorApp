using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WsCotizador
{
    public class Conexion
    {
        private SqlConnection conex = null;
        private SqlDataReader rider;
        private string strconex = null;
        private SqlCommand comando = null;
        private int respuesta;
        private SqlDataAdapter adaptador;


        private Boolean conextar(String db)
        {
            try
            {    //Data Source=DESARROLLO-3\SQLEXPRESS;Initial Catalog=Simmarent;Integrated Security=True
                //strconex = "Data Source=DESARROLLO-2\\DESARROLLO2;Initial Catalog=" + db + ";Integrated Security=True";
                strconex = "Data Source=DESARROLLO-3\\SQLEXPRESS;Initial Catalog=Simmarent;Integrated Security=True";
                conex = new SqlConnection(strconex);
            }
            catch (SqlException ex)
            {
                return false;
            }
            finally{
                conex.Open();
            }
            return true;

        }

       /* public int exceuteQuery(string db, string query)
        {
            conextar(db);
            comando = new SqlCommand(query, conex);
            return respuesta = comando.ExecuteNonQuery();

        }*/

       /* public SqlDataReader QueryResult(string db, string query)
        {
            if (conextar(db))
            {
                try
                {
                    comando = new SqlCommand(query, conex);
                }
                catch (SqlException sql)
                {
                    rider = null;
                }

            }
            else
            {
                rider = null;
            }
            return rider = comando.ExecuteReader();

        }*/

        public SqlDataAdapter returnDataset(string db, string query)
        {

            if (conextar(db))
            {
                try
                {
                    comando = new SqlCommand(query, conex);
                    adaptador = new SqlDataAdapter(comando);
                    comando.Cancel();
                    conex.Close();
                }
                catch (SqlException sql)
                {
                    
                }
            }
            else
            {
               
            }
            return adaptador;
        }
    }
}