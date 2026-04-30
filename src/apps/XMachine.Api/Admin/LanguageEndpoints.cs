using Microsoft.EntityFrameworkCore;
using XMachine.Module.Auth.Security;
using XMachine.Module.Platform.Domain;
using XMachine.Persistence.Operational;
using XMachine.SharedKernel;

namespace XMachine.Api.Admin;

/// <summary>Request body for creating or updating a tenant translation override.</summary>
public sealed record UpsertTranslationOverrideRequest(string Key, string Value);

/// <summary>Minimal API endpoints for tenant translation overrides (admin).</summary>
public static class LanguageEndpoints
{
    private static readonly string[] SupportedCodes = ["en", "tr", "de", "pl"];

    private static readonly Dictionary<string, string> DefaultLanguageNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = "English",
        ["tr"] = "Türkçe",
        ["de"] = "Deutsch",
        ["pl"] = "Polski",
    };

    /// <summary>Registers <c>/api/admin/languages*</c> routes.</summary>
    /// <param name="app">Host application.</param>
    public static void MapLanguageEndpoints(this WebApplication app)
    {
        var g = app.MapGroup("/api/admin").RequireAuthorization(AuthPolicies.AdminArea);

        g.MapGet("languages", async (XMachineDbContext db, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (currentUser.TenantId is null)
                return Results.Unauthorized();

            var tenantId = currentUser.TenantId.Value;

            var activeCodes = await db.TenantTranslationOverrides.AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.Status == EntityStatus.Active)
                .Select(x => x.LanguageCode)
                .ToListAsync(ct);

            var countByCode = SupportedCodes.ToDictionary(
                c => c,
                c => activeCodes.Count(x => string.Equals(x, c, StringComparison.OrdinalIgnoreCase)),
                StringComparer.OrdinalIgnoreCase);

            var list = SupportedCodes.Select(code => new
            {
                code,
                name = DefaultLanguageNames.GetValueOrDefault(code, code),
                overrideCount = countByCode.GetValueOrDefault(code, 0),
            }).ToList();

            return Results.Ok(list);
        });

        g.MapGet("languages/{code}/overrides", async (
            string code,
            XMachineDbContext db,
            ICurrentUser currentUser,
            CancellationToken ct) =>
        {
            if (!IsSupportedCode(code))
                return Results.BadRequest(new { error = "Unsupported language code." });

            if (currentUser.TenantId is null)
                return Results.Unauthorized();

            var tenantId = currentUser.TenantId.Value;

            var rows = await db.TenantTranslationOverrides.AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.LanguageCode == code &&
                    x.Status == EntityStatus.Active)
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    x.Id,
                    x.Key,
                    Value = x.Value,
                    languageCode = x.LanguageCode,
                })
                .ToListAsync(ct);

            return Results.Ok(rows);
        });

        g.MapPost("languages/{code}/overrides", async (
            string code,
            UpsertTranslationOverrideRequest body,
            XMachineDbContext db,
            ICurrentUser currentUser,
            CancellationToken ct) =>
        {
            if (!IsSupportedCode(code))
                return Results.BadRequest(new { error = "Unsupported language code." });

            if (currentUser.TenantId is null)
                return Results.Unauthorized();

            var key = (body.Key ?? string.Empty).Trim();
            var value = (body.Value ?? string.Empty).Trim();
            if (key.Length == 0 || value.Length == 0)
                return Results.BadRequest(new { error = "Key and value are required." });

            var tenantId = currentUser.TenantId.Value;
            var now = DateTimeOffset.UtcNow;
            var userId = currentUser.UserId;

            var existing = await db.TenantTranslationOverrides
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId && x.LanguageCode == code && x.Key == key,
                    ct);

            if (existing is null)
            {
                var entity = new TenantTranslationOverride
                {
                    TenantId = tenantId,
                    LanguageCode = code,
                    Key = key,
                    Value = value,
                    Status = EntityStatus.Active,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                };
                db.TenantTranslationOverrides.Add(entity);
                await db.SaveChangesAsync(ct);
                return Results.Ok(new { entity.Id, key = entity.Key, value = entity.Value });
            }

            existing.Value = value;
            existing.Status = EntityStatus.Active;
            existing.UpdatedAt = now;
            existing.UpdatedBy = userId;
            await db.SaveChangesAsync(ct);
            return Results.Ok(new { existing.Id, key = existing.Key, value = existing.Value });
        });

        g.MapDelete("languages/{code}/overrides/{key}", async (
            string code,
            string key,
            XMachineDbContext db,
            ICurrentUser currentUser,
            CancellationToken ct) =>
        {
            if (!IsSupportedCode(code))
                return Results.BadRequest();

            if (currentUser.TenantId is null)
                return Results.Unauthorized();

            var tenantId = currentUser.TenantId.Value;
            var decodedKey = Uri.UnescapeDataString(key);

            var entity = await db.TenantTranslationOverrides
                .FirstOrDefaultAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.LanguageCode == code &&
                        x.Key == decodedKey &&
                        x.Status == EntityStatus.Active,
                    ct);

            if (entity is null)
                return Results.NotFound();

            var now = DateTimeOffset.UtcNow;
            entity.Status = EntityStatus.Inactive;
            entity.UpdatedAt = now;
            entity.UpdatedBy = currentUser.UserId;
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });
    }

    private static bool IsSupportedCode(string code) =>
        SupportedCodes.Contains(code, StringComparer.OrdinalIgnoreCase);
}
