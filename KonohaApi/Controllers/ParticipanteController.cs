using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using KonohaApi.DAO;
using KonohaApi.Models;
using KonohaApi.ViewModels;

namespace KonohaApi.Controllers
{
    [RoutePrefix("api/aluno")]
    public class ParticipanteController : ApiController
    {
        private ParticipanteDAO _dao;
        public ParticipanteDAO DAO
        {
            get
            {
                if (_dao == null)
                    _dao = new ParticipanteDAO();
                return _dao;
            }
            set => _dao = value;
        }

        // GET: api/Aluno
        [HttpGet]
        [Route("busca-todos")]
        public ICollection<ParticipanteViewModel> BuscaTodos()
        {
            return DAO.ListaTodos();
        }

        // GET: api/Aluno/5
        [HttpGet]
        [Route("busca-por-id/{id:int}")]
        [ResponseType(typeof(ParticipanteViewModel))]
        public IHttpActionResult BuscaPorId(int id)
        {
            var aluno = DAO.BuscaPorId(id);

            return Ok(aluno);
        }

        // PUT: api/Aluno/5
        [HttpPut]
        [Route("altera/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Editar(int id, ParticipanteViewModel aluno)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aluno.Id)
            {
                return BadRequest();
            }
            string resultado = DAO.Editar(aluno);

            if (resultado.Equals("OK"))
                return StatusCode(HttpStatusCode.NoContent);
            else
                return BadRequest(resultado);

        }

        // POST: api/Aluno
        [HttpPost]
        [AllowAnonymous]
        [Route("adiciona")]
        [ResponseType(typeof(Participante))]
        public IHttpActionResult Salvar(ParticipanteViewModel aluno)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string resultado = DAO.Adicionar(aluno);

            if (resultado.Equals("OK"))
            {
                return StatusCode(HttpStatusCode.Created);
            }
            else
                return BadRequest(resultado);
        }

        // DELETE: api/Aluno/5
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("remove/{id:int}")]
        [ResponseType(typeof(Participante))]
        public IHttpActionResult Remove(int id)
        {
            var alunoViewModel = DAO.BuscaPorId(id);

            if (alunoViewModel == null)
                return NotFound();

            string resultado = DAO.Remove(id);
            if (resultado.Equals("OK"))
                return Ok();

            return BadRequest(resultado);
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