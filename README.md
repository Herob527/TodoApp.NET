# Todo App

This is a simple todo app with auth written in .NET using [aspire](https://github.com/aspire-apps/aspire).

Previously I was using docker compose as goto, but I've decided that why not check out Aspire for setting up deps instead.

My goal is to make .NET fullstack app mostly by myself instead of relying on existing patterns from previous job.

I've decided to use auth to test, what is possible in .NET speaking of auth.

Gotta say, .NET Identity, while it provides basis for auth, it requires more setup to avoid excessive database SELECT queries.

I kinda get, why we used Firebase instead of .NET Identity for auth and setup own user tables.

Currently, backend seems to work when testing via postman.

After making sure it works via tests, I think I'll set up simple website via blazor.

ChatGPT was quite helpful with this project to figure out some patterns not easily found online.
