namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public class ConfirmacionPagoViewModel
    {
        public string Mensaje { get; set; }
        public string FilePath { get; set; }
        public string PdfBase64 { get; set; }

        public Pagos pago { get; set; }
    }
}
