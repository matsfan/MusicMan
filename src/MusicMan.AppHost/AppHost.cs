var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MusicMan_Web>("musicman-web");

builder.Build().Run();
