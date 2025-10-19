using GerenciadorMateriais;
using GerenciadorMateriais.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// Endpoints de produtos

app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync());

app.MapGet("/produtos/{id}", async (int id, AppDbContext db) =>
    await db.Produtos.FindAsync(id)
        is Produto produto
            ? Results.Ok(produto)
            : Results.NotFound());

app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
{
    if (produto.Preco < 0 || produto.Quantidade < 0)
        return Results.BadRequest("Preço e quantidade devem ser positivos.");

    db.Produtos.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/produtos/{produto.Id}", produto);
});

app.MapPut("/produtos/{id}", async (int id, Produto inputProduto, AppDbContext db) =>
{
    var produto = await db.Produtos.FindAsync(id);
    if (produto is null) return Results.NotFound();

    if (inputProduto.Preco < 0 || inputProduto.Quantidade < 0)
        return Results.BadRequest("Preço e quantidade devem ser positivos.");

    produto.Nome = inputProduto.Nome;
    produto.Preco = inputProduto.Preco;
    produto.Quantidade = inputProduto.Quantidade;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/produtos/{id}", async (int id, int usuarioId, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(usuarioId);
    if (usuario == null) return Results.NotFound("Usuário não encontrado.");
    if (usuario.Perfil != "Admin") return Results.StatusCode(403);

    var produto = await db.Produtos.FindAsync(id);
    if (produto == null) return Results.NotFound();

    db.Produtos.Remove(produto);
    await db.SaveChangesAsync();
    return Results.Ok(produto);
});


// Endpoints de usuários

app.MapGet("/usuarios", async (AppDbContext db) => await db.Usuarios.ToListAsync());

app.MapGet("/usuarios/{id}", async (int id, AppDbContext db) =>
    await db.Usuarios.FindAsync(id)
        is Usuario usuario
            ? Results.Ok(usuario)
            : Results.NotFound());

app.MapPost("/usuarios", async (Usuario usuario, AppDbContext db) =>
{
    if (usuario.Perfil != "Admin" && usuario.Perfil != "Operacional")
        return Results.BadRequest("Perfil deve ser 'Admin' ou 'Operacional'.");

    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();
    return Results.Created($"/usuarios/{usuario.Id}", usuario);
});

app.MapPut("/usuarios/{id}", async (int id, Usuario inputUsuario, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario is null) return Results.NotFound();

    usuario.Nome = inputUsuario.Nome;
    usuario.Login = inputUsuario.Login;
    usuario.Senha = inputUsuario.Senha;
    usuario.Perfil = inputUsuario.Perfil;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/usuarios/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Usuarios.FindAsync(id) is Usuario usuario)
    {
        db.Usuarios.Remove(usuario);
        await db.SaveChangesAsync();
        return Results.Ok(usuario);
    }
    return Results.NotFound();
});


// Endpoints de movimentações

app.MapGet("/movimentacoes", async (int usuarioId, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(usuarioId);
    if (usuario == null) return Results.NotFound("Usuário não encontrado.");
    if (usuario.Perfil != "Admin") return Results.StatusCode(403);

    var movimentacoes = await db.Movimentacoes
        .Include(m => m.Produto)
        .Include(m => m.Usuario)
        .ToListAsync();

    return Results.Ok(movimentacoes);
});

app.MapPost("/movimentacoes/entrada", async (MovimentacaoRequest request, AppDbContext db) =>
{
    if (request.QuantidadeMovimentada <= 0)
        return Results.BadRequest("Quantidade deve ser maior que zero.");

    var produto = await db.Produtos.FindAsync(request.ProdutoId);
    if (produto is null) return Results.NotFound("Produto não encontrado.");

    var usuario = await db.Usuarios.FindAsync(request.UsuarioId);
    if (usuario is null) return Results.NotFound("Usuário não encontrado.");

    produto.Quantidade += request.QuantidadeMovimentada;
    
    var movimentacao = new Movimentacao
    {
        ProdutoId = request.ProdutoId,
        UsuarioId = request.UsuarioId,
        QuantidadeMovimentada = request.QuantidadeMovimentada,
        Tipo = "Entrada",
        DataHora = DateTime.Now
    };

    db.Movimentacoes.Add(movimentacao);
    await db.SaveChangesAsync();

    return Results.Created($"/movimentacoes/{movimentacao.Id}", movimentacao);
});

app.MapPost("/movimentacoes/saida", async (MovimentacaoRequest request, AppDbContext db) =>
{
    if (request.QuantidadeMovimentada <= 0)
        return Results.BadRequest("Quantidade deve ser maior que zero.");

    var produto = await db.Produtos.FindAsync(request.ProdutoId);
    if (produto is null) return Results.NotFound("Produto não encontrado.");

    var usuario = await db.Usuarios.FindAsync(request.UsuarioId);
    if (usuario is null) return Results.NotFound("Usuário não encontrado.");

    if (produto.Quantidade < request.QuantidadeMovimentada)
        return Results.BadRequest("Quantidade em estoque insuficiente.");

    produto.Quantidade -= request.QuantidadeMovimentada;
    
    var movimentacao = new Movimentacao
    {
        ProdutoId = request.ProdutoId,
        UsuarioId = request.UsuarioId,
        QuantidadeMovimentada = request.QuantidadeMovimentada,
        Tipo = "Saída",
        DataHora = DateTime.Now
    };

    db.Movimentacoes.Add(movimentacao);
    await db.SaveChangesAsync();

    return Results.Created($"/movimentacoes/{movimentacao.Id}", movimentacao);
});


// Endpoint de login

app.MapPost("/login", async (LoginRequest request, AppDbContext db) =>
{
    var usuario = await db.Usuarios
        .FirstOrDefaultAsync(u => u.Login == request.Login && u.Senha == request.Senha);

    if (usuario == null)
        return Results.Unauthorized();

    return Results.Ok(new
    {
        usuario.Id,
        usuario.Nome,
        usuario.Perfil
    });
});

app.Run();