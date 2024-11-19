using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public class FileMetadata
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public bool IsFolder { get; set; } // Para identificar si es una carpeta
        public string MimeType { get; set; } // Para identificar el tipo de archiv

        public FileMetadata() { }
    }
}
