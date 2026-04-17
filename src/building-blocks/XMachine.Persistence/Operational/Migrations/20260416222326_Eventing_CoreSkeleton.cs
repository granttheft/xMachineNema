using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XMachine.Persistence.Operational.Migrations
{
    /// <inheritdoc />
    public partial class Eventing_CoreSkeleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "eventing");

            migrationBuilder.CreateTable(
                name: "alarm_events",
                schema: "eventing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    alarm_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    alarm_text = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    duration_ms = table.Column<long>(type: "bigint", nullable: true),
                    ack_by = table.Column<Guid>(type: "uuid", nullable: true),
                    ack_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    alarm_status = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alarm_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_alarm_events_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_alarm_events_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_alarm_events_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "platform",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_alarm_events_user_accounts_ack_by",
                        column: x => x.ack_by,
                        principalSchema: "auth",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "downtime_records",
                schema: "eventing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    downtime_reason_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    downtime_reason_text = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    planned_flag = table.Column<bool>(type: "boolean", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    duration_ms = table.Column<long>(type: "bigint", nullable: true),
                    entered_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_downtime_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_downtime_records_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_downtime_records_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_downtime_records_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalSchema: "mes",
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_downtime_records_user_accounts_entered_by",
                        column: x => x.entered_by,
                        principalSchema: "auth",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "kpi_definitions",
                schema: "eventing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    formula_expression = table.Column<string>(type: "text", nullable: true),
                    scope_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    data_source_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kpi_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "oee_snapshots",
                schema: "eventing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    period_type = table.Column<int>(type: "integer", nullable: false),
                    period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    availability = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    performance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    quality = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    oee_value = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oee_snapshots", x => x.id);
                    table.ForeignKey(
                        name: "FK_oee_snapshots_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_oee_snapshots_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_oee_snapshots_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "platform",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "kpi_results",
                schema: "eventing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kpi_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    scope_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kpi_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_kpi_results_kpi_definitions_kpi_definition_id",
                        column: x => x.kpi_definition_id,
                        principalSchema: "eventing",
                        principalTable: "kpi_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alarm_events_ack_by",
                schema: "eventing",
                table: "alarm_events",
                column: "ack_by");

            migrationBuilder.CreateIndex(
                name: "IX_alarm_events_line_id",
                schema: "eventing",
                table: "alarm_events",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_alarm_events_machine_id",
                schema: "eventing",
                table: "alarm_events",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_alarm_events_site_id",
                schema: "eventing",
                table: "alarm_events",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_alarm_events_tenant_id_start_time",
                schema: "eventing",
                table: "alarm_events",
                columns: new[] { "tenant_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "IX_downtime_records_entered_by",
                schema: "eventing",
                table: "downtime_records",
                column: "entered_by");

            migrationBuilder.CreateIndex(
                name: "IX_downtime_records_line_id",
                schema: "eventing",
                table: "downtime_records",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_downtime_records_machine_id",
                schema: "eventing",
                table: "downtime_records",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_downtime_records_production_order_id",
                schema: "eventing",
                table: "downtime_records",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_downtime_records_tenant_id_start_time",
                schema: "eventing",
                table: "downtime_records",
                columns: new[] { "tenant_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "IX_kpi_definitions_tenant_id_code",
                schema: "eventing",
                table: "kpi_definitions",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kpi_results_kpi_definition_id_scope_type_scope_id_period_st~",
                schema: "eventing",
                table: "kpi_results",
                columns: new[] { "kpi_definition_id", "scope_type", "scope_id", "period_start" });

            migrationBuilder.CreateIndex(
                name: "IX_oee_snapshots_line_id",
                schema: "eventing",
                table: "oee_snapshots",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_oee_snapshots_machine_id",
                schema: "eventing",
                table: "oee_snapshots",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_oee_snapshots_site_id",
                schema: "eventing",
                table: "oee_snapshots",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_oee_snapshots_tenant_id_period_start_period_type",
                schema: "eventing",
                table: "oee_snapshots",
                columns: new[] { "tenant_id", "period_start", "period_type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alarm_events",
                schema: "eventing");

            migrationBuilder.DropTable(
                name: "downtime_records",
                schema: "eventing");

            migrationBuilder.DropTable(
                name: "kpi_results",
                schema: "eventing");

            migrationBuilder.DropTable(
                name: "oee_snapshots",
                schema: "eventing");

            migrationBuilder.DropTable(
                name: "kpi_definitions",
                schema: "eventing");
        }
    }
}
