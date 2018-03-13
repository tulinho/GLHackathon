using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Resiliens.Models
{
    public class ProcessadorDeTexto
    {
        public string RegexPalavrasInteresse = @"[a-zA-ZáéíóúâêîôûãõçÁÉÍÓÚÂÊÎÔÛÃÕÇ]{6,}|
R\\$[0-9.,]+|
[cpf|CPF]{3}[ ]*[0-9]{3}.[0-9]{3}.[0-9]{3}-[0-9]{2}|
[cnpj|CNPJ]{4}[0-9]{2}.[0-9]{3}.[0-9]{3}\/[0-9]{4}-[0-9]{2}";

        public int QuantidadeDocumentosProcessados { get; set; }
        public string Texto { get; set; }
        public List<Paragrafo> Paragrafos { get; set; }
        public List<Palavra> PalavrasEncontradasEmOutrosDocumentos { get; set; }
        public List<Palavra> Palavras { get; set; }

        public ProcessadorDeTexto(string texto)
        {
            QuantidadeDocumentosProcessados = 1;
            PalavrasEncontradasEmOutrosDocumentos = new List<Palavra>();
            Processar(texto);
        }

        public void Processar(string texto)
        {
            QuantidadeDocumentosProcessados++;
            Texto = texto;
            Palavras = new List<Palavra>();
            Paragrafos = new List<Paragrafo>();
            ExtrairParagrafos();
            ExtrairPalavras();
            CalcularPesoDasPalavras();
            CalcularPesoDosParagrafos();
            Paragrafos = Paragrafos.OrderByDescending(p => p.Peso).ToList();
            var palavrasordenadas = Palavras.OrderByDescending(m => m.QuantidadeOcorrencias);
        }

        private void ExtrairParagrafos()
        {
            List<string> linhas = Texto.Split(new string[] { "\n ", " \n", "\r\n", "\r\t" }, System.StringSplitOptions.None).ToList();
            StringBuilder texto = new StringBuilder();
            List<string> paragrafos = new List<string>();
            linhas.ForEach(linha => { texto = AdicionarLinhaAoParagrafo(linha, texto, paragrafos); });
            paragrafos = paragrafos.Where(p1 => p1.Count(s => s == '\n') > 1).ToList();

            int contador = 0;
            paragrafos.ForEach(paragrafo =>
            {
                Paragrafos.Add(new Paragrafo() { Texto = paragrafo, OrdemNoTexto = contador });
                contador++;
            });
        }

        private StringBuilder AdicionarLinhaAoParagrafo(string linha, StringBuilder texto, List<string> paragrafos)
        {
            if (String.IsNullOrWhiteSpace(linha))
            {
                paragrafos.Add(texto.ToString());
                return new StringBuilder();
            }
            texto.AppendLine(linha);
            return texto;
        }

        private void ExtrairPalavras()
        {
            Regex regex = new Regex(RegexPalavrasInteresse);
            Paragrafos.ForEach(paragrafo => ExtrairPalavra(paragrafo, regex));
        }

        private void ExtrairPalavra(Paragrafo paragrafo, Regex regex)
        {
            ContarPalavrasDoParagrafo(paragrafo, regex);
            paragrafo.Palavras.ForEach(palavra =>
            {
                var ocorrencia = Palavras.FirstOrDefault(m => m.Texto == palavra.Texto);
                if (ocorrencia != null)
                {
                    ocorrencia.QuantidadeOcorrencias += palavra.QuantidadeOcorrencias;
                }
                else
                {
                    Palavras.Add(new Palavra() { Texto = palavra.Texto, QuantidadeOcorrencias = palavra.QuantidadeOcorrencias });
                }
            });
        }

        private static void ContarPalavrasDoParagrafo(Paragrafo paragrafo, Regex regex)
        {
            var correspondencias = regex.Matches(paragrafo.Texto);
            paragrafo.QuantidadeDePalavras = correspondencias.Count;
            foreach (Match ocorrencia in correspondencias)
            {
                var palavra = paragrafo.Palavras.FirstOrDefault(m => m.Texto == ocorrencia.Value);
                if (palavra != null)
                {
                    palavra.QuantidadeOcorrencias++;
                    continue;
                }
                paragrafo.Palavras.Add(new Palavra() { QuantidadeOcorrencias = 1, Texto = ocorrencia.Value });
            }
        }

        private void CalcularPesoDasPalavras()
        {
            int quantidadeDePalavras = Palavras.Sum(m => m.QuantidadeOcorrencias);
            Palavras.ForEach(m => m.Peso = ((double)m.QuantidadeOcorrencias) / quantidadeDePalavras);

            Palavras.ForEach(m =>
            {
                var palavra = PalavrasEncontradasEmOutrosDocumentos.FirstOrDefault(n => m.Texto == n.Texto);
                if (palavra != null)
                {
                    palavra.QuantidadeDeDocumentosEmQueOcorre++;
                }
                else
                {
                    PalavrasEncontradasEmOutrosDocumentos.Add(m);
                    palavra = m;
                }
                m.Peso = m.Peso * Math.Log10(1 + palavra.QuantidadeDeDocumentosEmQueOcorre);
                //m.Peso = m.Peso * Math.Log10(QuantidadeDocumentosProcessados/palavra.QuantidadeDeDocumentosEmQueOcorre);
            });
        }

        private void CalcularPesoDosParagrafos()
        {
            Paragrafos.ForEach(paragrafo =>
            {
                var palavras = Palavras.Where(pl1 => paragrafo.Palavras.Any(pl2 => pl2.Texto == pl1.Texto));
                paragrafo.Peso = palavras.Sum(m => m.Peso * paragrafo.Palavras.First(pl2 => pl2.Texto == m.Texto).QuantidadeOcorrencias) / paragrafo.QuantidadeDePalavras;
                //paragrafo.Peso = palavras.Sum(m => m.Peso * paragrafo.Palavras.First(pl2 => pl2.Texto == m.Texto).QuantidadeOcorrencias);
            });
        }
    }
}