# TODO

- [x] Higienização do legado: remover referências do domínio de estacionamento, ajustar namespaces e documentação para Ordens de Serviço.
- [x] Modelagem de domínio: criar entidades de `OrderOfService`, checklists, componentes customizados e enums.
- [x] Persistência (EF Core Code First): configurar DbContext, mapeamentos e migrations iniciais para o novo domínio.
- [x] Casos de uso na camada Application: comandos/consultas para OS e checklists com validações.
- [x] API: endpoints autenticados para CRUD de OS e checklists, upload de fotos e versionamento de templates.
- [ ] Validações e segurança: aplicar FluentValidation nos DTOs críticos, reforçar políticas de autorização (técnico vs. admin), registrar auditoria básica e validar uploads/fotos para reprovações.
- [ ] Observabilidade e documentação: atualizar Swagger com exemplos e guias de payloads.
- [x] CI/CD e containerização: ajustar pipelines e imagens Docker para o novo domínio.
