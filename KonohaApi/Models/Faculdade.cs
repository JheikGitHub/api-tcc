namespace KonohaApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Faculdade")]
    public partial class Faculdade
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Faculdade()
        {
            AgendaEvento = new HashSet<AgendaEvento>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(14)]
        public string Telefone { get; set; }

        [Required]
        [StringLength(150)]
        public string Endereco { get; set; }

        [Required]
        [StringLength(9)]
        public string Cep { get; set; }

        [Required]
        [StringLength(18)]
        public string Cnpj { get; set; }

        public int CidadeId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AgendaEvento> AgendaEvento { get; set; }

        public virtual Cidade Cidade { get; set; }
    }
}
