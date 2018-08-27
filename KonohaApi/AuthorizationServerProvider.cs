using KonohaApi.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace KonohaApi
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials
          (OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Conytol-Allow-Origin", new[] { "*" });
            try
            {
                DataContext dataContext = new DataContext();
                // encontrando o usuário

                var usuario = dataContext.Usuario
                    .FirstOrDefault(x => x.UserName == context.UserName
                                    && x.Senha == context.Password);

                if (usuario == null)
                {
                    context.SetError("invalid_grant",
                        "Usuário não encontrado ou senha incorreta.");
                    return;
                }

                // emitindo o token com informacoes extras
                // se o usuário existe
                var identidadeUsuario = new ClaimsIdentity(context.Options.AuthenticationType);

                identidadeUsuario.AddClaim(new Claim(ClaimTypes.Name, usuario.UserName));

                var funcao = dataContext.Funcao.Find(usuario.FuncaoId);

                identidadeUsuario.AddClaim(new Claim(ClaimTypes.Role, funcao.Funcao1));

                var roles = new List<string>
                {
                    funcao.Funcao1
                };

                GenericPrincipal principal = new GenericPrincipal(identidadeUsuario, roles.ToArray());

                Thread.CurrentPrincipal = principal;

                context.Validated(identidadeUsuario);
            }
            catch (Exception)
            {
                context.SetError("invalid_grant", "Falha ao autenticar");
            }

        }
    }
}