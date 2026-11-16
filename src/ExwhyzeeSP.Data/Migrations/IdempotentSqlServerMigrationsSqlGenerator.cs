using System;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Update;

namespace ExwhyzeeSP.Data.Migrations
{
    /// <summary>
    /// Wraps EF Core SQL Server migration DDL statements in IF OBJECT_ID / IF COL_LENGTH / IF NOT EXISTS guards
    /// so Update-Database skips existing tables, columns, indexes, and foreign keys without failing on existing school DBs.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
    public class IdempotentSqlServerMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public IdempotentSqlServerMigrationsSqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            ICommandBatchPreparer commandBatchPreparer)
            : base(dependencies, commandBatchPreparer)
        {
        }

        protected override void Generate(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            var temp = new MigrationCommandListBuilder(Dependencies);
            base.Generate(operation, model, temp, terminate);

            var sql = CombineCommands(temp);
            if (string.IsNullOrWhiteSpace(sql))
                return;

            var schema = EscapeSqlLiteral(string.IsNullOrWhiteSpace(operation.Schema) ? "dbo" : operation.Schema);
            var table = EscapeSqlLiteral(operation.Name);

            builder
                .AppendLine($"IF OBJECT_ID(N'[{schema}].[{table}]', N'U') IS NULL")
                .AppendLine("BEGIN")
                .AppendLine(sql)
                .AppendLine("END")
                .EndCommand();
        }

        protected override void Generate(
            AddColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            var temp = new MigrationCommandListBuilder(Dependencies);
            base.Generate(operation, model, temp, terminate);

            var sql = CombineCommands(temp);
            if (string.IsNullOrWhiteSpace(sql))
                return;

            var schema = EscapeSqlLiteral(string.IsNullOrWhiteSpace(operation.Schema) ? "dbo" : operation.Schema);
            var table = EscapeSqlLiteral(operation.Table);
            var col = EscapeSqlLiteral(operation.Name);

            builder
                .AppendLine($"IF COL_LENGTH(N'{schema}.{table}', N'{col}') IS NULL")
                .AppendLine("BEGIN")
                .AppendLine(sql)
                .AppendLine("END")
                .EndCommand();
        }

        protected override void Generate(
            CreateIndexOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            var temp = new MigrationCommandListBuilder(Dependencies);
            base.Generate(operation, model, temp, terminate);

            var sql = CombineCommands(temp);
            if (string.IsNullOrWhiteSpace(sql))
                return;

            var schema = EscapeSqlLiteral(string.IsNullOrWhiteSpace(operation.Schema) ? "dbo" : operation.Schema);
            var table = EscapeSqlLiteral(operation.Table);
            var idx = EscapeSqlLiteral(operation.Name);

            var colChecks = new StringBuilder();
            foreach (var col in operation.Columns)
            {
                colChecks.AppendLine($"   AND COL_LENGTH(N'{schema}.{table}', N'{EscapeSqlLiteral(col)}') IS NOT NULL");
            }

            builder
                .AppendLine($"IF OBJECT_ID(N'[{schema}].[{table}]', N'U') IS NOT NULL")
                .AppendLine($"   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'{idx}' AND object_id = OBJECT_ID(N'[{schema}].[{table}]'))")
                .Append(colChecks.ToString())
                .AppendLine("BEGIN")
                .AppendLine(sql)
                .AppendLine("END")
                .EndCommand();
        }

        protected override void Generate(
            AddForeignKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            var temp = new MigrationCommandListBuilder(Dependencies);
            base.Generate(operation, model, temp, terminate);

            var sql = CombineCommands(temp);
            if (string.IsNullOrWhiteSpace(sql))
                return;

            var schema = EscapeSqlLiteral(string.IsNullOrWhiteSpace(operation.Schema) ? "dbo" : operation.Schema);
            var fk = EscapeSqlLiteral(operation.Name);

            builder
                .AppendLine($"IF OBJECT_ID(N'[{schema}].[{fk}]', N'F') IS NULL")
                .AppendLine("BEGIN")
                .AppendLine(sql)
                .AppendLine("END")
                .EndCommand();
        }

        private static string CombineCommands(MigrationCommandListBuilder temp)
        {
            var sb = new StringBuilder();
            foreach (var cmd in temp.GetCommandList())
            {
                if (!string.IsNullOrWhiteSpace(cmd.CommandText))
                {
                    sb.AppendLine(cmd.CommandText);
                }
            }
            return sb.ToString().Trim();
        }

        private static string EscapeSqlLiteral(string value) => value.Replace("'", "''");
    }
}
