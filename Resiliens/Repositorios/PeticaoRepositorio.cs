using Dapper;
using Resiliens.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Resiliens.Repositorios
{
    public class PeticaoRepositorio
    {
        #region Scripts

        private const string INSERIR = @"
INSERT INTO peticao (
cod_peticao, foro, natureza_processo, comarca, reclamante, cpf_reclamante, natureza_acao, requerido
)
values (
@cod_peticao, @foro, @natureza_processo, @comarca, @reclamante, @cpf_reclamante, @natureza_acao, @requerido
)";

        private const string LISTAR = @"
SELECT 
    cod_peticao AS CodigoPeticao, 
    foro AS Foro, 
    natureza_processo AS NaturezaProcesso, 
    comarca AS Comarca, 
    reclamante AS Reclamante, 
    cpf_reclamante AS CpfReclamante, 
    natureza_acao AS NaturezaAcao, 
    requerido AS Requerido
FROM 
    peticao
WHERE
    1=1 AND
    {0}
";

        #endregion

        public void Inserir(Peticao peticao)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("cod_peticao", peticao.CodigoPeticao, System.Data.DbType.AnsiString);
            parametros.Add("foro", peticao.Foro, System.Data.DbType.AnsiString);
            parametros.Add("natureza_processo", peticao.NaturezaProcesso, System.Data.DbType.AnsiString);
            parametros.Add("comarca", peticao.Comarca, System.Data.DbType.AnsiString);
            parametros.Add("reclamante", peticao.Reclamante, System.Data.DbType.AnsiString);
            parametros.Add("cpf_reclamante", peticao.CpfReclamante, System.Data.DbType.AnsiString);
            parametros.Add("natureza_acao", peticao.NaturezaAcao, System.Data.DbType.AnsiString);
            parametros.Add("requerido", peticao.Requerido, System.Data.DbType.AnsiString);

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost;Database=GLHackathon;";
                conn.Open();

                conn.Execute(INSERIR, parametros);
            }
        }

        public List<Peticao> Listar()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost;Database=GLHackathon;";
                conn.Open();

                return conn.Query<Peticao>(LISTAR).ToList<Peticao>();
            }
        }
    }
}