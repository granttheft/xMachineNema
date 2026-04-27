using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XMachine.Persistence.Operational.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "production");

            migrationBuilder.CreateTable(
                name: "job_executions",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                    shift_id = table.Column<Guid>(type: "uuid", nullable: true),
                    operator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    job_no = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    execution_status = table.Column<int>(type: "integer", nullable: false),
                    planned_qty = table.Column<int>(type: "integer", nullable: false),
                    produced_qty = table.Column<int>(type: "integer", nullable: false),
                    scrap_qty = table.Column<int>(type: "integer", nullable: false),
                    defect_qty = table.Column<int>(type: "integer", nullable: false),
                    planned_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    planned_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    actual_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    actual_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    paused_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    pause_reason = table.Column<int>(type: "integer", nullable: true),
                    pause_notes = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    notes = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_executions", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_executions_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_job_executions_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalSchema: "mes",
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "operator_declarations",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_execution_id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    declared_qty = table.Column<int>(type: "integer", nullable: false),
                    scrap_qty = table.Column<int>(type: "integer", nullable: false),
                    defect_qty = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    declared_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operator_declarations", x => x.id);
                    table.ForeignKey(
                        name: "FK_operator_declarations_job_executions_job_execution_id",
                        column: x => x.job_execution_id,
                        principalSchema: "production",
                        principalTable: "job_executions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_executions_machine_id",
                schema: "production",
                table: "job_executions",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_executions_production_order_id",
                schema: "production",
                table: "job_executions",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_executions_tenant_id_execution_status",
                schema: "production",
                table: "job_executions",
                columns: new[] { "tenant_id", "execution_status" });

            migrationBuilder.CreateIndex(
                name: "IX_job_executions_tenant_id_job_no",
                schema: "production",
                table: "job_executions",
                columns: new[] { "tenant_id", "job_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_executions_tenant_id_machine_id",
                schema: "production",
                table: "job_executions",
                columns: new[] { "tenant_id", "machine_id" });

            migrationBuilder.CreateIndex(
                name: "IX_job_executions_tenant_id_production_order_id",
                schema: "production",
                table: "job_executions",
                columns: new[] { "tenant_id", "production_order_id" });

            migrationBuilder.CreateIndex(
                name: "IX_job_executions_tenant_id_shift_id",
                schema: "production",
                table: "job_executions",
                columns: new[] { "tenant_id", "shift_id" });

            migrationBuilder.CreateIndex(
                name: "IX_operator_declarations_job_execution_id",
                schema: "production",
                table: "operator_declarations",
                column: "job_execution_id");

            migrationBuilder.CreateIndex(
                name: "IX_operator_declarations_tenant_id_declared_at",
                schema: "production",
                table: "operator_declarations",
                columns: new[] { "tenant_id", "declared_at" });

            migrationBuilder.CreateIndex(
                name: "IX_operator_declarations_tenant_id_job_execution_id",
                schema: "production",
                table: "operator_declarations",
                columns: new[] { "tenant_id", "job_execution_id" });

            migrationBuilder.CreateIndex(
                name: "IX_operator_declarations_tenant_id_operator_id",
                schema: "production",
                table: "operator_declarations",
                columns: new[] { "tenant_id", "operator_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operator_declarations",
                schema: "production");

            migrationBuilder.DropTable(
                name: "job_executions",
                schema: "production");
        }
    }
}
