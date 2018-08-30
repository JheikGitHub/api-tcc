namespace KonohaApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Evento")]
    public partial class Evento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Evento()
        {
            ParticipanteEvento = new HashSet<ParticipanteEvento>();
            TopicoDiscucao = new HashSet<TopicoDiscucao>();
            Funcionario = new HashSet<Funcionario>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        [StringLength(350)]
        public string Descricao { get; set; }

        [Required]
        [StringLength(100)]
        public string Local { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataInicio { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataEncerramento { get; set; }

        [Required]
        [StringLength(300)]
        public string Apresentador { get; set; }

        [Required]
        [StringLength(50)]
        public string CargaHoraria { get; set; }

        public int NumeroVagas { get; set; }

        [Required]
        [StringLength(500)]
        public string PathImagem { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoEvento { get; set; }

        public bool Cancelado { get; set; }

        public int AgendaEventoId { get; set; }

        public virtual AgendaEvento AgendaEvento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParticipanteEvento> ParticipanteEvento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TopicoDiscucao> TopicoDiscucao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Funcionario> Funcionario { get; set; }
    }
}