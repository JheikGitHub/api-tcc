using KonohaApi.Models;

namespace KonohaApi.DAO
{
    public class BaseDAO
    {
        private DataContext _db;

        public DataContext Db
        {
            get
            {
                if (_db == null)
                    _db = new DataContext();

                return _db;
            }
            set
            {
                _db = value;
            }
        }
    }
}