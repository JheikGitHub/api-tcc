using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels
{
    public class ComentarioViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Comentario")]
        public string Texto { get; set; }

        [Required]
        [Display(Name = "Id topico discuçao")]
        public int TopicoIDiscucaoId { get; set; }

       
        [Display(Name = "Id usuario")]
        public int UsuarioId { get; set; }
        
        [Display(Name = "Comentario filho")]
        public int? ParentId { get; set; }
    }
}