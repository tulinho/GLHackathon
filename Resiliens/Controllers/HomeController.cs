using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Resiliens.Entidades;
using Resiliens.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Resiliens.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string caminho = "C:\\GLHackathon\\Resources\\peticao1.pdf";

            string textoPeticao = LerArquivoPdf(caminho);
            Peticao peticao = ProcessarPeticao(textoPeticao);
            SalvarPeticao(peticao);
            return View();
        }

        public string LerArquivoPdf(string caminho)
        {
            try
            {
                using (PdfReader leitor = new PdfReader(caminho))
                {
                    StringBuilder texto = new StringBuilder();
                    for (int i = 1; i <= leitor.NumberOfPages; i++)
                    {
                        texto.Append(PdfTextExtractor.GetTextFromPage(leitor, i));
                    }
                    return texto.ToString();
                }
            }
            catch
            {
                throw;
            }
        }

        private Peticao ProcessarPeticao(string textoPeticao)
        {
            return new Peticao()
            {
                CodigoPeticao = "testeCodigoPeticao",
                Comarca = "testeComarca",
                CpfReclamante = "testeCpfReclamante",
                Foro = "testeForo",
                NaturezaAcao = "testeNaturezaAcao",
                NaturezaProcesso = "testeNaturezaProcesso",
                Reclamante = "testeReclamante",
                Requerido = "testeRequerido"
            };
        }

        private void SalvarPeticao(Peticao peticao)
        {
            PeticaoRepositorio repositorio = new PeticaoRepositorio();

            repositorio.Inserir(peticao);
        }
    }
}