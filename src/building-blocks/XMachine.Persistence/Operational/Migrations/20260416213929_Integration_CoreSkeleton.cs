using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XMachine.Persistence.Operational.Migrations
{
    /// <inheritdoc />
    public partial class Integration_CoreSkeleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integration");

            migrationBuilder.CreateTable(
                name: "connector_definitions",
                schema: "integration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    supports_read = table.Column<bool>(type: "boolean", nullable: false),
                    supports_write = table.Column<bool>(type: "boolean", nullable: false),
                    capabilities_json = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connector_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "connector_instances",
                schema: "integration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    connector_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    configuration_json = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connector_instances", x => x.id);
                    table.ForeignKey(
                        name: "FK_connector_instances_connector_definitions_connector_definit~",
                        column: x => x.connector_definition_id,
                        principalSchema: "integration",
                        principalTable: "connector_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_connector_instances_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "platform",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asset_tag_maps",
                schema: "integration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    connector_instance_id = table.Column<Guid>(type: "uuid", nullable: false),
                    machine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_address = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    canonical_field = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_tag_maps", x => x.id);
                    table.ForeignKey(
                        name: "FK_asset_tag_maps_connector_instances_connector_instance_id",
                        column: x => x.connector_instance_id,
                        principalSchema: "integration",
                        principalTable: "connector_instances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asset_tag_maps_machines_machine_id",
                        column: x => x.machine_id,
                        principalSchema: "platform",
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mapping_profiles",
                schema: "integration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    connector_instance_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapping_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_mapping_profiles_connector_instances_connector_instance_id",
                        column: x => x.connector_instance_id,
                        principalSchema: "integration",
                        principalTable: "connector_instances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sync_jobs",
                schema: "integration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    connector_instance_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    job_status = table.Column<int>(type: "integer", nullable: false),
                    payload_json = table.Column<string>(type: "text", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_jobs", x => x.id);
                    table.ForeignKey(
                        name: "FK_sync_jobs_connector_instances_connector_instance_id",
                        column: x => x.connector_instance_id,
                        principalSchema: "integration",
                        principalTable: "connector_instances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mapping_rules",
                schema: "integration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    mapping_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_field = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    logical_field = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    canonical_field = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    transform_kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapping_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_mapping_rules_mapping_profiles_mapping_profile_id",
                        column: x => x.mapping_profile_id,
                        principalSchema: "integration",
                        principalTable: "mapping_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_asset_tag_maps_connector_instance_id",
                schema: "integration",
                table: "asset_tag_maps",
                column: "connector_instance_id");

            migrationBuilder.CreateIndex(
                name: "IX_asset_tag_maps_machine_id",
                schema: "integration",
                table: "asset_tag_maps",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "IX_asset_tag_maps_tenant_id_connector_instance_id_source_addre~",
                schema: "integration",
                table: "asset_tag_maps",
                columns: new[] { "tenant_id", "connector_instance_id", "source_address" });

            migrationBuilder.CreateIndex(
                name: "IX_connector_definitions_tenant_id_code",
                schema: "integration",
                table: "connector_definitions",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_connector_instances_connector_definition_id",
                schema: "integration",
                table: "connector_instances",
                column: "connector_definition_id");

            migrationBuilder.CreateIndex(
                name: "IX_connector_instances_site_id",
                schema: "integration",
                table: "connector_instances",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_connector_instances_tenant_id_code",
                schema: "integration",
                table: "connector_instances",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mapping_profiles_connector_instance_id",
                schema: "integration",
                table: "mapping_profiles",
                column: "connector_instance_id");

            migrationBuilder.CreateIndex(
                name: "IX_mapping_profiles_tenant_id_connector_instance_id_name_versi~",
                schema: "integration",
                table: "mapping_profiles",
                columns: new[] { "tenant_id", "connector_instance_id", "name", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mapping_rules_mapping_profile_id_sort_order",
                schema: "integration",
                table: "mapping_rules",
                columns: new[] { "mapping_profile_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_sync_jobs_connector_instance_id",
                schema: "integration",
                table: "sync_jobs",
                column: "connector_instance_id");

            migrationBuilder.CreateIndex(
                name: "IX_sync_jobs_tenant_id_connector_instance_id_job_status",
                schema: "integration",
                table: "sync_jobs",
                columns: new[] { "tenant_id", "connector_instance_id", "job_status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asset_tag_maps",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "mapping_rules",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "sync_jobs",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "mapping_profiles",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "connector_instances",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "connector_definitions",
                schema: "integration");
        }
    }
}
