using DirectorySync;
using Microsoft.Extensions.FileProviders;
using System.Net;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            var ip = IPAddress.Parse("0.0.0.0");
            var port = new Random().Next(short.MaxValue / 2, short.MaxValue);

            serverOptions.Listen(ip, port);
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMvc();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Props.SERVERPATH),
            RequestPath = "/s"
        });


        app.Run();
    }
}