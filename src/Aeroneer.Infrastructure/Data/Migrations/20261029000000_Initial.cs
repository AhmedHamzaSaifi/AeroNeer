using Aeroneer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aeroneer.Infrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261029000000_Initial")]
public sealed class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Candidates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CompanyOrInstitute = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                ZipCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                Place = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                Country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                Gender = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                Telephone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Candidates", entity => entity.Id));

        migrationBuilder.CreateTable(
            name: "AssessmentAttempts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                Module = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Mode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                StartedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                FinishedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                DurationMilliseconds = table.Column<long>(type: "bigint", nullable: true),
                Score = table.Column<double>(type: "double precision", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssessmentAttempts", entity => entity.Id);
                table.ForeignKey("FK_AssessmentAttempts_Candidates_CandidateId", entity => entity.CandidateId, "Candidates", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CandidateModuleProgress",
            columns: table => new
            {
                CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                Module = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                PracticeCompleted = table.Column<bool>(type: "boolean", nullable: false),
                PracticeCompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CandidateModuleProgress", entity => new { entity.CandidateId, entity.Module });
                table.ForeignKey("FK_CandidateModuleProgress_Candidates_CandidateId", entity => entity.CandidateId, "Candidates", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_AssessmentAttempts_CandidateId_Module_Mode_AttemptNumber", table: "AssessmentAttempts", columns: new[] { "CandidateId", "Module", "Mode", "AttemptNumber" }, unique: true, filter: "\"Mode\" = 'Assessment'");
        migrationBuilder.CreateIndex(name: "IX_AssessmentAttempts_CandidateId_Module_Mode_StartedAtUtc", table: "AssessmentAttempts", columns: new[] { "CandidateId", "Module", "Mode", "StartedAtUtc" });
        migrationBuilder.CreateIndex(name: "IX_Candidates_Email", table: "Candidates", column: "Email");
        migrationBuilder.CreateIndex(name: "IX_Candidates_LastName", table: "Candidates", column: "LastName");
        migrationBuilder.CreateIndex(name: "IX_Candidates_LastName_FirstName", table: "Candidates", columns: new[] { "LastName", "FirstName" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AssessmentAttempts");
        migrationBuilder.DropTable(name: "CandidateModuleProgress");
        migrationBuilder.DropTable(name: "Candidates");
    }
}
