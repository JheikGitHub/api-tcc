using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels.ModelosDeAjuda
{
    public class MudaSenhaCpf
    {
        [Required]
        [StringLength(14, ErrorMessage = "cpf contém 14 caracteres.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Cpf")]
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

    public class EmitiCertificado
    {
        [Required]
        [Display(Name = "Id do usuario")]
        public int UsuarioId { get; set; }

        [Required]
        [Display(Name = "Id do evento")]
        public int EventoId { get; set; }
    }
}