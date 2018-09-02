using KonohaApi.DAO;
using KonohaApi.ViewModels;
using System;
using System.Net;
using System.Web.Http;

namespace KonohaApi.Controllers
{
    [RoutePrefix("api/agenda")]
    public class AgendaController : ApiController
    {
        private AgendaDAO _dao;
        public AgendaDAO DAO
        {
            get
            {
                if (_dao == null)
                    _dao = new AgendaDAO();
                return _dao;
            }
            set => _dao = value;
        }

        #region Crud Agenda
        [HttpGet]
        [Route("todos-os-eventos-da-agenda/{id}")]
        public IHttpActionResult BuscaEventosDaAgenda(string id)
        {
            var eventos = DAO.BuscaEventosDaAgenda(id);

            return Ok(eventos);
        }

        [HttpGet]
        [Route("busca-todos")]
        public IHttpActionResult BuscaTodos()
        {
            var agendas = DAO.ListaTodos();

            return Ok(agendas);
        }

        [HttpGet]
        [Route("busca-por-id/{id:int}")]
        public IHttpActionResult BuscaPorId(int id)
        {
            var agendaViewModel = DAO.BuscaPorId(id);

            if (agendaViewModel == null)
                return NotFound();

            return Ok(agendaViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("adiciona")]
        public IHttpActionResult Salvar(AgendaViewModel model)
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
        public IHttpActionResult Editar(int id, AgendaViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
            {
                return BadRequest();
            }

            var existente = DAO.NomeExistente(model.Nome);

            if (existente)
                return BadRequest("Agenda ja existe.");

            string resultado = DAO.Editar(model);

            if (resultado.Equals("OK"))
                return StatusCode(HttpStatusCode.NoContent);

            return BadRequest(resultado);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("remove/{id:int}")]
        public IHttpActionResult Remove(int id)
        {
            try
            {
                string resultado = DAO.Remove(id);

                if (resultado.Equals("OK"))
                    return Ok();

                return BadRequest(resultado);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

        }

        #endregion
    }
}
