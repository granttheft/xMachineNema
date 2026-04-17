using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XMachine.Persistence.Operational.Migrations
{
    /// <inheritdoc />
    public partial class Mes_CoreSkeleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mes");

            migrationBuilder.CreateTable(
                name: "production_orders",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_no = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    product_code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    quantity_planned = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    quantity_completed = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    order_status = table.Column<int>(type: "integer", nullable: false),
                    planned_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    planned_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    actual_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    actual_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    source_system = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    source_reference = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_production_orders_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_production_orders_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "platform",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shifts",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    shift_date = table.Column<DateOnly>(type: "date", nullable: false),
                    planned_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    planned_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    actual_start_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    actual_end_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lifecycle = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shifts", x => x.id);
                    table.ForeignKey(
                        name: "FK_shifts_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shifts_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "platform",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lot_batches",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lot_no = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    lot_status = table.Column<int>(type: "integer", nullable: false),
                    target_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    quantity_good = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lot_batches", x => x.id);
                    table.ForeignKey(
                        name: "FK_lot_batches_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lot_batches_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lot_batches_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalSchema: "mes",
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "production_operations",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence_no = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    operation_status = table.Column<int>(type: "integer", nullable: false),
                    quantity_planned = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    quantity_completed = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_operations", x => x.id);
                    table.ForeignKey(
                        name: "FK_production_operations_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_production_operations_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_production_operations_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalSchema: "mes",
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_recipe_assignments",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_version_assigned = table.Column<int>(type: "integer", nullable: false),
                    assigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_recipe_assignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_recipe_assignments_production_orders_production_order~",
                        column: x => x.production_order_id,
                        principalSchema: "mes",
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_recipe_assignments_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "mes",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recipe_parameters",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    data_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    unit = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    default_value = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    min_value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    max_value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_parameters", x => x.id);
                    table.ForeignKey(
                        name: "FK_recipe_parameters_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "mes",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_assignments",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shift_id = table.Column<Guid>(type: "uuid", nullable: true),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    assigned_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    assignment_role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_assignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_assignments_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employee_assignments_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employee_assignments_shifts_shift_id",
                        column: x => x.shift_id,
                        principalSchema: "mes",
                        principalTable: "shifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_employee_assignments_user_accounts_user_account_id",
                        column: x => x.user_account_id,
                        principalSchema: "auth",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inventory_movements",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    movement_type = table.Column<int>(type: "integer", nullable: false),
                    material_code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    lot_batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    production_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reference_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_movements", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_movements_lot_batches_lot_batch_id",
                        column: x => x.lot_batch_id,
                        principalSchema: "mes",
                        principalTable: "lot_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_inventory_movements_production_orders_production_order_id",
                        column: x => x.production_order_id,
                        principalSchema: "mes",
                        principalTable: "production_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "material_consumptions",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    material_code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    material_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    consumed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_material_consumptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_material_consumptions_lot_batches_lot_batch_id",
                        column: x => x.lot_batch_id,
                        principalSchema: "mes",
                        principalTable: "lot_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "production_declarations",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    good_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    declared_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_production_declarations", x => x.id);
                    table.ForeignKey(
                        name: "FK_production_declarations_lines_line_id",
                        column: x => x.line_id,
                        principalSchema: "platform",
                        principalTable: "lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_production_declarations_lot_batches_lot_batch_id",
                        column: x => x.lot_batch_id,
                        principalSchema: "mes",
                        principalTable: "lot_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_production_declarations_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "scrap_declarations",
                schema: "mes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scrap_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    reason_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    declared_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_scrap_declarations", x => x.id);
                    table.ForeignKey(
                        name: "FK_scrap_declarations_lot_batches_lot_batch_id",
                        column: x => x.lot_batch_id,
                        principalSchema: "mes",
                        principalTable: "lot_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_assignments_line_id",
                schema: "mes",
                table: "employee_assignments",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_assignments_machine_id",
                schema: "mes",
                table: "employee_assignments",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_assignments_shift_id",
                schema: "mes",
                table: "employee_assignments",
                column: "shift_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_assignments_tenant_id_user_account_id_shift_id",
                schema: "mes",
                table: "employee_assignments",
                columns: new[] { "tenant_id", "user_account_id", "shift_id" });

            migrationBuilder.CreateIndex(
                name: "IX_employee_assignments_user_account_id",
                schema: "mes",
                table: "employee_assignments",
                column: "user_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_lot_batch_id",
                schema: "mes",
                table: "inventory_movements",
                column: "lot_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_production_order_id",
                schema: "mes",
                table: "inventory_movements",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_tenant_id_occurred_at",
                schema: "mes",
                table: "inventory_movements",
                columns: new[] { "tenant_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "IX_lot_batches_line_id",
                schema: "mes",
                table: "lot_batches",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_lot_batches_machine_id",
                schema: "mes",
                table: "lot_batches",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_lot_batches_production_order_id",
                schema: "mes",
                table: "lot_batches",
                column: "production_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_lot_batches_tenant_id_lot_no",
                schema: "mes",
                table: "lot_batches",
                columns: new[] { "tenant_id", "lot_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_material_consumptions_lot_batch_id",
                schema: "mes",
                table: "material_consumptions",
                column: "lot_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_recipe_assignments_production_order_id_recipe_id",
                schema: "mes",
                table: "order_recipe_assignments",
                columns: new[] { "production_order_id", "recipe_id" });

            migrationBuilder.CreateIndex(
                name: "IX_order_recipe_assignments_recipe_id",
                schema: "mes",
                table: "order_recipe_assignments",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_declarations_line_id",
                schema: "mes",
                table: "production_declarations",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_declarations_lot_batch_id",
                schema: "mes",
                table: "production_declarations",
                column: "lot_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_declarations_machine_id",
                schema: "mes",
                table: "production_declarations",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_operations_line_id",
                schema: "mes",
                table: "production_operations",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_operations_machine_id",
                schema: "mes",
                table: "production_operations",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_operations_production_order_id_sequence_no",
                schema: "mes",
                table: "production_operations",
                columns: new[] { "production_order_id", "sequence_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_production_orders_line_id",
                schema: "mes",
                table: "production_orders",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_orders_site_id",
                schema: "mes",
                table: "production_orders",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_production_orders_tenant_id_order_no",
                schema: "mes",
                table: "production_orders",
                columns: new[] { "tenant_id", "order_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipe_parameters_recipe_id_code",
                schema: "mes",
                table: "recipe_parameters",
                columns: new[] { "recipe_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipes_tenant_id_code_version",
                schema: "mes",
                table: "recipes",
                columns: new[] { "tenant_id", "code", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scrap_declarations_lot_batch_id",
                schema: "mes",
                table: "scrap_declarations",
                column: "lot_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_line_id",
                schema: "mes",
                table: "shifts",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_site_id",
                schema: "mes",
                table: "shifts",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_tenant_id_site_id_shift_date_code",
                schema: "mes",
                table: "shifts",
                columns: new[] { "tenant_id", "site_id", "shift_date", "code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_assignments",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "inventory_movements",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "material_consumptions",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "order_recipe_assignments",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "production_declarations",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "production_operations",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "recipe_parameters",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "scrap_declarations",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "shifts",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "recipes",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "lot_batches",
                schema: "mes");

            migrationBuilder.DropTable(
                name: "production_orders",
                schema: "mes");
        }
    }
}
