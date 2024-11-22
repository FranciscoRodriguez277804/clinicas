namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;

public record Documentos
{
    public string ID { get; set; }
    public string TipoDocumento { get; set; }
    public string ArchivoDocumento { get; set; }

    public Documentos() { }
    public Documentos(string Tipo, string ArchivoDocumento)
    {
        this.TipoDocumento = Tipo;
        this.ArchivoDocumento = ArchivoDocumento;
    }
    public void validarDatos()
    {
        try
        {
            if (this.TipoDocumento == "")
            {
                throw new Exception("Ingrese un nombre o descripcion a su documento");
            }
            if (this.ArchivoDocumento == null || this.ArchivoDocumento.Length == 0)
            {
                throw new Exception("Ingrese un documento.");
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}

