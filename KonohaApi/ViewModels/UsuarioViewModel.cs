﻿using System;
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


        [Display(Name = "Foto de perfil")]
        public string PathFotoPerfil { get; set; }

        [Required]
        [Display(Name = "Gênero")]
        public string Genero { get; set; }

        [Required]
        [Display(Name = "Data de Nascimento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime DataNascimento { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email invalido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "UserName")]
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
