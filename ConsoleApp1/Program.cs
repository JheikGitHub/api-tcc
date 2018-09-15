using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Usuario user  = new Usuario();

            user.Id = "1";
            user.Nome = "Marcus";

            Test(user);

            Console.Write(user.Nome);
            Console.ReadKey();
        }
        public static  void Test(Usuario usuario)
        {
            usuario.Nome = "Jeik";
        }
    }

    public class Usuario
    {
        public string Id;
        public string Nome;
    }
}
