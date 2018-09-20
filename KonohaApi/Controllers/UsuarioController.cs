using KonohaApi.DAO;
using KonohaApi.ViewModels;
using KonohaApi.ViewModels.ModelosDeAjuda;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using static KonohaApi.ViewModels.MandaEmail;

namespace KonohaApi.Controllers
{
    [RoutePrefix("api/usuario")]
    public class UsuarioController : ApiController
    {
        private UsuarioDAO _dao;
        public UsuarioDAO DAO
        {
            get
            {
                if (_dao == null)
                    _dao = new UsuarioDAO();
                return _dao;
            }
            set => _dao = value;
        }

        [Authorize]
        [HttpGet]
        [Route("acess-user")]
        public IHttpActionResult ObterUsuario()
        {
            var acessUser = DAO.GetUser(User.Identity.Name);
            return Ok(acessUser);
        }

        [HttpGet]
        [Route("busca-todos")]
        public IHttpActionResult BuscaTodos()
        {
            var usuarios = DAO.ListaTodos();
            return Ok(usuarios);
        }

        [HttpGet]
        [Route("busca-por-id/{id:int}")]
        public IHttpActionResult BuscaPorId(int id)
        {
            var usuarioViewModel = DAO.BuscaPorId(id);

            if (usuarioViewModel == null)
                return NotFound();

            return Ok(usuarioViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("adiciona")]
        public IHttpActionResult Salvar(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.Adicionar(model);

            if (resultado.Equals("OK"))
            {
                return StatusCode(HttpStatusCode.Created);
            }
            else
                return BadRequest(resultado);
        }

        [HttpPut]
        [Route("altera/{id:int}")]
        public IHttpActionResult Editar(int id, UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
            {
                return BadRequest();
            }

            string resultado = DAO.Editar(model);

            if (resultado.Equals("OK"))
                return StatusCode(HttpStatusCode.NoContent);
            else
                return BadRequest(resultado);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("remove/{id:int}")]
        public IHttpActionResult Remove(int id)
        {
            if (id < 0)
                return BadRequest("Indice negativo.");

            string resultado = DAO.Remove(id);

            if (resultado.Equals("OK"))
                return Ok();

            return BadRequest(resultado);
        }

        [HttpGet]
        [Route("desativar-usuario/{id:int}")]
        public IHttpActionResult DesativaUsuario(int id)
        {
            if (id < 0)
                return BadRequest("Indice negativo.");

            string resultado = DAO.DesativaUsuario(id);
            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);
        }

        [HttpGet]
        [Route("ativar-usuario/{id:int}")]
        public IHttpActionResult AtivaUsuario(int id)
        {
            if (id < 0)
                return BadRequest("Indice negativo.");

            string resultado = DAO.AtivaUsuario(id);
            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);
        }

        [HttpPost]
        [Route("email")]
        public IHttpActionResult EmailMudaSenha(MudaSenhaCpf model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = DAO.BuscaPorCpf(model.Cpf);

            if (usuario == null)
                return NotFound();

            //coloca a url da pagina de alteração de senha 
            var mesagem = new Uri("http://localhost:4200/recupera-senha/" + usuario.Id);

            GmailEmailService gmail = new GmailEmailService();
            EmailMessage msg = new EmailMessage
            {
                Body = "<html><head> </head> <body>  <form method='POST'> <p> Click no link <a href=" + mesagem + ">Link </a> para recuperação da senha. </p> </form></body> </html>",
                IsHtml = true,
                Subject = "Recuperação de senha",
                ToEmail = usuario.Email
            };
            try
            {
                bool result = gmail.SendEmailMessage(msg);
                if (result)
                    return Ok();

                return BadRequest("Não foi possivel altera a senha, por favor tente mais tarde.");
            }
            catch (Exception)
            {
                return InternalServerError();
            }

        }

        [HttpPut]
        [Route("recupera-senha/{id:int}")]
        public IHttpActionResult RecuperaSenha(int id, MudaSenha model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id < 0)
                return BadRequest("Indice negativo.");

            string resultado = DAO.MudaSenha(id, model.NovaSenha);
            if (resultado.Equals("OK"))
                return Ok("Senha alterada com sucesso!");
            else
                return BadRequest(resultado);
        }

        [HttpPost]
        [Route("inscricao")]
        public IHttpActionResult Inscricao(InscricaoParticipanteEvento inscricao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.InscricaoEvento(inscricao);
            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);
        }

        [HttpPost]
        [Route("cancelar-inscricao")]
        public IHttpActionResult CancelarInscricao(InscricaoParticipanteEvento inscricao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.CancelaInscricaoEvento(inscricao);

            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);
        }

        [HttpGet]
        [Route("eventos-inscrito/{id:int}")]
        public IHttpActionResult EventosInscrito(int id)
        {
            if (id < 0)
                return BadRequest("Não e valido valores negativos!");
            try
            {
                var lista = DAO.ListaEventosInscritos(id);
                return Ok(lista);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("eventos-com-presenca-confirmada/{id:int}")]
        public IHttpActionResult EventoConfirmacaoPresenca(int id)
        {
            if (id < 0)
                return BadRequest("Não e valido valores negativos!");
            try
            {
                var lista = DAO.ListaEventosComCorfimacaoPresenca(id);
                return Ok(lista);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("gerar-certificado")]
        public HttpResponseMessage GerarCertificado(EmitiCertificado certificado)
        {

            if (!ModelState.IsValid)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            try
            {
                string caminho = DAO.GerarPDF(certificado);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                FileStream fileStream = File.OpenRead(caminho);
                response.Content = new StreamContent(fileStream);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DAO.Dispose();
            }
            base.Dispose(disposing);

        }
    }
}
