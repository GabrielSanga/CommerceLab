using System.Reflection;

namespace CommerceLab.Shared.Modules;

/// <summary>
/// Localiza as implementações de <see cref="IModule"/> nos assemblies de módulo
/// presentes na pasta de saída da aplicação.
/// </summary>
internal static class ModuleDiscovery
{
    private const string ModuleAssemblyPattern = "CommerceLab.Modules.*.dll";

    public static IReadOnlyList<IModule> Discover()
    {
        return LoadModuleAssemblies()
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IModule).IsAssignableFrom(type))
            .Select(CreateInstance)
            .OrderBy(module => module.Name, StringComparer.Ordinal)
            .ToList();
    }

    private static IEnumerable<Assembly> LoadModuleAssemblies()
    {
        foreach (var file in Directory.EnumerateFiles(AppContext.BaseDirectory, ModuleAssemblyPattern))
        {
            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFrom(file);
            }
            catch (BadImageFormatException)
            {
                continue;
            }

            yield return assembly;
        }
    }

    private static IModule CreateInstance(Type type)
    {
        if (Activator.CreateInstance(type) is not IModule module)
        {
            throw new InvalidOperationException($"O módulo '{type.FullName}' não pôde ser instanciado. Implementações de {nameof(IModule)} precisam de um construtor público sem parâmetros.");
        }

        return module;
    }
}
