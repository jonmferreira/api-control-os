# Rodando a API no Docker

## 🚀 Início Rápido

### Opção 1: API + SQL Server (Produção)

```bash
cd C:\Projetos\api-control-os

# Subir tudo (API + SQL Server)
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Parar tudo
docker-compose down
```

Acessar:
- **API:** http://localhost:8080
- **Swagger:** http://localhost:8080/swagger
- **SQL Server:** localhost:1433 (user: sa, password: Your_strong_password123)

---

### Opção 2: Apenas API (com In-Memory DB)

```bash
cd C:\Projetos\api-control-os

# Build da imagem
docker build -t serviceorders-api .

# Rodar container
docker run -d \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e Database__Provider=InMemory \
  --name serviceorders-api \
  serviceorders-api

# Ver logs
docker logs -f serviceorders-api

# Parar
docker stop serviceorders-api
docker rm serviceorders-api
```

Acessar:
- **API:** http://localhost:8080
- **Swagger:** http://localhost:8080/swagger

---

## 📋 Comandos Úteis

### Ver containers rodando
```bash
docker ps
```

### Ver logs em tempo real
```bash
# API
docker-compose logs -f api

# SQL Server
docker-compose logs -f db
```

### Executar comandos dentro do container
```bash
# Entrar no container da API
docker exec -it serviceorders-api bash

# Executar migrations manualmente
docker exec -it serviceorders-api dotnet ef database update
```

### Rebuild após mudanças no código
```bash
# Rebuild e restart
docker-compose up -d --build

# Ou rebuild apenas a API
docker-compose build api
docker-compose up -d api
```

### Limpar tudo
```bash
# Parar e remover containers
docker-compose down

# Remover também volumes (apaga banco de dados)
docker-compose down -v

# Remover imagens
docker rmi serviceorders-api
```

---

## 🔧 Configuração

### Variáveis de Ambiente

O `docker-compose.yml` já está configurado com:

**API:**
- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://+:8080`
- `Database__Provider=SqlServer`
- `ConnectionStrings__DefaultConnection=...`

**SQL Server:**
- `ACCEPT_EULA=Y`
- `SA_PASSWORD=Your_strong_password123`

### Alterar senha do SQL Server

Editar `docker-compose.yml`:

```yaml
db:
  environment:
    SA_PASSWORD: "SuaSenhaForte123!"

api:
  environment:
    ConnectionStrings__DefaultConnection: "Server=db,1433;Database=ServiceOrdersDb;User Id=sa;Password=SuaSenhaForte123!;TrustServerCertificate=True;"
```

### Usar In-Memory no Docker

Editar `docker-compose.yml`:

```yaml
api:
  environment:
    Database__Provider: "InMemory"  # Mudar aqui
    # Não precisa do ConnectionString
```

---

## 🌐 Integração com Front-end

### Front-end rodando localmente

Atualizar `.env` do front-end:

```env
VITE_OS_API_BASE_URL=http://localhost:8080/api
```

### Front-end + API no Docker

Criar `docker-compose.yml` na raiz do projeto `C:\Projetos\`:

```yaml
version: "3.9"

services:
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: serviceorders-db
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: "Your_strong_password123"
    ports:
      - "1433:1433"
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Your_strong_password123 -Q 'SELECT 1' || exit 1"]
      interval: 10s
      timeout: 5s
      retries: 10
      start_period: 20s

  api:
    build:
      context: ./api-control-os
      dockerfile: Dockerfile
    container_name: serviceorders-api
    environment:
      ASPNETCORE_ENVIRONMENT: "Production"
      ASPNETCORE_URLS: "http://+:8080"
      Database__Provider: "SqlServer"
      ConnectionStrings__DefaultConnection: "Server=db,1433;Database=ServiceOrdersDb;User Id=sa;Password=Your_strong_password123;TrustServerCertificate=True;"
    ports:
      - "8080:8080"
    depends_on:
      db:
        condition: service_healthy

  frontend:
    build:
      context: ./front-control-os
      dockerfile: Dockerfile
    container_name: serviceorders-frontend
    environment:
      VITE_OS_API_BASE_URL: "http://localhost:8080/api"
    ports:
      - "5173:80"
    depends_on:
      - api
```

Criar `Dockerfile` no front-end:

```dockerfile
# front-control-os/Dockerfile
FROM node:18-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

Rodar tudo:
```bash
cd C:\Projetos
docker-compose up -d
```

---

## 🐛 Troubleshooting

### API não inicia

**Ver logs:**
```bash
docker-compose logs api
```

**Problemas comuns:**
- SQL Server ainda não está pronto → Esperar alguns segundos
- Porta 8080 já em uso → Mudar porta no docker-compose.yml
- Erro de migrations → Entrar no container e rodar manual

### SQL Server não conecta

**Testar conexão:**
```bash
docker exec -it serviceorders-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P Your_strong_password123 \
  -Q "SELECT @@VERSION"
```

**Healthcheck falhou:**
- Aguardar mais tempo (pode levar 30-60s na primeira vez)
- Verificar senha no docker-compose.yml

### Porta já em uso

**Windows:**
```powershell
# Ver o que está usando a porta 8080
netstat -ano | findstr :8080

# Matar processo (substituir PID)
taskkill /PID <PID> /F
```

**Linux/Mac:**
```bash
# Ver o que está usando a porta
lsof -i :8080

# Matar processo
kill -9 <PID>
```

### Rebuildar após mudanças

```bash
# Rebuild completo
docker-compose down
docker-compose build --no-cache
docker-compose up -d

# Ou apenas rebuild da API
docker-compose build --no-cache api
docker-compose up -d api
```

---

## 📊 Monitoramento

### Ver uso de recursos
```bash
docker stats
```

### Ver logs de migrations
```bash
docker-compose logs api | grep -i migration
```

### Backup do banco
```bash
docker exec -it serviceorders-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P Your_strong_password123 \
  -Q "BACKUP DATABASE ServiceOrdersDb TO DISK = '/var/opt/mssql/backup/serviceorders.bak'"

# Copiar backup para host
docker cp serviceorders-db:/var/opt/mssql/backup/serviceorders.bak ./backup.bak
```

---

## ✅ Checklist

- [ ] Docker Desktop instalado e rodando
- [ ] `docker-compose.yml` configurado
- [ ] Porta 8080 disponível
- [ ] Porta 1433 disponível (se usar SQL Server)
- [ ] `docker-compose up -d` executado
- [ ] Aguardar 30-60s para SQL Server iniciar
- [ ] Acessar http://localhost:8080/swagger
- [ ] Front-end configurado para http://localhost:8080/api

---

## 🔗 Links Úteis

- **Docker Desktop:** https://www.docker.com/products/docker-desktop
- **Docker Compose Docs:** https://docs.docker.com/compose/
- **SQL Server Container:** https://hub.docker.com/_/microsoft-mssql-server

---

Criado por: jonmferreira
Data: 2025-12-26
