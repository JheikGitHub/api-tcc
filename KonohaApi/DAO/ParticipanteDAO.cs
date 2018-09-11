using AutoMapper;
using KonohaApi.Interfaces;
using KonohaApi.Models;
using KonohaApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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

        public string AdicionarParticipante(string nome, ParticipanteViewModel entity)
        {
            try
            {
                var usuario = Db.Usuario.FirstOrDefault(x => x.UserName == nome);
                if (usuario == null)
                    throw new Exception("Usuario invalido.");

                bool alunoExistente = Db.Participante.Count(x => x.Matricula == entity.Matricula) > 0;

                if (alunoExistente)
                    throw new Exception("Usuario ja cadastrado.");

                var alunoModel = Mapper.Map<ParticipanteViewModel, Participante>(entity);
                alunoModel.Id = usuario.Id;
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
            var alunoViewModel = Mapper.Map<Participante, ParticipanteViewModel>(Db.Participante.First(x => x.Id == id));

            return alunoViewModel;
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public string Editar(ParticipanteViewModel entity)
        {
            try
            {
                bool alunoExistente = Db.Participante.Count(x => x.Matricula == entity.Matricula) > 1;
                if (alunoExistente)
                    throw new Exception("Aluno existente.");

                var alunoModel = Mapper.Map<ParticipanteViewModel, Participante>(entity);

                Db.Entry(alunoModel).State = EntityState.Modified;
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
            var alunos = Db.Participante.ToList();

            var alunoViewModel = Mapper.Map<ICollection<Participante>, ICollection<ParticipanteViewModel>>(alunos);

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