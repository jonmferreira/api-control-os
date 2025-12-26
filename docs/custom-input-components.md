# Formato de `JsonBody` para componentes customizados

Cada componente customizado define, no campo `JsonBody`, um esquema leve para os campos que precisam ser preenchidos na resposta do checklist. O formato esperado é um objeto JSON com a propriedade `fields`, que deve conter um array com pelo menos um item.

## Estrutura mínima

```json
{
  "fields": [
    { "name": "placa", "label": "Placa", "type": "text", "required": true },
    { "name": "modelo", "label": "Modelo", "type": "text", "required": true }
  ]
}
```

### Regras
- `fields` é obrigatório e deve ser um array.
- Cada item de `fields` deve possuir:
  - `name`: identificador único do campo.
  - `label`: título a ser exibido no front-end.
  - `type`: tipo do campo (ex.: `text`, `number`, `date`).
  - `required`: `true` para campos obrigatórios.
- Pelo menos um campo obrigatório deve ser declarado para que o componente seja aceito.

## Validação do `CustomInputPayload`

Ao registrar uma resposta de checklist:
- Itens que usam componentes com `HasCustomInput=true` precisam enviar `CustomInputPayload` como **objeto JSON** com todos os campos marcados como `required`.
- Campos obrigatórios não podem ser nulos ou strings vazias.
- Itens sem `HasCustomInput` não aceitam `CustomInputPayload`.

### Exemplo completo (componente `DadosCarro`)

- **JsonBody do componente**
  ```json
  {
    "fields": [
      { "name": "placa", "label": "Placa", "type": "text", "required": true },
      { "name": "chassi", "label": "Chassi", "type": "text", "required": true },
      { "name": "modelo", "label": "Modelo", "type": "text", "required": true },
      { "name": "ano", "label": "Ano", "type": "number", "required": true }
    ]
  }
  ```

- **CustomInputPayload aceito**
  ```json
  {
    "placa": "ABC1234",
    "chassi": "9BWZZZ377VT004251",
    "modelo": "Fusca",
    "ano": 1974
  }
  ```
