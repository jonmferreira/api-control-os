# Guia de autenticação

Este documento resume como a autenticação baseada em JWT está configurada na Service Orders API e como interagir com os endpoints protegidos.

A API registra autenticação JWT em `Program.cs`, configurando o handler `JwtBearer` com emissor, audiência, chave de assinatura e `ClockSkew` zerado para que os tokens expirem exatamente no tempo configurado.【F:src/ServiceOrders.Api/Program.cs†L1-L66】 Os valores são lidos da seção `Jwt` em `appsettings.json`, onde você pode ajustar emissor, audiência, chave secreta e tempo de expiração (em minutos).【F:src/ServiceOrders.Api/appsettings.json†L17-L24】

As contas de usuário são persistidas na tabela `Users`. O repositório `UserRepository` encapsula o acesso a esses dados e o serviço de autenticação (`AuthService`) valida as credenciais usando o `Pbkdf2PasswordHasher` e emite o token via `JwtTokenGenerator`.【F:src/ServiceOrders.Application/Services/AuthService.cs†L1-L52】【F:src/ServiceOrders.Infrastructure/Authentication/JwtTokenGenerator.cs†L1-L57】 

Para autenticar, envie um `POST /api/Auth/login` com o e-mail e a senha do usuário.【F:src/ServiceOrders.Api/Controllers/AuthController.cs†L14-L57】 Ambos os campos são obrigatórios e validados server-side.【F:src/ServiceOrders.Api/Models/Requests/LoginRequest.cs†L1-L13】 Um login bem-sucedido retorna o token de acesso, o timestamp de expiração e as informações do usuário autenticado.【F:src/ServiceOrders.Api/Models/Responses/LoginResponse.cs†L1-L11】 

Além do login, os fluxos de recuperação de senha são expostos por `POST /api/Auth/forgot-password` e `POST /api/Auth/reset-password`, que delegam para o `PasswordResetService` e usam o repositório de tokens de redefinição para controlar expirações.【F:src/ServiceOrders.Application/Services/PasswordResetService.cs†L1-L69】 
