using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KonohaApi.ViewModels
{
    public class ParticipanteEventoViewModel
    {
        public int Id { get; set; }

        public bool InscricaoPrevia { get; set; }

        public bool ConfirmacaoPresenca { get; set; }

        public string CodigoValidacao { get; set; }

        public int ParticipanteId { get; set; }

        public int EventoId { get; set; }
 
    }
}