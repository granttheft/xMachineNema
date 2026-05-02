using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XMachine.Persistence.Operational.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanningModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "planning");

            migrationBuilder.CreateTable(
                name: "production_plans",
                schema: "planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_no = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    plan_status = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    planned_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    planned_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    approved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    approval_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plan_slots",
                schema: "planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    job_execution_id = table.Column<Guid>(type: "uuid", nullable: true),
                    slot_no = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    slot_status = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    planned_qty = table.Column<int>(type: "integer", nullable: false),
                    planned_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    planned_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    estimated_duration_minutes = table.Column<int>(type: "integer", nullable: true),
                    setup_time_minutes = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_plan_slots_production_plans_production_plan_id",
                        column: x => x.production_plan_id,
                        principalSchema: "planning",
                        principalTable: "production_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_plan_slots_production_plan_id",
                schema: "planning",
                table: "plan_slots",
                column: "production_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_plan_slots_tenant_id_machine_id",
                schema: "planning",
                table: "plan_slots",
                columns: new[] { "tenant_id", "machine_id" });

            migrationBuilder.CreateIndex(
                name: "IX_plan_slots_tenant_id_planned_start_at",
                schema: "planning",
                table: "plan_slots",
                columns: new[] { "tenant_id", "planned_start_at" });

            migrationBuilder.CreateIndex(
                name: "IX_plan_slots_tenant_id_production_plan_id",
                schema: "planning",
                table: "plan_slots",
                columns: new[] { "tenant_id", "production_plan_id" });

            migrationBuilder.CreateIndex(
                name: "IX_plan_slots_tenant_id_slot_status",
                schema: "planning",
                table: "plan_slots",
                columns: new[] { "tenant_id", "slot_status" });

            migrationBuilder.CreateIndex(
                name: "IX_production_plans_tenant_id_plan_no",
                schema: "planning",
                table: "production_plans",
                columns: new[] { "tenant_id", "plan_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_production_plans_tenant_id_plan_status",
                schema: "planning",
                table: "production_plans",
                columns: new[] { "tenant_id", "plan_status" });

            migrationBuilder.CreateIndex(
                name: "IX_production_plans_tenant_id_planned_start_at",
                schema: "planning",
                table: "production_plans",
                columns: new[] { "tenant_id", "planned_start_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plan_slots",
                schema: "planning");

            migrationBuilder.DropTable(
                name: "production_plans",
                schema: "planning");
        }
    }
}
