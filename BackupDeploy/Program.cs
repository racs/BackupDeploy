using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BackupDeploy
{
    class Program
    {
        public static  string RetornaNomePasta()
        {
            string sdata;
            DateTime data;
            data = System.DateTime.Now;
            sdata = Convert.ToString(data);
            sdata = sdata.Replace("/", "");
            sdata = sdata.Replace(":", "");
            sdata = sdata.Replace(" ", "_");
            sdata = "Sistema_" + sdata;
            return sdata;
        }

        public static void CopiarArquivos(string caminhoorigem, string caminhodestino)
        {
                       
            string[] diretorios = System.IO.Directory.GetDirectories(caminhoorigem, "*", SearchOption.AllDirectories);
            

            foreach (string d in diretorios)
            {                
                //Console.WriteLine(d);                
                Directory.CreateDirectory(d.Replace(caminhoorigem, caminhodestino));
                //Console.WriteLine(d.Replace(caminhoorigem, caminhodestino) + " criado");                
            }

            foreach (string f in Directory.GetFiles(caminhoorigem, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(f, f.Replace(caminhoorigem, caminhodestino), true);
                //Console.WriteLine(f.Replace(caminhoorigem,caminhodestino));                    

            }

        }

        static void Main(string[] args)
        {            
            string caminhodapastaorigem = @"D:\controle\Tiplan\sistema\";
            string caminhodapastadestino = @"D:\controle\Tiplan\backup\sistema\";
            string caminhodapastadestinorenomeado;
            string nomedapastacomdata;

            if (!Directory.Exists(caminhodapastadestino))
            {
                Directory.CreateDirectory(caminhodapastadestino);
                CopiarArquivos(caminhodapastaorigem, caminhodapastadestino);
            }
            else
            {
                nomedapastacomdata = RetornaNomePasta();
                caminhodapastadestinorenomeado = caminhodapastadestino.Replace("sistema", nomedapastacomdata);
                Directory.Move(caminhodapastadestino, caminhodapastadestinorenomeado);
                //Console.WriteLine(caminhodapastadestinorenomeado);
                CopiarArquivos(caminhodapastaorigem, caminhodapastadestino);
            }            

            //Console.WriteLine(sdata);
            Console.ReadLine();

        }
    }
}
