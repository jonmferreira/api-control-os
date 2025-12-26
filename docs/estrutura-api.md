# Estrutura sugerida para a API

A solução segue uma divisão em camadas baseada em Clean Architecture. Cada projeto tem uma responsabilidade clara e compõe a API de ordens de serviço.

## Correspondência entre camadas

- **Api** → `src/ServiceOrders.Api`
  - Controladores, endpoints REST e configuração do ASP.NET Core.
- **Domain** → `src/ServiceOrders.Domain`
  - Entidades de domínio (`OrderOfService`, `ChecklistTemplate`, `ChecklistResponse`, `User`, `MonthlyTarget`), contratos e validações.
- **Infrastructure** → `src/ServiceOrders.Infrastructure`
  - Persistência (EF Core), repositórios concretos, serviços de infraestrutura (SES, SNS, ViaCEP, CNPJa).
- **Application** → `src/ServiceOrders.Application`
  - Casos de uso, DTOs, orquestração de repositórios/serviços e validações de entrada.

## Organização recomendada

1. **Entities e DTOs**
   - Entidades do domínio permanecem em `ServiceOrders.Domain/Entities`.
   - DTOs e comandos de aplicação ficam em `ServiceOrders.Application/Dtos` e `ServiceOrders.Application/Commands`.
2. **Repositórios**
   - Interfaces em `ServiceOrders.Domain/Repositories`.
   - Implementações concretas em `ServiceOrders.Infrastructure/Repositories` utilizando o `ServiceOrdersDbContext`.
3. **Serviços**
   - Serviços de domínio compartilhados no projeto `ServiceOrders.Domain`.
   - Serviços de aplicação (autenticação, reset de senha, ordens de serviço) em `ServiceOrders.Application/Services`.
4. **Infraestrutura e utilitários**
   - Configurações de EF Core em `ServiceOrders.Infrastructure/Persistence/Configurations`.
   - Integrações externas (SES, SNS, ViaCEP, CNPJa) em pastas específicas dentro de `ServiceOrders.Infrastructure`.

Essa organização facilita testes isolados por camada, reutilização de abstrações e evolução incremental do domínio de ordens de serviço.
