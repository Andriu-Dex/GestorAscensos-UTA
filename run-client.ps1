cd "C:\Users\andri\Documents\D-Proyectos\Git\ProyectoAgiles\SistemaGestionAscensos\SGA.BlazorClient"
dotnet build "SGA.BlazorApp.Client.csproj" -o bin\Debug\net9.0\publish
cd bin\Debug\net9.0\publish
dotnet SGA.BlazorApp.Client.dll
