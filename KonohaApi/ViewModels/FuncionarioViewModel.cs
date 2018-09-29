using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels
{
    public class FuncionarioViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Administrador")]
        public bool IsAdmin { get; set; }

        [Required]
        [Display(Name = "Permissao para criar agenda")]
        public bool PermissaoCriarAgenda { get; set; }

        public UsuarioViewModel Usuario { get; set; }

    }
}