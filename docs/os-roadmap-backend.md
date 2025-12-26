# Roadmap backend – Ordem de Serviço (Code First)

Documento para guiar a migração do repositório para o contexto de Ordens de Serviço (OS) com CRUD completo, checklists versionados e componentes de formulário customizáveis. O objetivo é remover o domínio anterior de estacionamento e alinhar a stack existente (ASP.NET Core + EF Core, Docker, pipelines) ao novo cenário.

## Objetivos principais
- Implementar autenticação e autorização para acesso às OS.
- Criar CRUD de OS com descrição das atividades e anexos.
- Controlar checklists com definição persistida em banco (título, data de atualização, responsável técnico).
- Permitir itens de checklist com componentes customizados (descrição, flag `HasCustomInput`, `CustomInputComponentId`, valor aprovado/rejeitado/não se aplica).
- Salvar fotos vinculadas ao preenchimento da checklist.
- Padronizar Code First, migrations e infraestrutura de deploy (Docker e CI/CD) já existente.

## Stack e padrões
- ASP.NET Core 8 + EF Core (Code First) seguindo a mesma divisão em `Domain`, `Application`, `Infrastructure`, `Api`.
- Authentication/Authorization com JWT + roles/claims para técnicos e administradores.
- CQRS leve em `Application` para comandos/consultas de OS e checklists.
- Validações: FluentValidation em DTOs + regras de domínio em entidades/aggregates.
- Arquivos/fotos: seguir estratégia atual de storage (local/S3) já configurada no projeto.

## Modelagem inicial
- **OrderOfService (OS)**: Id, Título/Descrição, Status (Aberta, Em Andamento, Concluída, Rejeitada), Técnico responsável, Datas (abertura, conclusão), Anotações, Referência ao checklist preenchido.
- **ChecklistTemplate**: Id, Título, `UpdatedAt`, Responsável Técnico que publicou, Coleção de `ChecklistTemplateItem`.
- **ChecklistTemplateItem**: Id, Descrição, `HasCustomInput` (bool), `CustomInputComponentId` (nullable), `DefaultOutcome` (Aprovado/Rejeitado/Não se aplica) opcional, Ordem de exibição.
- **CustomInputComponent**: Id, Nome do componente, `JsonBody` (definição do form ex.: Campos chassi/placa/modelo/ano).
- **ChecklistResponse**: Id, `OrderOfServiceId`, `ChecklistTemplateId`, coleção de `ChecklistResponseItem`, anexos de fotos.
- **ChecklistResponseItem**: Id, `ChecklistTemplateItemId`, Valor (Aprovado/Rejeitado/Não se aplica), `CustomInputPayload` (json), Observação opcional, fotos vinculadas.
- **PhotoAttachment**: Id, URL/Path, Tipo (Evidência OS, Evidência Checklist Item), relação com OS/Item.

## Fases sugeridas
1. [x] **Higienização do legado**  
   - Remover referências do domínio de estacionamento (tickets, tarifas) e ajustar namespaces/descrições para OS.  
   - Atualizar README e documentação para refletir o novo domínio.
2. [x] **Modelagem de domínio**  
   - Criar entidades/aggregates conforme a seção anterior.  
   - Definir enums (`OrderStatus`, `ChecklistOutcome`) e value objects necessários.  
   - Interfaces de repositório para OS, templates, respostas e componentes customizados.
3. [x] **Persistência (EF Core Code First)**  
   - Configurar `DbContext` com novos DbSets e mapeamentos (owned types para payloads JSON se aplicável).  
   - Criar migrations iniciais para as novas tabelas; remover esquemas legados.  
   - Seeds mínimos: usuário admin, template de checklist de carro com componente `DadosCarro`.
4. [x] **Casos de uso na camada Application**  
   - Comandos: criar/atualizar OS, alterar status, registrar checklist response, anexar fotos.  
   - Consultas: listar OS por status/responsável, obter template vigente, histórico de respostas.  
   - Regras: impedir conclusão sem checklist, validar outcomes obrigatórios, validar payload quando `HasCustomInput=true`.
5. [x] **API (Controllers/Endpoints)**  
   - Endpoints autenticados para CRUD de OS e checklists (templates e respostas).  
   - Upload de fotos com retorno de URL para vinculação ao item.  
   - Versionar templates (publish/draft) para permitir evolução sem quebrar respostas antigas.
6. [x] **Validações e segurança**  
   - Cobrir DTOs pendentes com FluentValidation e validar `CustomInputPayload` conforme o `JsonBody`.  
   - Garantir políticas de autorização consistentes entre endpoints (técnico vs. admin).  
   - Implementar logging/auditoria das alterações de status e publicações de templates.  
   - Revisar upload de anexos para validar tipos/formato e exigir fotos em casos de reprovação.
7. [x] **Observabilidade e documentação**  
   - Swagger atualizado com exemplos de payloads para `DadosCarro` e outros componentes.  
   - Documentar formato esperado do `JsonBody` para novos componentes customizados.
8. [x] **CI/CD e containerização**  
   - Reaproveitar Dockerfile e docker-compose ajustando nomes/imagens.  
   - Atualizar pipelines do GitHub Actions para build/test dotnet e publicar a imagem da API com o novo nome (GHCR).  
   - Garantir que as migrations rodam na subida do container quando conectado a SQL Server.

## Regras e validações destacadas
- `HasCustomInput=true` exige `CustomInputComponentId` e preenchimento de `CustomInputPayload` seguindo o `JsonBody`.
- Resultado do item (`Aprovado`, `Rejeitado`, `NãoSeAplica`) é obrigatório; fotos obrigatórias quando rejeitado.
- Checklist deve referenciar o template vigente; bloquear reenvio se a OS já estiver concluída/cancelada.
- Uploads de fotos limitados por tamanho/formato; armazenar hash ou checksum para evitar duplicidade.
- Logs de auditoria para alterações de status de OS e publicações de templates.

## Entregáveis mínimos
- Migrations criadas e aplicáveis via `dotnet ef database update`.
- Endpoints CRUD funcionando com autenticação.  
- Swagger atualizado e exemplos de payload de checklist com componente `DadosCarro`.  
- Dockerfile e docker-compose compatíveis com o novo domínio.  
- Pipeline CI garantindo `dotnet build` e `dotnet test` do novo contexto.
