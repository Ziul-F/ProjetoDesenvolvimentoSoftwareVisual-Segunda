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

// Endpoints de Produtos
app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync());

app.MapGet("/produtos/{id}", async (int id, AppDbContext db) =>
    await db.Produtos.FindAsync(id)
        is Produto produto
            ? Results.Ok(produto)
            : Results.NotFound());

app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
{
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/produtos/{produto.Id}", produto);
});

app.MapPut("/produtos/{id}", async (int id, Produto inputProduto, AppDbContext db) =>
{
    var produto = await db.Produtos.FindAsync(id);

    if (produto is null) return Results.NotFound();

    produto.Nome = inputProduto.Nome;
    produto.Preco = inputProduto.Preco;
    produto.Quantidade = inputProduto.Quantidade;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/produtos/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Produtos.FindAsync(id) is Produto produto)
    {
        db.Produtos.Remove(produto);
        await db.SaveChangesAsync();
        return Results.Ok(produto);
    }

    return Results.NotFound();
});

// Endpoints de Usuários
app.MapGet("/usuarios", async (AppDbContext db) => await db.Usuarios.ToListAsync());

app.MapGet("/usuarios/{id}", async (int id, AppDbContext db) =>
    await db.Usuarios.FindAsync(id)
        is Usuario usuario
            ? Results.Ok(usuario)
            : Results.NotFound());

app.MapPost("/usuarios", async (Usuario usuario, AppDbContext db) =>
{
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


// Endpoints de Movimentações
app.MapGet("/movimentacoes", async (AppDbContext db) =>
    await db.Movimentacoes.Include(m => m.Produto).Include(m => m.Usuario).ToListAsync());

app.MapPost("/movimentacoes/entrada", async (Movimentacao movimentacao, AppDbContext db) =>
{
    var produto = await db.Produtos.FindAsync(movimentacao.ProdutoId);
    if (produto is null) return Results.NotFound("Produto não encontrado.");

    var usuario = await db.Usuarios.FindAsync(movimentacao.UsuarioId);
    if (usuario is null) return Results.NotFound("Usuário não encontrado.");

    produto.Quantidade += movimentacao.QuantidadeMovimentada;

    movimentacao.Tipo = "Entrada";
    movimentacao.DataHora = DateTime.Now;

    db.Movimentacoes.Add(movimentacao);
    await db.SaveChangesAsync();

    return Results.Created($"/movimentacoes/{movimentacao.Id}", movimentacao);
});

app.MapPost("/movimentacoes/saida", async (Movimentacao movimentacao, AppDbContext db) =>
{
    var produto = await db.Produtos.FindAsync(movimentacao.ProdutoId);
    if (produto is null) return Results.NotFound("Produto não encontrado.");

    var usuario = await db.Usuarios.FindAsync(movimentacao.UsuarioId);
    if (usuario is null) return Results.NotFound("Usuário não encontrado.");

    if (produto.Quantidade < movimentacao.QuantidadeMovimentada)
    {
        return Results.BadRequest("Quantidade em estoque insuficiente.");
    }

    produto.Quantidade -= movimentacao.QuantidadeMovimentada;

    movimentacao.Tipo = "Saída";
    movimentacao.DataHora = DateTime.Now;

    db.Movimentacoes.Add(movimentacao);
    await db.SaveChangesAsync();

    return Results.Created($"/movimentacoes/{movimentacao.Id}", movimentacao);
});


app.Run();