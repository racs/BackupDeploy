using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mail;

/* Rotina de backup durante o processo de deploy da aplicação, a pasta Sistema_atual guarda o conteúdo da pasta Tiplan\Sistema, 
ou seja guarda o sistema que está rodando no momento (versão atual) a pasta tiplan\backup\sistema vai sempre guardar a versão anterior do sistema atual
e as pastas com as datas guardarão as versões anteriores a esta.*/

namespace BackupDeploy
{
    class Program
    {
        public static string RetornaNomePasta()
        {
            string sdata;
            DateTime data;
            data = System.DateTime.Now;
            sdata = Convert.ToString(data);
            sdata = sdata.Replace("/", "");
            sdata = sdata.Replace(":", "");
            sdata = sdata.Replace(" ", "");
            sdata = "Sistema" + sdata;
            return sdata;
        }

        //Verifica a existência das pastas principais envolvidas no processo
        public static bool VerificarConsistencia(string pastatiplansistemadata, string pastatiplansistema, string pastatiplanbackupsistema)
        {
            if (!Directory.Exists(pastatiplansistemadata) | !Directory.Exists(pastatiplansistema) | !Directory.Exists(pastatiplanbackupsistema))
            {
                Enviar_email("Falha de consistência de estrutura, não foi possível realizar o backup.");
                return false;
            }
            else
            {
                Console.WriteLine("Consistência da estrutura de pastas verificada, iniciando backup ...");
                return true;
            }
        }

        public static void Enviar_email(string mensagem)
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "smfnwms01";
            client.EnableSsl = false;
            client.Credentials = new System.Net.NetworkCredential("*********", "************");
            MailMessage mail = new MailMessage();
            mail.Sender = new System.Net.Mail.MailAddress("*********", "*********");
            mail.From = new MailAddress("*********", "*********");
            mail.To.Add(new MailAddress("*********", "*********"));
            mail.Subject = "Rotina de Backup";

            mail.Body = mensagem;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            try
            {
                client.Send(mail);
            }
            catch (System.Exception erro)
            {
                //trata erro
            }
            finally
            {
                mail = null;
            }

        }

        public static void CopiarArquivos(string caminhoorigem, string caminhodestino)
        {

            string[] diretorios = System.IO.Directory.GetDirectories(caminhoorigem, "*", SearchOption.AllDirectories);


            foreach (string d in diretorios)
            {
                Directory.CreateDirectory(d.Replace(caminhoorigem, caminhodestino));
            }

            foreach (string f in Directory.GetFiles(caminhoorigem, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(f, f.Replace(caminhoorigem, caminhodestino), true);
            }

        }

        static void Main(string[] args)
        {
            string tiplansistema = @"D:\controle\Tiplan\Sistema";
            string tiplanbackupsistema = @"D:\controle\Tiplan\backup\Sistema";
            string tiplansistema_atual = @"D:\controle\Tiplan\Sistema_atual";
            string tiplanbackupsistema_data;
            string nomedapastacomdata;


            //caminhodapastaorigem = args[0];
            //caminhodapastadestino = args[1];

            if (VerificarConsistencia(tiplansistema_atual, tiplansistema, tiplanbackupsistema))
            {

                try
                {
                    nomedapastacomdata = RetornaNomePasta();
                    tiplanbackupsistema_data = tiplanbackupsistema.Replace("Sistema", nomedapastacomdata);

                    //move da pasta backup/sitema para backup/sistema_data
                    Console.WriteLine("Movendo arquivos da pasta " + tiplanbackupsistema + " para a pasta " + tiplanbackupsistema_data);
                    Directory.Move(tiplanbackupsistema, tiplanbackupsistema_data);
                    Console.WriteLine("Arquivos movidos com sucesso");


                    //copiar da pasta backup/sistema_atual para a pasta backup/sistema
                    Console.WriteLine("Criando a pasta " + tiplanbackupsistema);
                    Directory.CreateDirectory(tiplanbackupsistema);
                    Console.WriteLine("Pasta criada com sucesso");
                    Console.WriteLine("copiando arquivos da pasta " + tiplansistema_atual + " para a pasta " + tiplanbackupsistema);
                    CopiarArquivos(tiplansistema_atual, tiplanbackupsistema);
                    Console.WriteLine("Arquivos copiados com sucesso");

                    //copiar da pasta tiplan/sistema para backup/sistema_atual
                    Console.WriteLine("Deletando a pasta " + tiplansistema_atual);
                    Directory.Delete(tiplansistema_atual, true);
                    Console.WriteLine("Pasta deletada com sucesso");
                    Console.WriteLine("Criando a pasta " + tiplansistema_atual);
                    Directory.CreateDirectory(tiplansistema_atual);
                    Console.WriteLine("Pasta criada com sucesso");
                    Console.WriteLine("copiando arquivos da pasta " + tiplansistema + " para a pasta " + tiplansistema_atual);
                    CopiarArquivos(tiplansistema, tiplansistema_atual);
                    Console.WriteLine("Arquivos copiados com sucesso");
                    Console.WriteLine("Backup realizado com sucesso");
                    Enviar_email("Backup realizado com sucesso em " + DateTime.Now);
                }
                catch (Exception e)
                {
                    Console.WriteLine("O processo de backup falhou: {0}", e.Message);
                    Enviar_email("Ocorreu um erro durante a execução da rotina de backup durante o deploy em " + DateTime.Now);
                }
            }            

            //Console.WriteLine(sdata);
            Console.ReadLine();

        }
    }
}
