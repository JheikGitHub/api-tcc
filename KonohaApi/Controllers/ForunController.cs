using KonohaApi.DAO;
using KonohaApi.ViewModels;
using System.Net;
using System.Web.Http;

namespace KonohaApi.Controllers
{
    [RoutePrefix("api/forun")]
    public class ForunController : ApiController
    {
        private ForumDAO _dao;

        public ForumDAO DAO
        {
            get
            {
                if (_dao == null)
                    _dao = new ForumDAO();
                return _dao;
            }
            set => _dao = value;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("adiciona-topico")]
        public IHttpActionResult SalvaTopico(TopicoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.AdicionaTopico(model);

            if (resultado.Equals("OK"))
            {
                return StatusCode(HttpStatusCode.Created);
            }
            else
                return BadRequest(resultado);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("adiciona-comentario")]
        public IHttpActionResult SalvaComentario(ComentarioViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.AdicionaComentario(model);

            if (resultado.Equals("OK"))
            {
                return StatusCode(HttpStatusCode.Created);
            }
            else
                return BadRequest(resultado);
        }

        [HttpGet]
        [Route("busca-todos-topicos")]
        public IHttpActionResult BuscaTodosTopicos()
        {
            var topicos = DAO.BuscaTodosTopicos();
            return Ok(topicos);
        }

        [HttpGet]
        [Route("busca-topicos-por-nome/{id}")]
        public IHttpActionResult BuscaTopicosPorNome(string id)
        {
            var topicos = DAO.BuscaPorNome(id);
            return Ok(topicos);
        }
        [HttpGet]
        [Route("busca-topicos-do-evento/{id:int}")]
        public IHttpActionResult BuscaTopicosDoEvento(int id)
        {
            var comentarios = DAO.BuscaTopicosDoEvento(id);
            return Ok(comentarios);
        }

        [HttpGet]
        [Route("busca-todos-comentarios-por-topicos/{id:int}")]
        public IHttpActionResult BuscaTodos(int id)
        {
            var comentarios = DAO.BuscaTodoComentarioPorTopico(id);
            return Ok(comentarios);
        }

        [HttpGet]
        [Route("busca-comentarios-filhos")]
        public IHttpActionResult BuscaComentarioFilho(int id)
        {
            var comentariosFilho = DAO.BuscaComentariosFilho(id);
            return Ok(comentariosFilho);
        }

        [HttpPut]
        [Route("altera-topico/{id:int}")]
        public IHttpActionResult EditaTopico(int id, TopicoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return BadRequest();

            string resultado = DAO.EditaTopico(model);
            if (resultado.Equals("OK"))
                return StatusCode(HttpStatusCode.NoContent);
            else
                return BadRequest(resultado);
        }

        [HttpPut]
        [Route("altera-comentario/{id:int}")]
        public IHttpActionResult EditaComentario(int id, ComentarioViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return BadRequest();

            string resultado = DAO.EditaComentario(model);

            if (resultado.Equals("OK"))
                return StatusCode(HttpStatusCode.NoContent);
            else
                return BadRequest(resultado);
        }

        [HttpDelete]
        //  [Authorize(Roles = "Admin")]
        [Route("remove-topico/{id:int}")]
        public IHttpActionResult RemoveTopico(int id)
        {
            if (id < 0)
                return BadRequest("Indice negativo");

            string resultado = DAO.RemoveTopico(id);

            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);
        }

        [HttpDelete]
        //  [Authorize(Roles = "Admin")]
        [Route("remove-comentario/{id:int}")]
        public IHttpActionResult RemoveComentario(int id)
        {
            if (id < 0)
                return BadRequest("Indice negativo");

            string resultado = DAO.RemoveComentario(id);

            if (resultado.Equals("OK"))
                return Ok();
            else
                return BadRequest(resultado);
        }
    }
}
