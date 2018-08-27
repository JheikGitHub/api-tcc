namespace KonohaApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AgendaEvento")]
    public partial class AgendaEvento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AgendaEvento()
        {
            Evento = new HashSet<Evento>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(350)]
        public string Descricao { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DateEncerramento { get; set; }

        [Required]
        [StringLength(500)]
        public string PathImagem { get; set; }

        public int FaculdadeId { get; set; }

        public int FuncionarioId { get; set; }

        public virtual Faculdade Faculdade { get; set; }

        public virtual Funcionario Funcionario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evento> Evento { get; set; }
    }
}
