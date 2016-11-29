using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Data.SqlClient;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data;
using System.Drawing;

namespace WsCotizador
{
    /// <summary>
    /// Descripción breve de Service1
    /// </summary>
    [WebService(Namespace = "http://WsCotizador.cl")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        private string query, retorno;
        Conexion conn = new Conexion();
        public SqlDataReader reader;
        ElementosGenerales elm = new ElementosGenerales();
        private SqlDataReader rider;
        private String retornoMV = "";

        private string To;
        private string Subject;
        private string Body;
        private MailMessage mail;
        private Attachment Data;

        private string[] Datos, PreDatos;
        private string usuario, pass;
        private String respuestaErro;
        SqlDataAdapter adp = new SqlDataAdapter();
        DataSet dt = new DataSet();
        DataTable tabla = new DataTable();

        //    METODOS SELECT 
        /// ///////////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public DataSet SelectCliente(String Cuenta,String Nombre,String Apellido , String IdEmpresa)
        {
            dt.Clear();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("Clientes");
            try
            {               
                adp = new SqlDataAdapter();
                if (Cuenta.Equals("") && Nombre.Equals("") && IdEmpresa.Equals("") && Apellido.Equals(""))
                {
                    tabla.Rows.Add("Hay campos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    query = "execute SelectClientes '" + Nombre + "','" + Apellido + "','" + IdEmpresa + "'";
                    String recivir = elm.sacarDB(Cuenta);
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "Clientes");
                }
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        [WebMethod]
        public DataSet SelectEmpresa(String cuenta, String Rut)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            adp = new SqlDataAdapter();
            dt = new DataSet("Empresas");
            try
            {
                if (cuenta.Equals("") || Rut.Equals(""))
                {
                    tabla.Rows.Add("Hay campos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    String recivir = elm.sacarDB(cuenta);
                    if (Rut.Equals("*"))
                    {
                        query = "EXECUTE SelectEmpresa '" + Rut + "'";
                        adp = conn.returnDataset(recivir, query);
                        adp.Fill(dt, "Empresas");
                    }
                    else
                    {
                        if (elm.validarRut(Rut))
                        {
                            query = "EXECUTE SelectEmpresa '" + Rut + "'";
                            adp = conn.returnDataset(recivir, query);
                            adp.Fill(dt, "Empresas");
                        }
                        else
                        {
                            tabla.Rows.Add("Rut no valido");
                            dt.Tables.Add(tabla);
                        }
                    }
                }               
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }           
            return dt;
           
        }
        [WebMethod]
        public DataSet SelectUsuario(String cuenta, String Usuario)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String)); 
            adp = new SqlDataAdapter();
            dt = new DataSet("Usuarios");
            try
            {
                if (cuenta.Equals("") && Usuario.Equals(""))
                {
                    tabla.Rows.Add("Hay campos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    if (Usuario.Equals("*"))
                    {
                        String recivir = elm.sacarDB(cuenta);
                        query = "execute SelectUsuario '" + Usuario + "'";
                        adp = conn.returnDataset(recivir, query);
                        adp.Fill(dt, "Usuarios");
                    }
                    else
                    {
                        String recivir = elm.sacarDB(cuenta);
                        query = "execute SelectUsuario '" + Usuario + "'";
                        adp = conn.returnDataset(recivir, query);
                        adp.Fill(dt, "Usuarios");
                    }
                }              
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
          
        }

        [WebMethod]
        public DataSet SelectOpVisitas(String cuenta)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            adp = new SqlDataAdapter();
            dt = new DataSet("OpVisitas");
            try
            {
                if (cuenta.Equals(""))
                {
                    tabla.Rows.Add("Hay campos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    query = "execute SelectOpVisitas";
                    String recivir = elm.sacarDB(cuenta);
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "OpVisitas");                    
                }

            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        [WebMethod]
        public DataSet SelectProducto(String cuenta, String IdEmpresa, String IdProducto)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            retorno = "";
            adp = new SqlDataAdapter();
            dt = new DataSet("Producto");
            try
            {
                if (cuenta.Equals("") && IdEmpresa.Equals(""))
                {
                    tabla.Rows.Add("Hay campos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    query = "execute SelectProducto '" + IdEmpresa + "','"+IdProducto+"'";
                    String recivir = elm.sacarDB(cuenta);
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "Producto");
                }
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        [WebMethod]
        public DataSet SelectCotizacion(String cuenta, String IdEvento)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("Cotizaciones");
            adp = new SqlDataAdapter();
            try
            {
                query = "EXECUTE SELECTCotizacion '" + IdEvento + "'";
                String recivir = elm.sacarDB(cuenta);
                adp = conn.returnDataset(recivir, query);
                adp.Fill(dt, "Cotizaciones");
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        [WebMethod]
        public DataSet SelectEventos(String cuenta, String IdUsuario)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            adp = new SqlDataAdapter();
            dt = new DataSet("Eventos");
            try
            {
                query = "EXECUTE SELECTEventos '" + IdUsuario + "'";
                String recivir = elm.sacarDB(cuenta);
                adp = conn.returnDataset(recivir, query);
                adp.Fill(dt, "Eventos");
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }           
            return dt;
        }
        [WebMethod]
        public DataSet SelectUbicacion(String cuenta, int ubicacion)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = null;
            adp = new SqlDataAdapter();

            try
            {
               query = "execute SelectUbicacion " + ubicacion + "";
               String recivir = elm.sacarDB(cuenta);           
               adp = conn.returnDataset(recivir, query);
            if (ubicacion == 1)
            {
                dt = new DataSet("Comuna");
                adp.Fill(dt, "Comuna");
            }
            else if (ubicacion == 2)
            {
                dt = new DataSet("Cuidad");
                adp.Fill(dt, "Cuidad");
            }
            else if (ubicacion == 3)
            {
                dt = new DataSet("Pais");
                adp.Fill(dt, "Pais");
            }
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        [WebMethod]
        public DataSet selectclientesEmpresas(String cuenta)
        {
            dt.Clear();           
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            adp = new SqlDataAdapter();
            dt = new DataSet("selectclientesEmpresas");            
            try
            {
                query = "execute selectclientesEmpresas ";
                String recivir = elm.sacarDB(cuenta);
                adp = conn.returnDataset(recivir, query);
                adp.Fill(dt, "selectclientesEmpresas");
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        [WebMethod]
        public DataSet selectcRubrosEmpresas(String cuenta)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = null;
            adp = new SqlDataAdapter();
            dt = new DataSet("selectcRubrosEmpresas");
            try
            {
                query = "execute selectcRubrosEmpresas";
                String recivir = elm.sacarDB(cuenta);
                adp = conn.returnDataset(recivir, query);
                adp.Fill(dt, "selectcRubrosEmpresas");
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
           
        }
        [WebMethod]
        public DataSet SelectTPproductos(String cuenta)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            adp = new SqlDataAdapter();
            dt = new DataSet("TPproductos");
            try
            {
                query = "EXECUTE SelectTpProducto";
                String recivir = elm.sacarDB(cuenta);
                adp = conn.returnDataset(recivir, query);
                adp.Fill(dt, "TPproductos");
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;           
        }
        
        // METODO LOGIN O INGRESO 
        /// //////////////////////////////////////////////////////////////////////////////////////////////////
      
        [WebMethod]
        public DataSet Loging(string usuario, string pass)
        {
            dt.Clear();
            tabla.Columns.Add("Error", typeof(String)); 
            try
            {
                if (usuario.Equals("") || pass.Equals(""))
                {
                    tabla.Rows.Add("Hay campos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    String recivir = elm.sacarDB(usuario);
                    query = "execute Login '" + usuario + "','" + pass + "'";                    
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "Login");
                }  
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }

        // METODO INSERT 
        /// //////////////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public DataSet AddTPproductos(string usuario, string nombre, String estado)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("TPproductos");
            adp = new SqlDataAdapter();
            try
            {
                if (nombre.Equals("") || estado.Equals(""))
                {
                    tabla.Rows.Add("Hay campos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    String recivir = elm.sacarDB(usuario);
                    query = "Execute addTPproductos '" + nombre + "','" + estado + "'";                   
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "addTPproductos");
                }
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        [WebMethod]
        public DataSet AddCliente(String usuario,String Nombre,String Apellido ,String Direccion ,String Calle ,String Movil ,String Telefono,String Rut,int IdEmpresa , String Fecha,String Hora ,String Correo ,String Contacto ,Boolean activar,int idComuna ,int idCiudad ,int idPais,int idEmpresaCleinte)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));            
            dt = new DataSet("Clientes");
            adp = new SqlDataAdapter();
            try
            {
                if (Nombre.Equals("") || Apellido.Equals("") || Direccion.Equals("") || Telefono.Equals("") || Rut.Equals("") || IdEmpresa < 0 || Fecha.Equals("") || Hora.Equals("") || Contacto.Equals("") || Calle.Equals("") || Movil.Equals("") || Correo.Equals("") || activar.Equals("") || idComuna < 0 || idCiudad < 0 || idPais < 0 || idEmpresaCleinte < 0)
                {
                    tabla.Rows.Add("Hay campos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    if (elm.validarRut(Rut))
                    {
                        String recivir = elm.sacarDB(usuario);
                        query = "execute AddCliente  '" + Nombre + "','" + Apellido + "','" + Direccion + "','" + Telefono + "','" + Rut + "'," + IdEmpresa + ",'" + Fecha + "','" + Hora + "','" + Contacto + "','" + Calle + "','" + Movil + "','" + Correo + "','" + activar + "'," + idComuna + "," + idCiudad + "," + idPais + "," + idEmpresaCleinte + "";
                        adp = conn.returnDataset(recivir, query);
                        adp.Fill(dt, "Clientes");
                    }
                    else
                    {
                        tabla.Rows.Add("El rut no es valido");
                        dt.Tables.Add(tabla);
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        [WebMethod]
        public DataSet AddEmpresa(string usuarios, string Nombre, string Direccion, string Telefono, string Rut, string logo, string Fecha, string Hora, string Fondo, string titulo,string correosupervisor)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("Empresa");
            adp = new SqlDataAdapter();
            try
            {
                if (Nombre.Equals("") || Direccion.Equals("") || Telefono.Equals("") || Rut.Equals("") || logo.Equals(""))
                {
                    tabla.Rows.Add("Hay datos vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    if (elm.validarRut(Rut))
                    {
                        String recivir = elm.sacarDB(usuarios);
                        query = "execute AddEmpresa  '" + Nombre + "','" + Direccion + "','" + Telefono + "','" + Rut + "','" + logo + "','" + Fecha + "','" + Hora + "','" + Fondo + "','" + titulo + "','" + correosupervisor + "'";
                        //retorno = "" + conn.exceuteQuery(recivir, query);
                        adp = conn.returnDataset(recivir, query);
                        adp.Fill(dt, "Empresa");
                    }
                    else
                    {
                        tabla.Rows.Add("Los datos no fueron guardados correctamente");
                        dt.Tables.Add(tabla);
                    }

                }
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
            
        }
        
        [WebMethod]
        public DataSet AddUsuario(string usuario1, string Nombre, string Apellido, string Direccion, string Telefono, string Usuario, string Contraseña, string IdEmpresa, string Hora, string Fecha, string estadoSuper)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("Usuario");
            adp = new SqlDataAdapter();
            try
            {
               if (Nombre.Equals("") || Apellido.Equals("") || Direccion.Equals("") || Telefono.Equals("") || Usuario.Equals("") || Contraseña.Equals("") || IdEmpresa.Equals("") || Hora.Equals("") || Fecha.Equals("") || estadoSuper.Equals(""))
            {                
                tabla.Rows.Add("Alguno de los valores estan vacios");
                dt.Tables.Add(tabla);
            }
            else
            {
                String recivir = elm.sacarDB(usuario1);
                query = "execute AddUsuario '" + Nombre + "','" + Apellido + "','" + Direccion + "','" + Telefono + "','" + Usuario + "','" + Contraseña + "'," + IdEmpresa + ",'" + Hora + "','" + Fecha + "'," + estadoSuper + "";
                //retorno = "" + conn.exceuteQuery(recivir, query);
                adp = conn.returnDataset(recivir, query);
                adp.Fill(dt, "Usuario");
            }

            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
           
        }
        [WebMethod]
        public DataSet AddOpVisita(string user, string Nombre, Boolean activar)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("Visita");
            adp = new SqlDataAdapter();
            try
            {
                if (Nombre.Equals("") && user.Equals(""))
                {                    
                    tabla.Rows.Add("Alguno de los valores estan vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    String recivir = elm.sacarDB(user);
                    query = "execute AddOpVisita '" + Nombre + "'," + activar + "";
                    //retorno = "" + conn.exceuteQuery(recivir, query);
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "Visita");
                }              
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;           
        }
        
        [WebMethod]
        public DataSet AddEvento(String usuario, int IdCliente, int IdUsuario, String DesVisita, int IdOpVisita, String fecha, String Hora)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            adp = new SqlDataAdapter();
            dt = new DataSet("Evento");
            try
            {
                if (IdCliente.Equals("") || IdUsuario.Equals("") || DesVisita.Equals("") || IdOpVisita.Equals("") || fecha.Equals("") || Hora.Equals(""))
                {
                    tabla.Rows.Add("Alguno de los valores estan vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    String recivir = elm.sacarDB(usuario);
                    query = "execute AddEvento " + IdCliente + "," + IdUsuario + ",'" + DesVisita + "'," + IdOpVisita + ",'" + fecha + "','" + Hora + "'";
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "Producto");
                    //retorno = "" + conn.exceuteQuery(recivir, query);
                }

            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;
                     
        }
       
        [WebMethod]
        public DataSet AddCotizacion(string usuario, string idProducto, string Fecha, string Hora, string IdEvento, string ValorCT)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("Cotizacion");
            adp = new SqlDataAdapter();
            try
            {
                if (idProducto.Equals("") || Fecha.Equals("") || Hora.Equals(""))
                {
                    tabla.Rows.Add("Alguno de los valores estan vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    String recivir = elm.sacarDB(usuario);
                    query = "Execute AddCotizacion " + idProducto + ",'" + Fecha + "','" + Hora + "'," + IdEvento + "," + ValorCT + "";
                    //retorno = "" + conn.exceuteQuery(recivir, query);
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "Cotizacion");
                }

            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;            
        }
        [WebMethod]
        public DataSet AddProdu(string usuario, string Nombre, string Descripcion, string Precio, string IdEmpresa, string Fechacreacion, string FechaPrecio, String foto, Boolean Activar, String TipoProducto)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("Producto");
            adp = new SqlDataAdapter();
            try
            {
                if (Nombre.Equals("") || Descripcion.Equals("") || Precio.Equals("") || IdEmpresa.Equals("") || Fechacreacion.Equals("") || FechaPrecio.Equals("") || TipoProducto.Equals(""))
                {
                    tabla.Rows.Add("Alguno de los valores estan vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    int count = foto.Length;
                    String recivir = elm.sacarDB(usuario);
                    query = "execute AddProductos '" + Nombre + "','" + Descripcion + "','" + Precio + "','" + IdEmpresa + "','" + Fechacreacion + "','" + FechaPrecio + "','" + foto + "','" + Activar + "','" + TipoProducto + "'";
                    //retorno = "" + conn.exceuteQuery(recivir, query);
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "Producto");
                }

            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;            
        }
         // updates 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
        [WebMethod]
        public DataSet UpdateOpVisitas(String Cuenta, String NombreNuevo, Boolean activo, String nombre)
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("UpdateOpVisitas");
            adp = new SqlDataAdapter();          
            try
            {
                if (Cuenta.Equals("") && NombreNuevo.Equals("") && nombre.Equals(""))
                {
                    tabla.Rows.Add("Alguno de los valores estan vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    query = "execute UpdateOpVisitas '" + NombreNuevo + "'," + activo + ",'" + nombre + "'";
                    String recivir = elm.sacarDB(Cuenta);
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "UpdateOpVisitas");
                }
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }           
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;           
        }
         [WebMethod]
         public DataSet UPDATECliente(String Cuenta,String Nombre ,String Apellido ,String Direccion,String Calle ,String Movil ,String Telefono ,String Rut ,int IdEmpresa ,String Fecha ,String Hora ,String Correo ,String Contacto ,Boolean activar,int idComuna,int idCiudad ,int idPais,int idEmpresaCleinte )
        {
            dt.Clear();
            tabla = new DataTable();
            tabla.Columns.Add("Error", typeof(String));
            dt = new DataSet("UpdateOpVisitas");
            adp = new SqlDataAdapter();           
            try
            {
                if (Cuenta.Equals("") && Nombre.Equals("") && Direccion.Equals("") && Telefono.Equals("") && Rut.Equals("") && IdEmpresa.Equals("") && Fecha.Equals("") && Hora.Equals("") && Contacto.Equals(""))
                {
                    tabla.Rows.Add("Alguno de los valores estan vacios");
                    dt.Tables.Add(tabla);
                }
                else
                {
                    query = "execute UPDATECliente '"+Nombre+"','"+Apellido+"','"+Direccion+"','"+Calle+"','"+Movil+"','"+Telefono+"','"+Rut+"',"+IdEmpresa+",'"+Fecha+"','"+Hora+"','"+Correo+"','"+Contacto+"',"+activar+","+idComuna+","+idCiudad+","+idPais+","+idEmpresaCleinte+"";
                    String recivir = elm.sacarDB(Cuenta);
                    adp = conn.returnDataset(recivir, query);
                    adp.Fill(dt, "UpdateOpVisitas");
                }
            }
            catch (NullReferenceException ex)
            {
                tabla.Rows.Add("El usuario no es correcto");
                dt.Tables.Add(tabla);
            }
            catch (ArgumentException ex1)
            {
                tabla.Rows.Add(ex1.ToString());
                dt.Tables.Add(tabla);
            }            
            return dt;            
        }
         [WebMethod]
         public DataSet updateEmpresa(String Cuenta,String Nombre,String Direccion,String Telefono,String Rut,String Fecha,String Hora,String titulo,String logo,String Fondo)
         {
             dt.Clear(); 
             tabla = new DataTable();
             tabla.Columns.Add("Respuesta", typeof(String));
             DataRow fila = tabla.NewRow();
             dt = new DataSet("UpdateOpVisitas");
             adp = new SqlDataAdapter();
            
             try
             {
                 if (Cuenta.Equals("") && Nombre.Equals("") && Direccion.Equals("") && Telefono.Equals("") && Rut.Equals("") && Fecha.Equals("") && Hora.Equals("") && logo.Equals("") && Fondo.Equals(""))
                 {
                     tabla.Rows.Add("Alguno de los valores estan vacios");
                     dt.Tables.Add(tabla);
                 }
                 else
                 {
                     query = "execute updateEmpresa '" + Nombre + "','" + Direccion + "','" + Telefono + "','" + Rut + "','" + Fecha + "','" + Hora + "','" + titulo + "','" + logo + "','" + Fondo + "'";
                     String recivir = elm.sacarDB(Cuenta);
                     adp = conn.returnDataset(recivir, query);
                     adp.Fill(dt, "UpdateOpVisitas");
                 }
             }
             catch (NullReferenceException ex)
             {
                 tabla.Rows.Add("El usuario no es correcto");
                 dt.Tables.Add(tabla);
             }
             catch (ArgumentException ex1)
             {
                 tabla.Rows.Add(ex1.ToString());
                 dt.Tables.Add(tabla);
             }             
             return dt;             
         }
         [WebMethod]
         public DataSet UpdateProductos(String Cuenta,String Nombre,String TipoProducto,String Descripcion,String Precio,String IdEmpresa,String Fechacreacion ,String FechaPrecio,String Foto, Boolean  bit,int idproducto)
         {
             dt.Clear();
             tabla = new DataTable();
             tabla.Columns.Add("Respuesta", typeof(String));
             //DataRow fila = tabla.NewRow();
             dt = new DataSet("UpdateOpVisitas");
             adp = new SqlDataAdapter();             
             try
             {
                 if (Cuenta.Equals("") && Nombre.Equals("") &&  TipoProducto.Equals("") && Descripcion.Equals("") &&  Precio.Equals("") && IdEmpresa.Equals("") &&  Fechacreacion.Equals("") && FechaPrecio.Equals("") && Foto.Equals("") && idproducto>1)
                 {
                     tabla.Rows.Add("Alguno de los valores estan vacios");
                     dt.Tables.Add(tabla);
                 }
                 else
                 {
                     query = "execute UpdateProductos '"+Nombre+"','"+TipoProducto+"','"+Descripcion+"','"+Precio+"','"+IdEmpresa+"','"+Fechacreacion+"','"+FechaPrecio+"','"+Foto+"',"+bit+",'"+idproducto+"'";
                     String recivir = elm.sacarDB(Cuenta);
                     adp = conn.returnDataset(recivir, query);
                     adp.Fill(dt, "UpdateOpVisitas");
                 }
             }
             catch (NullReferenceException ex)
             {
                 tabla.Rows.Add("El usuario no es correcto");
                 dt.Tables.Add(tabla);
             }
             catch (ArgumentException ex1)
             {
                 tabla.Rows.Add(ex1.ToString());
                 dt.Tables.Add(tabla);
             }             
             return dt;
             
         }
         [WebMethod]
         public DataSet updateUsuario(String Cuenta, String Nombre, String Apellido, String Direccion, String Telefono, String Usuario, String Contraseña, String IdEmpresa, String Hora, String Fecha, String estadoSuper, String idusuario)
         {
             dt.Clear();
             dt = new DataSet();
             tabla = new DataTable();
             tabla.Columns.Add("Respuesta", typeof(String));
             DataRow fila = tabla.NewRow();             
             adp = new SqlDataAdapter();            
             try
             {
                 if (Cuenta.Equals("") && Nombre.Equals("") && Apellido.Equals("") &&  Direccion.Equals("") && Telefono.Equals("") && Usuario.Equals("") && Contraseña.Equals("") && IdEmpresa.Equals("") &&  Hora.Equals("") &&  Fecha.Equals("") &&  estadoSuper.Equals("") &&  idusuario.Equals("") )
                 {
                     tabla.Rows.Add("Alguno de los valores estan vacios");
                     dt.Tables.Add(tabla);
                 }
                 else
                 {
                     query = " execute updateUsuario '" + Nombre + "','" + Apellido + "','" + Direccion + "','" + Telefono + "','" + Usuario + "','" + Contraseña + "','" + IdEmpresa + "','" + Hora + "','" + Fecha + "','" + estadoSuper + "','" + idusuario + "'";
                     String recivir = elm.sacarDB(Cuenta);
                     adp = conn.returnDataset(recivir, query);
                     adp.Fill(dt, "UpdateOpVisitas");
                 }
             }
             catch (NullReferenceException ex)
             {
                 tabla.Rows.Add("El usuario no es correcto");
                 dt.Tables.Add(tabla);
             }
             catch (ArgumentException ex1)
             {
                 tabla.Rows.Add(ex1.ToString());
                 dt.Tables.Add(tabla);
             }             
             return dt;             
         }

        
        // ELIMINACION 
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////       
         [WebMethod]
         public DataSet DeleteTablas(String Cuenta, int tabla2, int idTabla, int estado)
         {
             dt.Clear();
             dt = new DataSet("UpdateOpVisitas");
             adp = new SqlDataAdapter();
             String Nomtabla = "";
             tabla = new DataTable();
             tabla.Columns.Add("Respuesta", typeof(String));
             try
             {
                 if (Cuenta.Equals("") && tabla2 > 0 && idTabla > 0)
                 {
                     tabla.Rows.Add("Alguno de los valores estan vacios");
                     dt.Tables.Add(tabla);
                 }
                 else
                 {
                     if (tabla2 == 1)
                     {
                         Nomtabla = "EXEC	DeleteCliente '" + idTabla + "','" + estado + "'";
                     }
                     else if (tabla2 == 2)
                     {
                         Nomtabla = "EXEC	DeleteEmpresa '" + idTabla + "','" + estado + "'";
                     }
                     else if (tabla2 == 3)
                     {
                         Nomtabla = "EXEC	DeleteOpVisitas '" + idTabla + "','" + estado + "'";
                     }
                     else if (tabla2 == 4)
                     {
                         Nomtabla = "EXEC	DeleteProducto '" + idTabla + "','" + estado + "'";
                     }
                     else if (tabla2 == 5)
                     {
                         Nomtabla = "EXEC	DeleteUsuario '" + idTabla + "','" + estado + "'";
                     }
                     query = Nomtabla;
                     String recivir = elm.sacarDB(Cuenta);
                     adp = conn.returnDataset(recivir, query);
                     adp.Fill(dt, "UpdateOpVisitas");
                 }
             }
             catch (NullReferenceException ex)
             {
                 tabla.Rows.Add("El usuario no es correcto");
                 dt.Tables.Add(tabla);
             }
             catch (ArgumentException ex1)
             {
                 tabla.Rows.Add(ex1.ToString());
                 dt.Tables.Add(tabla);
             }             
             return dt;             
         }

         // EXTRAS 
         /// ////////////////////////////////////////////////////////////////////////////////////////////////////   
        [WebMethod]
         public DataSet enviar(String Correo, String detalleVisita, String cadena, String nomEmpresa, String vendedor, String Nomcliente, String FonoCliente, String Direccioncliente, String mailCliente, String mg, String numCT,String correoCopia)
         {
             dt.Clear();
             dt = new DataSet();
             tabla = new DataTable();
             tabla.Columns.Add("Respuesta", typeof(String));
             DataRow fila = tabla.NewRow();
             String res = null;
            
             if (cadena.Equals(""))
             {
                  fila["Respuesta"] = "No hay datos para realizar el envio...";
             }
             else
             {
                 try
                 {

                     StreamReader file = new StreamReader("C:\\Web Services Tpco\\imagenes\\cotizacion\\DatosWsWinsor\\RutaDe archivo Csv.txt");
                     string ruta = file.ReadToEnd();
                     file.Dispose();
                     pdf(nomEmpresa, cadena, vendedor, detalleVisita, Nomcliente, FonoCliente, Direccioncliente, mailCliente, mg, numCT);
                  
                     file = new StreamReader("C:\\DatosWsWinsor\\DatosDestinatario.txt");
                     To = file.ReadToEnd();
                     file.Dispose();
                     PreDatos = To.Split('-');
                     To = Correo;
                     Subject = "Cotizacion: " + numCT + ",Empresa: " + nomEmpresa + " Fecha: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ", Realizada por vendedor: " + vendedor;
                     Body ="Adjunto enviamos "+detalleVisita+" solicitada.\n Agradecemos su preferencia y estamos a su disposición para concretarla y/o lo que requiera en relación a nuestros servicios/productos. \n Slds \n Vendedor: "+vendedor+" \n Empresa : "+nomEmpresa+""; 
                     MailMessage mail = new MailMessage();
                     mail.To.Add(new MailAddress(this.To));
                     file = new StreamReader("C:\\DatosWsWinsor\\Correo y Usuario.txt");
                     usuario = file.ReadToEnd();
                     file.Dispose();
                     Datos = usuario.Split(';');
                     usuario = Datos[0];
                     pass = Datos[1];
                     mail.From = new MailAddress(usuario);
                     mail.Subject = Subject;
                     mail.Body = Body;
                     mail.CC.Add(correoCopia);
                     mail.IsBodyHtml = false;
                     if (!(ruta.Trim() == ""))
                     {
                         Data = new Attachment(ruta, MediaTypeNames.Application.Octet);
                         mail.Attachments.Add(Data);
                         
                     }
                     file = new StreamReader("C:\\DatosWsWinsor\\NombreHost.txt");
                     SmtpClient client = new SmtpClient(file.ReadToEnd());
                     file.Dispose();
                     client.Port = 25;
                     client.Credentials = new System.Net.NetworkCredential(usuario, pass);
                     client.Send(mail);
                     fila["Respuesta"] = "Mail enviado";
                     Data.Dispose();
                     mail.Dispose();
                     
                 }
                 catch (WebException ex)
                 {
                     fila["Respuesta"] = ex.ToString();
                 }
                 catch (InvalidOperationException ex2)
                 {
                     fila["Respuesta"] = "El nombre del Host es incorrecto: " + ex2.ToString();
                 }
                 catch (FormatException ex3)
                 {
                     fila["Respuesta"] = "Deber haber un destinatario: " + ex3.ToString();
                 }
                 catch (NullReferenceException ex4)
                 {
                     fila["Respuesta"] = "El usuario no es correcto: " + ex4.ToString();
                 }
                 catch (SmtpException ex5)
                 {
                     fila["Respuesta"] = "El Correo o pass no es correcto: " + ex5.ToString();
                 }
             }
             tabla.Rows.Add(fila);
             dt.Tables.Add(tabla);             
             return dt;
         }
         public void pdf(String nomEmpresa, String cadena, String vendedor, String Asunto, String Nomcliente, String FonoCliente,String Direccioncliente,String mailCliente,String img,String numCT)
         {

             int calculo=0;
             int fuente = 8;
             // Creamos el documento con el tamaño de página tradicional
             Document doc = new Document(PageSize.LETTER);
             
              //Indicamos donde vamos a guardar el documento
               PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream("C:\\Web Services Tpco\\imagenes\\cotizacion\\Cotizacion.pdf", FileMode.Create));            
               



             // PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Propuestas\\Cotizador\\Cotizacion.pdf", FileMode.Create));
             string R = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "Cotizacion.pdf";
             
             // Le colocamos el título y el autor
             // **Nota: Esto no será visible en el documento
             doc.AddTitle("jose marcelo ");
             doc.AddCreator("Yasna Castillos");
             // Abrimos el archivo
             doc.Open();
             // Escribimos el encabezamiento en el documento             
             iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, fuente, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

             imagen1(img);
             iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance("C:\\Web Services Tpco\\imagenes\\abc.Jpeg");
             imagen.BorderWidth = 0;
             imagen.Alignment = Element.ALIGN_RIGHT;
             float percentage = 0.0f;
             percentage = 150 / imagen.Width;
             imagen.ScalePercent(percentage * 100);

             // Insertamos la imagen en el documento
             doc.Add(imagen);
             
             
             Paragraph p1 = new Paragraph();
             p1.Alignment = Element.ALIGN_RIGHT;
             Paragraph p2 = new Paragraph();
             p2.Alignment = Element.ALIGN_RIGHT;

             Chunk chunk2 = new Chunk("Cotizacion:" + numCT + " ", FontFactory.GetFont("ARIAL", fuente, PdfContentByte.ALIGN_CENTER));
             Chunk chunk3 = new Chunk("Fecha de cotizacion:" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "", FontFactory.GetFont("ARIAL", fuente, PdfContentByte.ALIGN_CENTER));
             
             p1.Add(chunk2);
             p2.Add(chunk3);
             doc.Add(p1);
             doc.Add(p2);
             doc.Add(new Paragraph("\n"));            
             
             doc.Add(new Paragraph("Nombre : " + Nomcliente + "", FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT)));
             doc.Add(new Paragraph("Empresa: " + nomEmpresa + "", FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT)));
             doc.Add(new Paragraph("Fonos : " + FonoCliente + "", FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT)));
             doc.Add(new Paragraph("Direccion: " + Direccioncliente + "", FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT))); 
             doc.Add(new Paragraph("Email: "+mailCliente+ "", FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT))); 
             doc.Add(new Paragraph("Referencia: "+Asunto+ "", FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT)));
             doc.Add(new Paragraph("Vendedor: " + vendedor + "", FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT))); 
             doc.Add(Chunk.NEWLINE);
             // Creamos una tabla que contendrá el nombre, apellido y país 
             // de nuestros visitante.
             PdfPTable tblPrueba = new PdfPTable(5);
             tblPrueba.WidthPercentage = 100;
             // Configuramos el título de las columnas de la tabla
             PdfPCell cltipo = new PdfPCell(new Phrase("Tipo", _standardFont));
             cltipo.BorderWidth = 0;
             cltipo.BorderWidthBottom = 0.25f;
             PdfPCell clNombre = new PdfPCell(new Phrase("Nombre", _standardFont));
             clNombre.BorderWidth = 0;
             clNombre.BorderWidthBottom = 0.25f;
             PdfPCell clPHora = new PdfPCell(new Phrase("Precio hora $", _standardFont));
             clPHora.BorderWidth = 0;
             clPHora.BorderWidthBottom = 0.25f;
             PdfPCell cltotal = new PdfPCell(new Phrase("Total $", _standardFont));
             cltotal.BorderWidth = 0;
             cltotal.BorderWidthBottom = 0.25f;
             PdfPCell cltiempo = new PdfPCell(new Phrase("Tiempo", _standardFont));
             cltiempo.BorderWidth = 0;
             cltiempo.BorderWidthBottom = 0.25f;
             // Añadimos las celdas a la tabla
             tblPrueba.AddCell(cltipo);
             tblPrueba.AddCell(clNombre);
             tblPrueba.AddCell(clPHora);
             tblPrueba.AddCell(cltotal);
             tblPrueba.AddCell(cltiempo);
             String[] datosPre, datos = cadena.Split('~');
             int pdatos = 0;

             for (int i = 0; i <= datos.Length - 2; i++)
             {
                 datosPre = datos[i].Split(';');
                 // Configuramos el título de las columnas de la tabla
                 cltipo = new PdfPCell(new Phrase(datosPre[0], _standardFont));
                 cltipo.BorderWidth = 0;
                 clNombre = new PdfPCell(new Phrase(datosPre[1], _standardFont));
                 clNombre.BorderWidth = 0;
                 pdatos = int.Parse(datosPre[2].ToString());
                 clPHora = new PdfPCell(new Phrase(pdatos.ToString("N0"), _standardFont));
                 clPHora.BorderWidth = 0;
                 pdatos = int.Parse(datosPre[4].ToString());
                 cltotal = new PdfPCell(new Phrase(pdatos.ToString("N0"), _standardFont));
                 cltotal.BorderWidth = 0;
                 calculo = calculo + int.Parse(datosPre[4].ToString());
                 cltiempo = new PdfPCell(new Phrase(datosPre[3], _standardFont));
                 cltiempo.BorderWidth = 0;
                 tblPrueba.AddCell(cltipo);
                 tblPrueba.AddCell(clNombre);
                 tblPrueba.AddCell(clPHora);
                 tblPrueba.AddCell(cltotal);
                 tblPrueba.AddCell(cltiempo);
             }
             doc.Add(tblPrueba);
             int neto, iva;
             neto = (calculo / 100) * 81;
             iva = (calculo / 100) * 19;
             doc.Add(new Paragraph("\n")); 
             doc.Add(new Paragraph("\n"));
             Paragraph p4 = new Paragraph();
             p4.Alignment = Element.ALIGN_RIGHT;
             Paragraph p5 = new Paragraph();
             p5.Alignment = Element.ALIGN_RIGHT;
             Paragraph p6 = new Paragraph();
             p6.Alignment = Element.ALIGN_RIGHT;
             Chunk chunk4 = new Chunk("Neto: " + neto.ToString("N0") + "", FontFactory.GetFont("ARIAL", fuente, PdfContentByte.ALIGN_CENTER));
             Chunk chunk5 = new Chunk("19% IVA: " + iva.ToString("N0") + "", FontFactory.GetFont("ARIAL", fuente, PdfContentByte.ALIGN_CENTER));
             Chunk chunk6 = new Chunk("Total: " + calculo.ToString("N0") + "", FontFactory.GetFont("ARIAL", fuente, PdfContentByte.ALIGN_CENTER));
             p4.Add(chunk4);
             p5.Add(chunk5);
             p6.Add(chunk6);
             doc.Add(p4);
             doc.Add(p5);
             doc.Add(p6);
             doc.Add(new Paragraph("\n"));
             doc.Add(new Paragraph("Validez: 30 dias",FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT)));
             doc.Add(new Paragraph("Forma de pago: 30,60,90 dias cheque adjunto.", FontFactory.GetFont("ARIAL", fuente, iTextSharp.text.Element.ALIGN_RIGHT)));             
             doc.Add(Chunk.NEWLINE);
             doc.Close();
            writer.Close();
         }
        
        private void imagen1(String img){
            string imagen = img;
            byte[] imageBytes = Convert.FromBase64String(imagen);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
            imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            Bitmap bit = new Bitmap(250, 190);
            Graphics g = Graphics.FromImage(bit);
            g.DrawImage(image, new Point(0, 0));
            string ruta = "C:\\Web Services Tpco\\imagenes\\";
            bit.Save(ruta + "abc.Jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);           
       
    }  
        
     public System.Drawing.Image stringToImage(string inputString)
    {
        byte[] imageBytes = Convert.FromBase64String(inputString);
        MemoryStream ms = new MemoryStream(imageBytes);
        System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true, true);
        return image;
    }



     // GENERICOS 
     /// ////////////////////////////////////////////////////////////////////////////////////////////////////   

     [WebMethod]
     public DataSet rubroEmpresasService(String cuenta,int servicio ,String Nomrubro ,Boolean activo ,int idRubro )
     {
         dt.Clear();
         tabla = new DataTable();
         tabla.Columns.Add("Error", typeof(String));
         adp = new SqlDataAdapter();
         dt = new DataSet("rubroEmpresasService");
         try
         {
             if (cuenta.Equals(""))
             {
                 tabla.Rows.Add("Hay campos vacios");
                 dt.Tables.Add(tabla);
             }
             else
             {
                 query = "execute rubroEmpresasService "+servicio+",'"+Nomrubro+"',"+activo+","+idRubro+"";
                 String recivir = elm.sacarDB(cuenta);
                 adp = conn.returnDataset(recivir, query);
                 adp.Fill(dt, "rubroEmpresasService");
             }
         }
         catch (NullReferenceException ex)
         {
             tabla.Rows.Add("El usuario no es correcto");
             dt.Tables.Add(tabla);
         }
         catch (ArgumentException ex1)
         {
             tabla.Rows.Add(ex1.ToString());
             dt.Tables.Add(tabla);
         }
         return dt;
     }

     [WebMethod]
     public DataSet tipoProductosService(String cuenta, int servicio,String  TipoProducto,Boolean  estado,int idTpProducto)
     {
         dt.Clear();
         tabla = new DataTable();
         tabla.Columns.Add("Error", typeof(String));
         adp = new SqlDataAdapter();
         dt = new DataSet("tipoProductosService");
         try
         {
             if (cuenta.Equals(""))
             {
                 tabla.Rows.Add("Hay campos vacios");
                 dt.Tables.Add(tabla);
             }
             else
             {
                 query = "execute tipoProductosService " + servicio + ",'" + TipoProducto + "'," + estado + "," + idTpProducto + "";
                 String recivir = elm.sacarDB(cuenta);
                 adp = conn.returnDataset(recivir, query);
                 adp.Fill(dt, "tipoProductosService");
             }
         }
         catch (NullReferenceException ex)
         {
             tabla.Rows.Add("El usuario no es correcto");
             dt.Tables.Add(tabla);
         }
         catch (ArgumentException ex1)
         {
             tabla.Rows.Add(ex1.ToString());
             dt.Tables.Add(tabla);
         }
         return dt;
     }

     [WebMethod]
     public DataSet paisService(String cuenta, int servicio, String nomPais, Boolean estado, int idPais)
     {
         dt.Clear();
         tabla = new DataTable();
         tabla.Columns.Add("Error", typeof(String));
         adp = new SqlDataAdapter();
         dt = new DataSet("paisService");
         try
         {
             if (cuenta.Equals(""))
             {
                 tabla.Rows.Add("Hay campos vacios");
                 dt.Tables.Add(tabla);
             }
             else
             {
                 query = "execute paisService " + servicio + ",'" + nomPais + "'," + estado + "," + idPais + "";
                 String recivir = elm.sacarDB(cuenta);
                 adp = conn.returnDataset(recivir, query);
                 adp.Fill(dt, "paisService");
             }
         }
         catch (NullReferenceException ex)
         {
             tabla.Rows.Add("El usuario no es correcto");
             dt.Tables.Add(tabla);
         }
         catch (ArgumentException ex1)
         {
             tabla.Rows.Add(ex1.ToString());
             dt.Tables.Add(tabla);
         }
         return dt;
     }


     [WebMethod]
     public DataSet EmpresasClientesService(String cuenta, int servicio, String NombreEmp, Boolean activo, int idRubro, int  idEmpresa)
     {
         dt.Clear();
         tabla = new DataTable();
         tabla.Columns.Add("Error", typeof(String));
         adp = new SqlDataAdapter();
         dt = new DataSet("EmpresasClientesService");
         try
         {
             if (cuenta.Equals(""))
             {
                 tabla.Rows.Add("Hay campos vacios");
                 dt.Tables.Add(tabla);
             }
             else
             {
                 query = "execute EmpresasClientesService " + servicio + ",'" + NombreEmp + "'," + activo + "," + idRubro + "," + idEmpresa + "";
                 String recivir = elm.sacarDB(cuenta);
                 adp = conn.returnDataset(recivir, query);
                 adp.Fill(dt, "EmpresasClientesService");
             }
         }
         catch (NullReferenceException ex)
         {
             tabla.Rows.Add("El usuario no es correcto");
             dt.Tables.Add(tabla);
         }
         catch (ArgumentException ex1)
         {
             tabla.Rows.Add(ex1.ToString());
             dt.Tables.Add(tabla);
         }
         return dt;
     }

     [WebMethod]
     public DataSet CuidadService(String cuenta, int servicio, String nomCiudad, Boolean activo, int idPais, int  idCiudad)
     {
         dt.Clear();
         tabla = new DataTable();
         tabla.Columns.Add("Error", typeof(String));
         adp = new SqlDataAdapter();
         dt = new DataSet("CuidadService");
         try
         {
             if (cuenta.Equals(""))
             {
                 tabla.Rows.Add("Hay campos vacios");
                 dt.Tables.Add(tabla);
             }
             else
             {
                 query = "execute CuidadService " + servicio + ",'" + nomCiudad + "'," + activo + "," + idPais + "," + idCiudad + "";
                 String recivir = elm.sacarDB(cuenta);
                 adp = conn.returnDataset(recivir, query);
                 adp.Fill(dt, "CuidadService");
             }
         }
         catch (NullReferenceException ex)
         {
             tabla.Rows.Add("El usuario no es correcto");
             dt.Tables.Add(tabla);
         }
         catch (ArgumentException ex1)
         {
             tabla.Rows.Add(ex1.ToString());
             dt.Tables.Add(tabla);
         }
         return dt;
     } 

         [WebMethod]
     public DataSet ComunaService(String cuenta, int servicio, String nomComuna,Boolean activo ,int idCiudad ,int idcomuna )
     {
         dt.Clear();
         tabla = new DataTable();
         tabla.Columns.Add("Error", typeof(String));
         adp = new SqlDataAdapter();
         dt = new DataSet("ComunaService");
         try
         {
             if (cuenta.Equals(""))
             {
                 tabla.Rows.Add("Hay campos vacios");
                 dt.Tables.Add(tabla);
             }
             else
             {
                 query = "execute ComunaService " + servicio + ",'" + nomComuna + "'," + activo + "," + idCiudad + "," + idcomuna + "";
                 String recivir = elm.sacarDB(cuenta);
                 adp = conn.returnDataset(recivir, query);
                 adp.Fill(dt, "ComunaService");
             }
         }
         catch (NullReferenceException ex)
         {
             tabla.Rows.Add("El usuario no es correcto");
             dt.Tables.Add(tabla);
         }
         catch (ArgumentException ex1)
         {
             tabla.Rows.Add(ex1.ToString());
             dt.Tables.Add(tabla);
         }
         return dt;
     } 
    }
}
    
