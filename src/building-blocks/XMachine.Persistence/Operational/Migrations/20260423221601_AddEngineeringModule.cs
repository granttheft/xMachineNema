using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XMachine.Persistence.Operational.Migrations
{
    /// <inheritdoc />
    public partial class AddEngineeringModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "engineering");

            migrationBuilder.AddColumn<string>(
                name: "location",
                schema: "platform",
                table: "machines",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "machine_type",
                schema: "platform",
                table: "machines",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "operational_status",
                schema: "platform",
                table: "machines",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateTable(
                name: "maintenance_work_orders",
                schema: "engineering",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    work_order_no = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    work_order_type = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    work_order_status = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    reason_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    assigned_to = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    reported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reported_by = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closing_notes = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintenance_work_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_maintenance_work_orders_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pm_schedules",
                schema: "engineering",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    interval_hours = table.Column<int>(type: "integer", nullable: true),
                    interval_days = table.Column<int>(type: "integer", nullable: true),
                    last_done_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    next_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    owner_role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pm_schedules", x => x.id);
                    table.ForeignKey(
                        name: "FK_pm_schedules_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "machine_faults",
                schema: "engineering",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    maintenance_work_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fault_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    reported_by = table.Column<Guid>(type: "uuid", nullable: false),
                    reported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    resolved = table.Column<bool>(type: "boolean", nullable: false),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_machine_faults", x => x.id);
                    table.ForeignKey(
                        name: "FK_machine_faults_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_machine_faults_maintenance_work_orders_maintenance_work_ord~",
                        column: x => x.maintenance_work_order_id,
                        principalSchema: "engineering",
                        principalTable: "maintenance_work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_machines_tenant_id_operational_status",
                schema: "platform",
                table: "machines",
                columns: new[] { "tenant_id", "operational_status" });

            migrationBuilder.CreateIndex(
                name: "IX_machine_faults_machine_id",
                schema: "engineering",
                table: "machine_faults",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_machine_faults_maintenance_work_order_id",
                schema: "engineering",
                table: "machine_faults",
                column: "maintenance_work_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_machine_faults_tenant_id_machine_id",
                schema: "engineering",
                table: "machine_faults",
                columns: new[] { "tenant_id", "machine_id" });

            migrationBuilder.CreateIndex(
                name: "IX_machine_faults_tenant_id_reported_at",
                schema: "engineering",
                table: "machine_faults",
                columns: new[] { "tenant_id", "reported_at" });

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_work_orders_machine_id",
                schema: "engineering",
                table: "maintenance_work_orders",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_work_orders_tenant_id_machine_id",
                schema: "engineering",
                table: "maintenance_work_orders",
                columns: new[] { "tenant_id", "machine_id" });

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_work_orders_tenant_id_work_order_no",
                schema: "engineering",
                table: "maintenance_work_orders",
                columns: new[] { "tenant_id", "work_order_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_work_orders_tenant_id_work_order_status",
                schema: "engineering",
                table: "maintenance_work_orders",
                columns: new[] { "tenant_id", "work_order_status" });

            migrationBuilder.CreateIndex(
                name: "IX_pm_schedules_machine_id",
                schema: "engineering",
                table: "pm_schedules",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_pm_schedules_tenant_id_machine_id",
                schema: "engineering",
                table: "pm_schedules",
                columns: new[] { "tenant_id", "machine_id" });

            migrationBuilder.CreateIndex(
                name: "IX_pm_schedules_tenant_id_next_due_at",
                schema: "engineering",
                table: "pm_schedules",
                columns: new[] { "tenant_id", "next_due_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "machine_faults",
                schema: "engineering");

            migrationBuilder.DropTable(
                name: "pm_schedules",
                schema: "engineering");

            migrationBuilder.DropTable(
                name: "maintenance_work_orders",
                schema: "engineering");

            migrationBuilder.DropIndex(
                name: "IX_machines_tenant_id_operational_status",
                schema: "platform",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "location",
                schema: "platform",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "machine_type",
                schema: "platform",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "operational_status",
                schema: "platform",
                table: "machines");
        }
    }
}
