namespace KonohaApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ParticipanteEvento")]
    public partial class ParticipanteEvento
    {
        public int Id { get; set; }

        public bool InscricaoPrevia { get; set; }

        public bool ConfirmacaoPresenca { get; set; }

        [Required]
        [StringLength(13)]
        public string CodigoValidacao { get; set; }

        public int ParticipanteId { get; set; }

        public int EventoId { get; set; }

        public virtual Evento Evento { get; set; }

        public virtual Participante Participante { get; set; }
    }
}
