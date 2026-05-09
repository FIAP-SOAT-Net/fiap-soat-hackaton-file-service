# FIAP Soat Hackaton File Service

API para gerenciamento de arquivos com upload, consulta, listagem e remoção. O projeto persiste metadados no MongoDB, envia o conteúdo para um storage S3-compatible e publica eventos em RabbitMQ para sinalizar os principais acontecimentos do ciclo de vida do arquivo.

## Visão geral

O sistema está organizado em camadas, com uma API HTTP fina na borda e regras de negócio concentradas na aplicação:

- `Api`: expõe os endpoints HTTP, faz o binding da requisição e registra as dependências no container.
- `Application`: contém controllers, use cases e contratos de gateways.
- `Infrastructure`: implementa os gateways usando MongoDB, RabbitMQ e S3.
- `Domain`: define os modelos principais e as respostas compartilhadas.

O fluxo principal é:

1. A requisição chega em `FileEndpoints`.
2. O endpoint converte a entrada HTTP para um request da aplicação.
3. `FileController` escolhe o use case correto.
4. O use case executa validações e orquestra os gateways.
5. A infraestrutura grava no MongoDB, envia para S3 e publica eventos no RabbitMQ.

## Pipeline de execução

### Startup da API

O arquivo `Program.cs` monta a aplicação assim:

1. Lê a configuração de S3, RabbitMQ e MongoDB.
2. Registra `IAmazonS3` com `AmazonS3Client`.
3. Registra os controllers, handlers, repositório, storage e publisher.
4. Cria o `MongoClient` e o `IMongoDatabase`.
5. Adiciona health checks e OpenAPI.
6. Faz o mapeamento dos endpoints em `FileEndpoints`.

### Fluxo de upload

`POST /files/upload`

1. O endpoint recebe um arquivo via `multipart/form-data` no campo `file`.
2. O conteúdo é carregado em memória.
3. `FileController.UploadAsync` transforma a entrada em `UploadFileCommand`.
4. `UploadFileHandler` valida se o arquivo está vazio e se o `Content-Type` é `application/pdf` ou começa com `image/`.
5. `AwsS3Storage` envia o conteúdo para o bucket configurado no S3.
6. `FileRepository` persiste o documento no MongoDB.
7. `RabbitMqPublisher` publica o evento `file.uploaded.<fileId>`.
8. A API responde com `201 Created`.

### Fluxo de consulta

`GET /files/{fileId}`

1. O endpoint encaminha o ID para `GetFileHandler`.
2. O repositório tenta converter o ID para `ObjectId`.
3. Se o arquivo existir, a API responde `200 OK` com o documento.
4. Se não existir, retorna `404 Not Found`.

`GET /files`

1. A API busca todos os documentos no MongoDB.
2. O controller devolve somente metadados públicos: `id`, `fileName`, `contentType`, `size` e `uploadedAt`.

### Fluxo de remoção

`DELETE /files/{fileId}`

1. O endpoint envia o comando para `DeleteFileHandler`.
2. O repositório remove o documento no MongoDB.
3. Se a exclusão ocorrer, o handler publica `file.deleted.<fileId>` no RabbitMQ.
4. A API responde `204 No Content`.
5. Se o arquivo não existir, a resposta é `404 Not Found`.

## Dependências

### Runtime e build

- .NET SDK 10.0
- ASP.NET Core
- Docker e Docker Compose para o ambiente local completo

### NuGet principais da API

- `AWSSDK.S3` para integração com S3-compatible storage
- `MongoDB.Driver` para persistência no MongoDB
- `Microsoft.AspNetCore.OpenApi` para OpenAPI em desenvolvimento

### Serviços externos

- MongoDB
- RabbitMQ
- S3-compatible storage, atualmente configurado com LocalStack no `docker-compose.yml`

## Estrutura do projeto

```text
src/
	Fiap.Soat.Hackaton.FileService.Api/
	Fiap.Soat.Hackaton.FileService.Infrastructure/
	Fiap.Spat.Hackaton.FileService.Application/
	Fiap.Spat.Hackaton.FileService.Domain/
tests/
	Fiap.Soat.Hackaton.FileService.Api.Tests/
	Fiap.Soat.Hackaton.FileService.Infrastructure.Tests/
	Fiap.Soat.Hackaton.FileService.Integration.Tests/
	Fiap.Spat.Hackaton.FileService.Application.Tests/
	Fiap.Spat.Hackaton.FileService.Domain.Tests/
```

## Como rodar localmente

Existem duas formas recomendadas de executar o projeto.

### Opção 1: subir tudo com Docker Compose

Pré-requisitos:

- Docker Desktop instalado e em execução
- Docker Compose habilitado

O `docker-compose.yml` sobe:

- MongoDB
- RabbitMQ com interface de gerenciamento
- LocalStack para simular S3
- A API .NET

Passos:

1. Crie um arquivo `.env` na raiz do projeto com as variáveis exigidas pelo compose.
2. Suba os serviços com `docker-compose up -d --build`.
3. Acesse a API em `http://localhost:5180`.

### Exemplo de `.env`

```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

MONGO_INITDB_ROOT_USERNAME=root
MONGO_INITDB_ROOT_PASSWORD=password123
MONGO_INITDB_DATABASE=fileservice

MONGODB_HOST=mongodb
MONGODB_PORT=27017
MONGODB_USER=root
MONGODB_PASSWORD=password123
MONGODB_DATABASE=fileservice

RABBITMQ_DEFAULT_USER=guest
RABBITMQ_DEFAULT_PASS=guest
RABBITMQ_PORT=5672
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_EXCHANGE_NAME=fileservice.events.exchange
RABBITMQ_QUEUE_NAME=fileservice.events

S3_REGION=us-east-1
S3_BUCKET_NAME=fia-soat-hackaton-files
S3_ACCESS_KEY=test
S3_SECRET_KEY=test
S3_FORCE_PATH_STYLE=true
LOCALSTACK_AUTH_TOKEN=coloque_seu_token_aqui
```

Observações importantes:

- O compose atual usa `LocalStack Pro`, então o `LOCALSTACK_AUTH_TOKEN` é obrigatório.
- O projeto espera uma conexão MongoDB válida em `ConnectionStrings:MongoDB`.
- O RabbitMQ e o S3 também precisam estar configurados mesmo quando você roda fora do Docker.

### Opção 2: rodar pela IDE ou com `dotnet run`

Pré-requisitos:

- .NET SDK 10.0 instalado
- MongoDB acessível
- RabbitMQ acessível
- S3-compatible endpoint acessível, como LocalStack ou outro serviço equivalente

Exemplo de execução:

```bash
dotnet restore
dotnet build
dotnet run --project src/Fiap.Soat.Hackaton.FileService.Api/Fiap.Soat.Hackaton.FileService.Api.csproj
```

Se você não usar Docker Compose, garanta estas configurações mínimas:

- `ConnectionStrings__MongoDB`
- `MongoDb__Database`
- `RabbitMq__Host`
- `RabbitMq__Port`
- `RabbitMq__UserName`
- `RabbitMq__Password`
- `RabbitMq__ExchangeName`
- `RabbitMq__QueueName`
- `S3__ServiceUrl`
- `S3__Region`
- `S3__BucketName`
- `S3__AccessKey`
- `S3__SecretKey`
- `S3__ForcePathStyle`

## Endpoints

### Upload de arquivo

`POST /files/upload`

Aceita `multipart/form-data` com o campo `file`.

Regras:

- apenas PDF ou imagens são aceitos
- arquivo vazio é rejeitado

Resposta:

- `201 Created` quando o upload é concluído
- `400 Bad Request` quando a validação falha

### Buscar arquivo por ID

`GET /files/{fileId}`

Resposta:

- `200 OK` com o documento
- `404 Not Found` se o arquivo não existir

### Listar arquivos

`GET /files`

Resposta:

- `200 OK` com a lista de arquivos

O retorno traz apenas metadados públicos.

### Remover arquivo

`DELETE /files/{fileId}`

Resposta:

- `204 No Content` quando a remoção ocorre
- `404 Not Found` quando o ID não existe

### Health check

`GET /healthcheck`

O endpoint já existe no host, mas o projeto ainda não registra health probes específicos para MongoDB, RabbitMQ ou S3.

## Principais fluxos do projeto

### Upload

Esse é o fluxo mais importante do sistema e o único que interage com todos os blocos externos ao mesmo tempo:

- a API recebe o arquivo via HTTP
- o use case valida o tipo e o tamanho
- o conteúdo é enviado ao S3
- o documento é persistido no MongoDB
- um evento é publicado no RabbitMQ

O evento publicado no upload usa o padrão `file.uploaded.<id>`.

### Leitura

Os fluxos de leitura usam apenas o MongoDB:

- `GET /files/{id}` busca o documento completo
- `GET /files` lista todos os arquivos com metadados simplificados

### Exclusão

A remoção também envolve um evento assíncrono:

- o documento é removido do MongoDB
- se a exclusão for bem-sucedida, o evento `file.deleted.<id>` é publicado

## Testes

O projeto de integração já cobre o endpoint de upload com `WebApplicationFactory` e fakes para as dependências externas.

Executar:

```bash
dotnet test tests/Fiap.Soat.Hackaton.FileService.Integration.Tests/Fiap.Soat.Hackaton.FileService.Integration.Tests.csproj
```

Esses testes não dependem de MongoDB, RabbitMQ ou S3 reais porque os serviços são substituídos no host de teste.

## Docker

### Subir tudo

```bash
docker-compose up -d --build
```

### Parar tudo

```bash
docker-compose down
```

### Remover volumes

```bash
docker-compose down -v
```

## Pontos de atenção

- O upload não aceita envio por header ou corpo bruto; o contrato real é `multipart/form-data` com o campo `file`.
- O `GET` e o `DELETE` dependem de um `fileId` válido no formato de `ObjectId` do MongoDB.
- O `docker-compose.yml` exige variáveis de ambiente, então o arquivo `.env` é praticamente obrigatório para subir o ambiente completo.
- O `LocalStack Pro` exige token de autenticação.
- A API está configurada com `UseHttpsRedirection`, então em ambiente apenas HTTP pode aparecer um warning sobre porta HTTPS, o que é esperado durante desenvolvimento.

## Referências úteis

- [Program.cs](src/Fiap.Soat.Hackaton.FileService.Api/Program.cs)
- [FileEndpoints.cs](src/Fiap.Soat.Hackaton.FileService.Api/Endpoints/FileEndpoints.cs)
- [Dockerfile da API](Dockerfile)
- [docker-compose.yml](docker-compose.yml)
- [Guia de integração](INTEGRATION-GUIDE.md)
- [Guia do Docker Compose](DOCKER-COMPOSE-GUIDE.md)
