namespace KonohaApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comentario")]
    public partial class Comentario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comentario()
        {
            Comentario1 = new HashSet<Comentario>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Texto { get; set; }

        public int TopicoId { get; set; }

        public int UsuarioId { get; set; }

        public int? ParentId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comentario> Comentario1 { get; set; }

        public virtual Comentario Comentario2 { get; set; }

        public virtual TopicoDiscucao TopicoDiscucao { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
