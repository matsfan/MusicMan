var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MusicMan_WebApi>("musicman-web");

builder.Build().Run();
