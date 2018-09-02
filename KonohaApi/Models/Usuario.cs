namespace KonohaApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Usuario")]
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            Comentario = new HashSet<Comentario>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(14)]
        public string Cpf { get; set; }
        
        [Required]
        [StringLength(500)]
        public string PathFotoPerfil { get; set; }

        [Required]
        [StringLength(1)]
        public string Genero { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime DataNascimento { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Senha { get; set; }

        public bool Ativo { get; set; }

        [Required]
        [StringLength(20)]
        public string Perfil { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comentario> Comentario { get; set; }

        public virtual Funcionario Funcionario { get; set; }

        public virtual Participante Participante { get; set; }
    }
}
