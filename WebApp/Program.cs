using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
using WebApp;

var builder = WebApplication.CreateBuilder(args);
//builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath).AddOcelot();
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration,
  (o) =>
  {
      o.GenerateDocsDocsForGatewayItSelf(opt =>
      {
          //opt.FilePathsForXmlComments = { "MyAPI.xml" };
          opt.GatewayDocsTitle = "Demo Gateway";
          opt.GatewayDocsOpenApiInfo = new()
          {
              Title = "Demo Gateway",
              Version = "v1",
          };
          //opt.DocumentFilter<MyDocumentFilter>();
          opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
          {
              Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
              Name = "Authorization",
              In = ParameterLocation.Header,
              Type = SecuritySchemeType.ApiKey,
              Scheme = "Bearer"
          });
          opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
          {
              {
                  new OpenApiSecurityScheme
                  {
                      Reference = new OpenApiReference
                      {
                          Type = ReferenceType.SecurityScheme,
                          Id = "Bearer"
                      },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,
                  },
                  new List<string>()
              }
          });
      });
  });

//builder.Services.AddSwaggerGen(c =>
//{
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
//    {
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http,
//        Scheme = "Bearer"
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Demo Gateway API",
//        Version = "v1",
//    });
//});


builder.Services.AddJwtAuthentication();
builder.Services.AddAuthorization();


if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
}

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

await app.UseOcelot();
await app.RunAsync();
