namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Pagos
    {

        public string id_cobro { get; set; }
        public DateTime fecha_pago { get; set; }
        public DateTime fecha_consulta { get; set; }
        public string id_consulta { get; set; }
        public string Hora_inicio { get; set; }
        public string Hora_Fin { get; set; }
        public string nro_factura { get; set; }
        public string factura { get; set; }
        public string comprobante { get; set; }
        public string convenio { get; set; }
        public DateTime insertdate { get; set; }
        public string estado { get; set; }
        public string estado_conciliado { get; set; }
        public string nombre_Empleado { get; set; }
        public string nombre_Usuario { get; set; }
        public TipoPago Tipo { get; set; } = new TipoPago();
        public int Importe { get; set; }
        public string sucursalSesion { get; set; }
        public string userEmail { get; set; }
        public string idUsuario { get; set; }


        public Pagos(string id_cobro, DateTime fecha_pago, DateTime fechaConsulta, string nro_factura, string factura, string comprobante, string convenio, DateTime insertdate,
            string estado, string estado_conciliado, TipoPago tipo, int importe, string userEmail, string idUsuario)
        {
            this.id_cobro = id_cobro;
            this.fecha_pago = fecha_pago;
            this.fecha_consulta = fechaConsulta;
            this.nro_factura = nro_factura;
            this.factura = factura;
            this.comprobante = comprobante;
            this.convenio = convenio;
            this.insertdate = insertdate;
            this.estado = estado;
            this.estado_conciliado = estado_conciliado;
            Tipo = tipo;
            Importe = importe;
            this.userEmail = userEmail;
            this.idUsuario = idUsuario;
        }
        public Pagos() { }

        public Pagos(string id_cobro, DateTime fecha_pago, DateTime fecha_consulta, string hora_inicio, string hora_Fin, string nro_factura, string factura, string comprobante, string convenio, DateTime insertdate, string estado, string estado_conciliado, string nombre_Empleado, string nombre_Usuario, TipoPago tipo, int importe, string sucursalSesion)
        {
            this.id_cobro = id_cobro;
            this.fecha_pago = fecha_pago;
            this.fecha_consulta = fecha_consulta;
            this.Hora_inicio = hora_inicio;
            this.Hora_Fin = hora_Fin;
            this.nro_factura = nro_factura;
            this.factura = factura;
            this.comprobante = comprobante;
            this.convenio = convenio;
            this.insertdate = insertdate;
            this.estado = estado;
            this.estado_conciliado = estado_conciliado;
            this.nombre_Empleado = nombre_Empleado;
            this.nombre_Usuario = nombre_Usuario;
            Tipo = tipo;
            Importe = importe;
            this.sucursalSesion = sucursalSesion;
        }


        public void validarDatos()
        {
            try
            {
                if (fecha_pago > DateTime.Today.Date)
                {
                    throw new Exception("La fecha no puede ser mayor que hoy.");
                }
                if (userEmail == null)
                {
                    throw new Exception("Ingrese un usuario registrante valido");
                }
                if (Tipo == null)
                {
                    throw new Exception("Ingrese un tipo de pago valido");
                }
                if (Importe < 0)
                {
                    throw new Exception("Ingrese un importe valido");
                }
                if (nombre_Usuario == null)
                {
                    throw new Exception("Ingrese un usuario valido");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}


