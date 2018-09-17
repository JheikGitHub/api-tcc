using KonohaApi.DAO;
using KonohaApi.Models;
using KonohaApi.ViewModels;
using System.Net;
using System.Web.Http;

namespace KonohaApi.Controllers
{
    [RoutePrefix("api/funcionario")]
    public class FuncionarioController : ApiController
    {
        private readonly DataContext _db = new DataContext();

        private FuncionarioDAO _dao;
        public FuncionarioDAO DAO
        {
            get
            {
                if (_dao == null)
                    _dao = new FuncionarioDAO();
                return _dao;
            }
            set => _dao = value;
        }

        [HttpGet]
        [Route("busca-todos")]
        public IHttpActionResult BuscaTodos()
        {
            var funcionarios = DAO.ListaTodos();

            return Ok(funcionarios);
        }

        [HttpGet]
        [Route("busca-por-id/{id:int}")]
        public IHttpActionResult BuscaPorId(int id)
        {
            var funcionarioViewModel = DAO.BuscaPorId(id);

            return Ok(funcionarioViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("adiciona")]
        public IHttpActionResult Salvar(FuncionarioViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string resultado = DAO.AdicionarFuncionario(User.Identity.Name, model);

            if (resultado.Equals("OK"))
                return StatusCode(HttpStatusCode.Created);
            else
                return BadRequest(resultado);
        }

        [HttpPut]
        [Route("altera/{id:int}")]
        public IHttpActionResult Editar(int id, FuncionarioViewModel model)
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
    }
}
