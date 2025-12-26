# Script para rodar API no Docker
# Uso: .\docker-start.ps1 [opcao]
# Opções: prod, dev, stop, rebuild, logs

param(
    [string]$Opcao = "prod"
)

$ErrorActionPreference = "Stop"

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  Service Orders API - Docker  " -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

switch ($Opcao) {
    "prod" {
        Write-Host "Iniciando API + SQL Server..." -ForegroundColor Green
        docker-compose up -d
        Write-Host ""
        Write-Host "Aguardando inicialização (30s)..." -ForegroundColor Yellow
        Start-Sleep -Seconds 30
        Write-Host ""
        Write-Host "✓ API rodando em: http://localhost:8080" -ForegroundColor Green
        Write-Host "✓ Swagger em: http://localhost:8080/swagger" -ForegroundColor Green
        Write-Host "✓ SQL Server em: localhost:1433 (sa/Your_strong_password123)" -ForegroundColor Green
        Write-Host ""
        Write-Host "Ver logs: docker-compose logs -f api" -ForegroundColor Cyan
    }

    "dev" {
        Write-Host "Iniciando API (In-Memory DB)..." -ForegroundColor Green
        docker build -t serviceorders-api .
        docker run -d `
            -p 8080:8080 `
            -e ASPNETCORE_ENVIRONMENT=Development `
            -e Database__Provider=InMemory `
            --name serviceorders-api `
            serviceorders-api
        Write-Host ""
        Write-Host "Aguardando inicialização (10s)..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        Write-Host ""
        Write-Host "✓ API rodando em: http://localhost:8080" -ForegroundColor Green
        Write-Host "✓ Swagger em: http://localhost:8080/swagger" -ForegroundColor Green
        Write-Host ""
        Write-Host "Ver logs: docker logs -f serviceorders-api" -ForegroundColor Cyan
    }

    "stop" {
        Write-Host "Parando containers..." -ForegroundColor Yellow
        docker-compose down
        docker stop serviceorders-api 2>$null
        docker rm serviceorders-api 2>$null
        Write-Host "✓ Containers parados" -ForegroundColor Green
    }

    "rebuild" {
        Write-Host "Rebuild da API..." -ForegroundColor Yellow
        docker-compose down
        docker-compose build --no-cache api
        docker-compose up -d
        Write-Host ""
        Write-Host "Aguardando inicialização (30s)..." -ForegroundColor Yellow
        Start-Sleep -Seconds 30
        Write-Host ""
        Write-Host "✓ Rebuild completo!" -ForegroundColor Green
        Write-Host "✓ API rodando em: http://localhost:8080" -ForegroundColor Green
    }

    "logs" {
        Write-Host "Mostrando logs da API..." -ForegroundColor Cyan
        docker-compose logs -f api
    }

    default {
        Write-Host "Uso: .\docker-start.ps1 [opcao]" -ForegroundColor Red
        Write-Host ""
        Write-Host "Opções disponíveis:" -ForegroundColor Yellow
        Write-Host "  prod     - Inicia API + SQL Server (padrão)" -ForegroundColor White
        Write-Host "  dev      - Inicia apenas API (In-Memory)" -ForegroundColor White
        Write-Host "  stop     - Para todos os containers" -ForegroundColor White
        Write-Host "  rebuild  - Rebuild e restart" -ForegroundColor White
        Write-Host "  logs     - Mostra logs da API" -ForegroundColor White
        Write-Host ""
        Write-Host "Exemplos:" -ForegroundColor Yellow
        Write-Host "  .\docker-start.ps1" -ForegroundColor Cyan
        Write-Host "  .\docker-start.ps1 dev" -ForegroundColor Cyan
        Write-Host "  .\docker-start.ps1 stop" -ForegroundColor Cyan
    }
}
