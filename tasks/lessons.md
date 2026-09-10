# Lessons

- Keep environment decisions for startup bootstrap in `Program.cs`. Put development
  bootstrap credentials in `appsettings.Development.json`, not C# fallback literals.
  Gate the entire `SetupAdminUserAsync` call with `app.Environment.IsDevelopment()`;
  do not merely pass the environment as a flag while invoking bootstrap everywhere.
