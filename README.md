# Service Orders Clean Architecture Sample

Esta solução demonstra uma API ASP.NET Core estruturada com Clean Architecture para gerenciar ordens de serviço (OS). Ela fornece autenticação JWT, recuperação de senha, consultas de CEP/CNPJ e endpoints iniciais para abrir, concluir e consultar ordens.

## Estrutura de projetos

```
ServiceOrders.sln
├── src
│   ├── ServiceOrders.Domain           # Entidades de domínio, contratos e serviços transversais
│   ├── ServiceOrders.Application      # Casos de uso, DTOs e orquestração de serviços
│   ├── ServiceOrders.Infrastructure   # Persistência com EF Core e integrações externas (SES, SNS, CNPJa, ViaCEP)
│   └── ServiceOrders.Api              # API REST com endpoints de autenticação e ordens de serviço
└── tests
    └── ServiceOrders.UnitTests        # Testes unitários do domínio
```

## Endpoints principais

- `POST /api/auth/login` – autenticação via e-mail e senha.
- `POST /api/auth/forgot-password` / `POST /api/auth/reset-password` – fluxo de recuperação de senha.
- `POST /api/orders` – cria uma nova ordem de serviço.
- `PUT /api/orders/{id}` – atualiza título, descrição ou técnico responsável.
- `POST /api/orders/{id}/status` – altera o status para **Aberta**, **EmAndamento**, **Concluída** ou **Rejeitada** com observações.
- `GET /api/orders` – lista ordens de serviço com filtro opcional por status e técnico.
- `GET /api/orders/{id}` – consulta uma OS com checklist e anexos.
- `POST /api/checklist-templates` – publica templates versionados de checklist (com componentes customizados).
- `POST /api/checklist-responses` – registra o preenchimento de checklist para uma OS, incluindo fotos.
- `POST /api/custom-components` – registra componentes customizados (`JsonBody`) para itens de checklist com input especial (veja [docs/custom-input-components.md](docs/custom-input-components.md) para o formato esperado).
- `POST /api/photos` – cadastra metadados de fotos para OS ou itens de checklist.
- `GET /api/cep/{cep}` – consulta endereço via ViaCEP.
- `GET /api/cnpj/{cnpj}` – consulta dados cadastrais via CNPJa Open API.

## Executando a API

```bash
dotnet build
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/ServiceOrders.Api
```

A documentação interativa (Swagger) estará disponível em `https://localhost:5001/swagger` no perfil de desenvolvimento.

## Configuração

- **Jwt**: defina `Issuer`, `Audience` e `SecretKey` em `appsettings.json`.
- **Database**: por padrão utiliza o provedor InMemory (`Database:Name = ServiceOrdersDb`). Para SQL Server, configure `Database:Provider` como `SqlServer` e informe a connection string (`ConnectionStrings:DefaultConnection`).
- **E-mail/SMS**: as seções `Email:AwsSes` e `Aws:Sms` permitem configurar SES e SNS (SMS) da AWS.
- **CNPJa**: configure `Cnpja:Token` se possuir um token válido para as consultas de CNPJ.

## Executando com Docker

O `Dockerfile` publica a API em uma imagem baseada no .NET 8. Use o `docker-compose.yml` para subir a API junto de um SQL Server 2022:

```bash
docker compose up --build
```

A API ficará disponível em `http://localhost:8080` quando o banco estiver saudável.

## Testes

```bash
dotnet test
```

Os testes cobrem regras básicas do domínio (por exemplo, validações e conclusão de ordens de serviço).
