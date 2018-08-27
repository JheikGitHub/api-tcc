using System.Collections.Generic;

namespace KonohaApi.Interfaces
{
    interface ICrud<TEntity> where TEntity : class
    {
        string Adicionar(TEntity entity);
        string Remove(int id);
        string Editar(TEntity entity);
        ICollection<TEntity> ListaTodos();
        TEntity BuscaPorId(int id);
    }
}
