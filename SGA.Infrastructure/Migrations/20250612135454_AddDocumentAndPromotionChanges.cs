using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentAndPromotionChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    YearsInCurrentRank = table.Column<int>(type: "int", nullable: false),
                    RequiredWorks = table.Column<int>(type: "int", nullable: false),
                    MinimumEvaluationScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    RequiredTrainingHours = table.Column<int>(type: "int", nullable: false),
                    RequiredResearchMonths = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrentRank = table.Column<int>(type: "int", nullable: false),
                    StartDateInCurrentRank = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaysInCurrentRank = table.Column<int>(type: "int", nullable: false),
                    UserTypeId = table.Column<int>(type: "int", nullable: false),
                    Works = table.Column<int>(type: "int", nullable: false),
                    EvaluationScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TrainingHours = table.Column<int>(type: "int", nullable: false),
                    ResearchMonths = table.Column<int>(type: "int", nullable: false),
                    YearsInCurrentRank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_UserTypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalTable: "UserTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademicDegrees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DegreeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IssuingInstitution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicDegrees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicDegrees_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IssuingInstitution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: true),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    RequirementId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "Requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Documents_Teachers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentObservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentObservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentObservations_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentObservations_Teachers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    CurrentRank = table.Column<int>(type: "int", nullable: false),
                    TargetRank = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionRequests_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PromotionRequests_Teachers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionRequests_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionObservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PromotionRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionObservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionObservations_PromotionRequests_PromotionRequestId",
                        column: x => x.PromotionRequestId,
                        principalTable: "PromotionRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Requirements",
                columns: new[] { "Id", "MinimumEvaluationScore", "Name", "RequiredResearchMonths", "RequiredTrainingHours", "RequiredWorks", "YearsInCurrentRank" },
                values: new object[,]
                {
                    { 1, 80m, "Requisitos para Titular 2", 0, 100, 1, 4 },
                    { 2, 85m, "Requisitos para Titular 3", 12, 150, 2, 4 },
                    { 3, 85m, "Requisitos para Titular 4", 24, 200, 3, 5 },
                    { 4, 90m, "Requisitos para Titular 5", 36, 250, 4, 5 }
                });

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Usuario con acceso total al sistema", "Administrador" },
                    { 2, "Docente que puede solicitar ascensos", "Docente" },
                    { 3, "Usuario que evalúa las solicitudes de ascenso", "Evaluador" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "CurrentRank", "DaysInCurrentRank", "Email", "EvaluationScore", "FirstName", "IdentificationNumber", "LastName", "Password", "ResearchMonths", "StartDateInCurrentRank", "TrainingHours", "UserTypeId", "Works", "YearsInCurrentRank" },
                values: new object[,]
                {
                    { 1, 1, 1440, "juan.perez@uta.edu.ec", 85m, "Juan", "0123456789", "Pérez", "Contraseña123", 0, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100, 2, 1, 4 },
                    { 2, 2, 1800, "maria.gomez@uta.edu.ec", 90m, "María", "9876543210", "Gómez", "Contraseña456", 18, new DateTime(2019, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 120, 2, 2, 5 },
                    { 3, 5, 3000, "carlos.rodriguez@uta.edu.ec", 95m, "Carlos", "1122334455", "Rodríguez", "Contraseña789", 60, new DateTime(2015, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 500, 3, 10, 8 }
                });

            migrationBuilder.InsertData(
                table: "AcademicDegrees",
                columns: new[] { "Id", "DegreeType", "IssuingInstitution", "TeacherId", "Title" },
                values: new object[,]
                {
                    { 1, "Maestría", "Universidad Técnica de Ambato", 1, "Máster en Ciencias de la Computación" },
                    { 2, "Doctorado", "Universidad de Barcelona", 2, "PhD en Ingeniería de Software" },
                    { 3, "Doctorado", "Universidad Politécnica de Madrid", 3, "PhD en Ciencias de la Computación" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicDegrees_TeacherId",
                table: "AcademicDegrees",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentObservations_DocumentId",
                table: "DocumentObservations",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentObservations_ReviewerId",
                table: "DocumentObservations",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_RequirementId",
                table: "Documents",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ReviewerId",
                table: "Documents",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TeacherId",
                table: "Documents",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionObservations_PromotionRequestId",
                table: "PromotionObservations",
                column: "PromotionRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRequests_DocumentId",
                table: "PromotionRequests",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRequests_ReviewerId",
                table: "PromotionRequests",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRequests_TeacherId",
                table: "PromotionRequests",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserTypeId",
                table: "Teachers",
                column: "UserTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicDegrees");

            migrationBuilder.DropTable(
                name: "DocumentObservations");

            migrationBuilder.DropTable(
                name: "PromotionObservations");

            migrationBuilder.DropTable(
                name: "PromotionRequests");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Requirements");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "UserTypes");
        }
    }
}
