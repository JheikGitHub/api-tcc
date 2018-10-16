using AutoMapper;
using KonohaApi.Interfaces;
using KonohaApi.Models;
using KonohaApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace KonohaApi.DAO
{
    public class FuncionarioDAO : BaseDAO, ICrud<FuncionarioViewModel>, IDisposable
    {
        #region Crud Funcionario
        public string Adicionar(FuncionarioViewModel entity)
        {
            try
            {
                var funcionarioModel = Mapper.Map<FuncionarioViewModel, Funcionario>(entity);

                Db.Funcionario.Add(funcionarioModel);
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public string AdicionarFuncionario(string userName, FuncionarioViewModel entity)
        {
            try
            {
                var usuario = Db.Usuario.FirstOrDefault(x => x.UserName == userName);
                if (usuario == null)
                    throw new Exception("Usuario invalido.");

                if (entity.Usuario.PathFotoPerfil != "")
                {
                    string path = HttpContext.Current.Server.MapPath("~/Imagens/Usuario/");

                    var bits = Convert.FromBase64String(entity.Usuario.PathFotoPerfil);

                    string nomeImagem = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg";

                    string imgPath = Path.Combine(path, nomeImagem);

                    File.WriteAllBytes(imgPath, bits);

                    entity.Usuario.PathFotoPerfil = nomeImagem;
                }
                entity.Usuario.DataCadastro = DateTime.Now;
                entity.Usuario.Ativo = true;

                var funcionarioModel = Mapper.Map<FuncionarioViewModel, Funcionario>(entity);
               
                Db.Funcionario.Add(funcionarioModel);
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public FuncionarioViewModel BuscaPorId(int id)
        {
            var funcionarioViewModel = Mapper.Map<Funcionario, FuncionarioViewModel>(Db.Funcionario.First(x => x.Id == id));

            return funcionarioViewModel;
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public string Editar(FuncionarioViewModel entity)
        {
            try
            {
                var funcionarioModel = Mapper.Map<FuncionarioViewModel, Funcionario>(entity);

                Db.Entry(funcionarioModel).State = EntityState.Modified;
                Db.Entry(funcionarioModel.Usuario).State = EntityState.Modified;
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public ICollection<FuncionarioViewModel> ListaTodos()
        {
            var funcionarioModel = Db.Funcionario.ToList();

            foreach (var item in funcionarioModel)
            {
                Db.Funcionario.Include("Usuario").Where(x => x.Usuario.Id == item.Id).ToList();
            }

            var funcionarioViewModel = Mapper.Map<ICollection<Funcionario>, ICollection<FuncionarioViewModel>>(funcionarioModel);

            return funcionarioViewModel;
        }

        public string Remove(int id)
        {
            try
            {
                var funcionarioModel = Db.Funcionario.Find(id);

                if (funcionarioModel == null)
                    throw new Exception("Funcionario não encontrado.");

                Db.Funcionario.Remove(funcionarioModel);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        #endregion

        public ICollection<AgendaViewModel> AgendasDoFuncionario(int id)
        {

            var agendasModel = Db.AgendaEvento.Where(x => x.FuncionarioId == id).ToList();

            var agendasViewModel = Mapper.Map<ICollection<AgendaEvento>, ICollection<AgendaViewModel>>(agendasModel);

            return agendasViewModel;

        }
    }
}