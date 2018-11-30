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
    public class ParticipanteDAO : BaseDAO, ICrud<ParticipanteViewModel>, IDisposable
    {

        #region Crud Participante
        public string Adicionar(ParticipanteViewModel entity)
        {
            try
            {
                bool alunoExistente = Db.Participante.Count(x => x.Matricula == entity.Matricula) > 0;

                if (alunoExistente)
                    throw new Exception("Usuario ja cadastrado.");


                var alunoModel = Mapper.Map<ParticipanteViewModel, Participante>(entity);

                Db.Participante.Add(alunoModel);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string AdicionarParticipante(ParticipanteViewModel entity)
        {
            try
            {
                //var usuario = Db.Usuario.FirstOrDefault(x => x.UserName == nome);
                //if (usuario == null)
                //    throw new Exception("Usuario invalido.");

                bool alunoExistente = Db.Participante.Count(x => x.Matricula == entity.Matricula) > 0;

                if (alunoExistente)
                    throw new Exception("Usuario ja cadastrado.");

                if (entity.Usuario.PathFotoPerfil != "")
                {
                    string path = HttpContext.Current.Server.MapPath("~/Imagens/Usuario/");

                    var bits = Convert.FromBase64String(entity.Usuario.PathFotoPerfil);

                    string nomeImagem = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg";

                    string imgPath = Path.Combine(path, nomeImagem);

                    File.WriteAllBytes(imgPath, bits);

                    entity.Usuario.PathFotoPerfil = nomeImagem;
                }
                entity.Usuario.Ativo = true;
                entity.Usuario.DataCadastro = DateTime.Now;
                var alunoModel = Mapper.Map<ParticipanteViewModel, Participante>(entity);

                Db.Participante.Add(alunoModel);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public ParticipanteViewModel BuscaPorId(int id)
        {
            var alunoViewModel = Mapper.Map<Participante, ParticipanteViewModel>(Db.Participante.Include("Usuario").FirstOrDefault(x => x.Id == id));

            return alunoViewModel;
        }


        public ParticipanteViewModel BuscaporCodigo(string id)
        {
            var alunoViewModel = Mapper.Map<Participante, ParticipanteViewModel>(Db.Participante.Include("Usuario").FirstOrDefault(x => x.CodCarteirinha == id));

            return alunoViewModel;
        }

        public ParticipanteViewModel GetParticipanteLogado(string username)
        {
            var participanteView = Mapper.Map<Participante, ParticipanteViewModel>(Db.Participante.Include("Usuario").FirstOrDefault(x => x.Usuario.UserName == username));

            return participanteView;
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public string Editar(ParticipanteViewModel entity)
        {
            try
            {
                var participanteModel = Mapper.Map<ParticipanteViewModel, Participante>(entity);

                Db.Participante.Attach(participanteModel);

                bool alunoExistente = Db.Participante.Count(x => x.Matricula == entity.Matricula) > 1;

                if (alunoExistente)
                    throw new Exception("Aluno existente.");

                Db.Entry(participanteModel).State = EntityState.Modified;
                Db.Entry(participanteModel.Usuario).State = EntityState.Modified;
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public ICollection<ParticipanteViewModel> ListaTodos()
        {
            var participantes = Db.Participante.ToList();

            foreach (var item in participantes)
            {
                Db.Participante.Include("Usuario").Where(x => x.Usuario.Id == item.Id).ToList();
            }

            var alunoViewModel = Mapper.Map<ICollection<Participante>, ICollection<ParticipanteViewModel>>(participantes);

            return alunoViewModel;
        }

        public string Remove(int id)
        {
            try
            {
                var ParticipanteModel = Db.Participante.Find(id);

                if (ParticipanteModel == null)
                    throw new Exception("Participante não encontrado.");

                Db.Participante.Remove(ParticipanteModel);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        #endregion

    }
}