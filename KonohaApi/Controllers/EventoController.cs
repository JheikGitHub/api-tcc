using KonohaApi.DAO;
using KonohaApi.ViewModels;
using KonohaApi.ViewModels.ModelosDeAjuda;
using System;
using System.Net;
using System.Web.Http;

namespace KonohaApi.Controllers
{
    [RoutePrefix("api/evento")]
    public class EventoController : ApiController
    {
        private EventoDAO _dao;

        public EventoDAO DAO
        {
            get
            {
                if (_dao == null)
                    _dao = new EventoDAO();
                return _dao;
            }
            set => _dao = value;
        }

        [HttpGet]
        [Route("busca-todos")]
        public IHttpActionResult BuscaTodos()
        {
            var eventos = DAO.ListaTodos();
            return Ok(eventos);
        }

        [HttpGet]
        [Route("busca-por-id/{id:int}")]
        public IHttpActionResult BuscaPorId(int id)
        {
            var eventoViewModel = DAO.BuscaPorId(id);

            if (eventoViewModel == null)
                return NotFound();

            return Ok(eventoViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("adiciona")]
        public IHttpActionResult Salvar(EventoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.Adicionar(model);

            if (resultado.Equals("OK"))
                return StatusCode(HttpStatusCode.Created);
            else
                return BadRequest(resultado);
        }

        [HttpPut]
        [Route("altera/{id:int}")]
        public IHttpActionResult Editar(int id, EventoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return BadRequest();

            string resultado = DAO.Editar(model);
            if (resultado.Equals("OK"))
                return StatusCode(HttpStatusCode.NoContent);

            return BadRequest(resultado);

        }

        [HttpDelete]
        //  [Authorize(Roles = "Admin")]
        [Route("remove/{id:int}")]
        public IHttpActionResult Remove(int id)
        {
            string resultado = DAO.Remove(id);

            if (resultado.Equals("OK"))
                return Ok();

            return BadRequest(resultado);

        }

        [HttpGet]
        [Route("pesquisa-eventos/{id}")]
        public IHttpActionResult PesquisaEventos(string id)
        {
            var eventos = DAO.FiltraEventos(id);

            return Ok(eventos);
        }

        [HttpPost]
        [Route("cancelar-evento")]
        public IHttpActionResult CancelarEvento(MotivoCancelamentoEvento motivo)
        {
            if (!ModelState.IsValid)
                return BadRequest("Não e valido valores negativos!");

            string resultado = DAO.CancelarEvento(motivo);

            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);

        }

        [HttpPost]
        [Route("alteracoes-evento")]
        public IHttpActionResult ModificacaoEvento(MotivoCancelamentoEvento motivo)
        {
            if (!ModelState.IsValid)
                return BadRequest("Não e valido valores negativos!");
            try
            {
                bool resultado = DAO.AlteraçoesEvento(motivo);
                if (resultado)
                    return Ok();
                else
                    return BadRequest("Não foi possivel realizar essa operação, Por favor tente mais tarde.");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("participantes-inscritos-no-evento/{id:int}")]
        public IHttpActionResult ParticipantesIncritosNoEvento(int id)
        {
            if (id < 0)
                return BadRequest("Não e valido valores negativos!");

            var lista = DAO.ListaParticipanteDoEventos(id);
            return Ok(lista);
        }

        [HttpGet]
        [Route("numeros-de-inscritos/{id:int}")]
        public IHttpActionResult NumerosInscritos(int id)
        {
            if (id < 0)
                return BadRequest("Não e valido valores negativos!");

            var lista = DAO.ListaParticipanteDoEventos(id);
            var evento = DAO.BuscaPorId(id);

            int inscritos = evento.NumeroVagas - lista.Count;
            return Ok(inscritos);
        }

        [HttpPost]
        [Route("confimacao-de-presenca-no-evento")]
        public IHttpActionResult ConfimacaoPresencaEvento(ConfimacaoParticipanteEvento confimacao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.ConfimacaoPresenca(confimacao);

            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);

        }

        [HttpPost]
        [Route("cancelar-confimacao-de-presenca-no-evento")]
        public IHttpActionResult CancelarConfimacaoPresencaEvento(ConfimacaoParticipanteEvento confimacao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.CancelarConfimacaoPresenca(confimacao);

            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);

        }

        [HttpGet]
        [Route("busca-eventos-moderador/{id:int}")]
        public IHttpActionResult EventoModerador(int id)
        {
            if (id < 0)
                return BadRequest("Indice negativo");

            var eventos = DAO.EventosModerador(id);

            return Ok(eventos);
        }
        [HttpGet]
        [Route("todos-moderadores/{id:int}")]
        public IHttpActionResult Moderadores(int id)
        {
            if (id < 0)
                return BadRequest("Indice negativo");

            var funcionarios = DAO.ModeradoresEvento(id);

            return Ok(funcionarios);
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
