using System;
using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels
{
    public class UsuarioViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        [Required]
        [Display(Name = "Foto 3x4")]
        public string PathFoto3x4 { get; set; }

        [Required]
        [Display(Name = "Foto de perfil")]
        public string PathFotoPerfil { get; set; }

        [Required]
        [Display(Name = "Gênero")]
        public string Genero { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }
        
        [Required]
        [Display(Name = "Data de Nascimento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime DataNascimento { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email invalido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [ScaffoldColumn(false)]
        public bool Ativo { get; set; }

        [Required]
        [Display(Name = "Perfil")]
        public string Perfil { get; set; }

    }
    
}
