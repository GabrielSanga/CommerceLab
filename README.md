# CommerceLab

Monólito modular em .NET 10 para estudo de arquitetura: cada módulo é dono do seu schema no PostgreSQL e se pluga no Host pelo contrato `IModule`, sem que o Host conheça o conteúdo do módulo.

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker Desktop
- Ferramenta `dotnet-ef` global:

  ```powershell
  dotnet tool install --global dotnet-ef
  ```

## Subir o ambiente

Na ordem — as migrations **não** rodam no startup.

```powershell
# 1. banco
docker compose up -d

# 2. criar/atualizar o schema do módulo Catálogo
dotnet ef database update -p src/Modules/Catalogo/CommerceLab.Modules.Catalogo -s src/CommerceLab.Host --context CatalogoDBContext

# 3. aplicação
dotnet run --project src/CommerceLab.Host
```

## Estrutura

```
src/
  CommerceLab.Host/            composição: descobre os módulos e mapeia os endpoints
  CommerceLab.Shared/          contrato IModule e tipo Result, compartilhados
  Modules/Catalogo/
    ...Catalogo/               Domain, Application, Infrastructure, Presentation
    ...Catalogo.Contracts/     interface com outros modulos
docs/
  Diagramas/                   diagrama de classes
```
