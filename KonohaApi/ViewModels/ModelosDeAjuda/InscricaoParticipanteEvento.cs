using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels
{
    public class InscricaoParticipanteEvento
    {
        [Required]
        [Display(Name = "Id do Participante")]
        public int ParticipanteId { get; set; }

        [Required]
        [Display(Name ="Id do evento")]
        public int EventoId { get; set; }
    }

    public class GeraCertificado
    {
        [Required]
        [Display(Name = "Id do Participante")]
        public int ParticipanteId { get; set; }

        [Required]
        [Display(Name = "Id do evento")]
        public int EventoId { get; set; }
    }

    public class ConfimacaoParticipanteEvento
    {
        [Required]
        [Display(Name = "Id do Participante")]
        public int UsuarioId { get; set; }

        [Required]
        [Display(Name = "Id do evento")]
        public int EventoId { get; set; }

    }
}