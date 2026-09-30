using System.Security.Claims;
using LegalAI.Models;
using LegalAI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o => { o.LoginPath = "/login.html"; o.AccessDeniedPath = "/login.html"; o.ExpireTimeSpan = TimeSpan.FromHours(4); o.SlidingExpiration = true; });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddHttpClient<AiService>();
var app = builder.Build();
var db = app.Services.GetRequiredService<DatabaseService>();
db.Initialize();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/login.html"));

app.MapPost("/api/auth/login", async (LoginRequest req, DatabaseService database, HttpContext ctx) =>
{
    var user = database.FindUser(req.Username);
    if (user is null || !PasswordService.Verify(req.Password, user.PasswordHash))
        return Results.Unauthorized();
    var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role), new Claim("department", user.Department) };
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    database.AddAuditLog(user.Id, user.Username, "LOGIN", "User", user.Id); return Results.Ok(new { user = new { user.Username, user.Role, user.Department } });
});
app.MapPost("/api/auth/logout", async (HttpContext ctx, DatabaseService database) => { database.AddAuditLog(GetUserId(ctx), ctx.User.Identity?.Name ?? "unknown", "LOGOUT", "User", GetUserId(ctx)); await ctx.SignOutAsync(); return Results.Ok(); }).RequireAuthorization();
app.MapGet("/api/auth/me", (ClaimsPrincipal u) => Results.Ok(new { username = u.Identity?.Name, role = u.FindFirstValue(ClaimTypes.Role), department = u.FindFirstValue("department") })).RequireAuthorization();

var records = app.MapGroup("/api/ho-so").RequireAuthorization();
records.MapGet("", (HttpContext ctx, DatabaseService database, string? q, string? type, string? status, string? sort) =>
{
    var role = ctx.User.FindFirstValue(ClaimTypes.Role) ?? "Employee";
    var dept = ctx.User.FindFirstValue("department") ?? "";
    return Results.Ok(database.SearchRecords(q, type, status, sort, role == "Admin" ? null : dept));
});
records.MapGet("/{id:int}", (int id, HttpContext ctx, DatabaseService database) =>
{
    var r = database.GetRecord(id); if (r is null) return Results.NotFound();
    if (!CanAccess(ctx.User, r)) return Results.Forbid(); return Results.Ok(r);
});
records.MapPost("", (LegalRecord r, HttpContext ctx, DatabaseService database) =>
{
    if (string.IsNullOrWhiteSpace(r.Title) || string.IsNullOrWhiteSpace(r.Content) || string.IsNullOrWhiteSpace(r.RecordType) || string.IsNullOrWhiteSpace(r.Status)) return Results.BadRequest(new { message = "Tên, loại, nội dung và trạng thái hồ sơ là bắt buộc." });
    if (ctx.User.IsInRole("Employee")) r.Department = ctx.User.FindFirstValue("department") ?? r.Department;
    try { r.Id = database.AddRecord(r); database.AddAuditLog(GetUserId(ctx), ctx.User.Identity?.Name ?? "unknown", "CREATE", "LegalRecord", r.Id); return Results.Ok(database.GetRecord(r.Id)); } catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
});
records.MapPut("/{id:int}", (int id, LegalRecord r, HttpContext ctx, DatabaseService database) =>
{
    var old = database.GetRecord(id); if (old is null) return Results.NotFound();
    if (!CanAccess(ctx.User, old)) return Results.Forbid();
    if (string.IsNullOrWhiteSpace(r.Title) || string.IsNullOrWhiteSpace(r.Content) || string.IsNullOrWhiteSpace(r.RecordType) || string.IsNullOrWhiteSpace(r.Status)) return Results.BadRequest(new { message = "Tên, loại, nội dung và trạng thái hồ sơ là bắt buộc." });
    if (ctx.User.IsInRole("Employee")) r.Department = old.Department;
    r.Id = id; var ok = database.UpdateRecord(r); if (ok) database.AddAuditLog(GetUserId(ctx), ctx.User.Identity?.Name ?? "unknown", "UPDATE", "LegalRecord", id); return ok ? Results.Ok(database.GetRecord(id)) : Results.Problem("Không cập nhật được dữ liệu.");
});
records.MapDelete("/{id:int}", (int id, HttpContext ctx, DatabaseService database) =>
{
    if (!ctx.User.IsInRole("Admin")) return Results.Forbid();
    var ok = database.DeleteRecord(id); if (ok) database.AddAuditLog(GetUserId(ctx), ctx.User.Identity?.Name ?? "unknown", "DELETE", "LegalRecord", id); return ok ? Results.Ok() : Results.NotFound();
});

app.MapGet("/api/dashboard", (HttpContext ctx, DatabaseService database) =>
{
    var role = ctx.User.FindFirstValue(ClaimTypes.Role) ?? "Employee";
    var dept = ctx.User.FindFirstValue("department") ?? "";
    return Results.Ok(database.GetDashboard(role == "Admin" ? null : dept));
}).RequireAuthorization();
app.MapGet("/api/admin/audit-logs", (HttpContext ctx, DatabaseService database) => { if (!ctx.User.IsInRole("Admin")) return Results.Forbid(); return Results.Ok(database.GetAuditLogs()); }).RequireAuthorization();
app.MapGet("/api/canh-bao", (HttpContext ctx, DatabaseService database) =>
{
    var role = ctx.User.FindFirstValue(ClaimTypes.Role) ?? "Employee"; var dept = ctx.User.FindFirstValue("department") ?? "";
    return Results.Ok(database.GetExpiring(30, role == "Admin" ? null : dept));
}).RequireAuthorization();
app.MapGet("/api/admin/backup", (HttpContext ctx, DatabaseService database) =>
{
    if (!ctx.User.IsInRole("Admin")) return Results.Forbid();
    var path = database.Backup(); database.AddAuditLog(GetUserId(ctx), ctx.User.Identity?.Name ?? "unknown", "BACKUP", "Database", null); return Results.Ok(new { message = "Đã tạo bản sao lưu.", path });
}).RequireAuthorization();

var ai = app.MapGroup("/api/ai").RequireAuthorization();
ai.MapPost("/tom-tat/{id:int}", async (int id, HttpContext ctx, DatabaseService database, AiService service) =>
{
    var r = database.GetRecord(id); if (r is null) return Results.NotFound(); if (!CanAccess(ctx.User, r)) return Results.Forbid();
    database.AddAuditLog(GetUserId(ctx), ctx.User.Identity?.Name ?? "unknown", "AI_SUMMARY", "LegalRecord", id); return Results.Ok(new { result = await service.SummarizeAsync(r) });
});
ai.MapPost("/hoi-dap", async (ChatRequest request, HttpContext ctx, DatabaseService database, AiService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Question)) return Results.BadRequest(new { message = "Câu hỏi không được để trống." });
    var role = ctx.User.FindFirstValue(ClaimTypes.Role) ?? "Employee"; var dept = ctx.User.FindFirstValue("department") ?? "";
    var data = database.SearchRecords(null, null, null, null, role == "Admin" ? null : dept);
    database.AddAuditLog(GetUserId(ctx), ctx.User.Identity?.Name ?? "unknown", "AI_CHAT", "LegalRecord", null); return Results.Ok(new { result = await service.ChatAsync(request.Question, data) });
});

app.Run();
static int? GetUserId(HttpContext ctx) => int.TryParse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
static bool CanAccess(ClaimsPrincipal user, LegalRecord r) => user.IsInRole("Admin") || string.Equals(user.FindFirstValue("department"), r.Department, StringComparison.OrdinalIgnoreCase);
public record LoginRequest(string Username, string Password);
public record ChatRequest(string Question);
