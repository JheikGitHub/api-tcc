using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels
{
    public class ParticipanteViewModel 
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Matricula")]
        public string Matricula { get; set; }

        [Required]
        [Display(Name = "Codigo carteira estudantil")]
        public string CodCarteirinha { get; set; }
        
        [Required]
        public bool IsAluno { get; set; }

        public UsuarioViewModel Usuario { get; set; }
    }
}