using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio
{
    public class Documento
    {
        public Guid DocumentoId { get; set; }
        public Guid ObjetoReferencia { get; set; }
        public String Nombre { get; set; }
        public String Extension { get; set; }
        public byte[] Contenido { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
