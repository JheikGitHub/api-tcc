using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels.ModelosDeAjuda
{
    public class MudaSenhaCpf
    {
        [Required]
        [StringLength(150, ErrorMessage = "O {0} deve ter pelo menos {2} caracteres.", MinimumLength = 7)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Cpf { get; set; }
    }

    public class MudaSenha
    {
        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova senha")]
        [Compare("NovaSenha", ErrorMessage = "A nova senha e a senha de confirmação não coincidem.")]
        public string ConfimarSenha { get; set; }
    }
}