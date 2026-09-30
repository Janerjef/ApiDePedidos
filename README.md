# API de Pedidos

API REST para criar e gerenciar pedidos. A solução usa .NET 8, SQL Server para gravação e MongoDB para consultas. A interface web foi iniciada com React e Vite.

## Funcionalidades

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/orders` | Cria um pedido |
| `GET` | `/api/orders` | Lista pedidos resumidos |
| `GET` | `/api/orders/{id}` | Consulta os detalhes de um pedido |
| `PUT` | `/api/orders/{id}` | Atualiza as quantidades dos itens existentes |
| `DELETE` | `/api/orders/{id}` | Remove um pedido |

O sistema usa um cliente fixo e três produtos de exemplo:

| Produto | ID |
|---|---|
| Teclado | `00000000-0000-0000-0000-000000000001` |
| Mouse | `00000000-0000-0000-0000-000000000002` |
| Monitor | `00000000-0000-0000-0000-000000000003` |

## Tecnologias

- .NET 8 e ASP.NET Core Web API
- Entity Framework Core e SQL Server
- MongoDB
- MediatR e CQRS
- React e Vite
- Docker Compose

## Organização

- `ApiDePedidos.Api`: endpoints HTTP, configuração e Swagger.
- `ApiDePedidos.Application`: comandos, consultas, handlers, DTOs e interfaces.
- `ApiDePedidos.Domain`: entidades e regras de negócio.
- `ApiDePedidos.Infrastructure`: implementação dos repositórios e acesso ao SQL Server e MongoDB.
- `apidepedidos-web`: interface React.

O SQL Server guarda os pedidos. Depois da gravação, a aplicação atualiza uma projeção de leitura no MongoDB. Os endpoints GET consultam o MongoDB.

## Pré-requisitos

- .NET SDK 8
- Node.js e npm
- Docker Engine e Docker Compose
- Credenciais locais para o SQL Server e o MongoDB

## Configurar os bancos

Na raiz da solução, crie um arquivo `.env` usando `.env.example` como referência. Configure `MSSQL_SA_PASSWORD` com uma senha forte. Use a mesma senha na connection string da API.

Não versione nem compartilhe o `.env`. Não coloque senhas reais em `appsettings.json`, `appsettings.Development.json`, `.env.example` ou neste README.

Inicie os bancos:

```bash
docker compose up -d sqlserver mongodb
```

Confira o estado dos containers:

```bash
docker compose ps -a
```

Se o SQL Server não estiver como `Up`, consulte o log:

```bash
docker compose logs --tail=60 sqlserver
```

### Configurar as connection strings da API

A API executada localmente não carrega o `.env` automaticamente. Configure as conexões com User Secrets.

Para o SQL Server local:

```bash
dotnet user-secrets set "ConnectionStrings:Orders" 'Server=localhost,1433;Database=ApiDePedidos;User Id=sa;Password=SUA_SENHA_SQL;TrustServerCertificate=True' --project ApiDePedidos.Api
```

Para o MongoDB local:

```bash
dotnet user-secrets set "ConnectionStrings:OrdersMongo" 'mongodb://localhost:27017' --project ApiDePedidos.Api
```

Substitua `SUA_SENHA_SQL` pela senha configurada no `.env`. Não inclua os sinais `<` ou `>` ao substituir valores.

Para usar MongoDB Atlas, configure `ConnectionStrings:OrdersMongo` com a URI do Atlas. Substitua o marcador da senha pela senha do usuário do banco e codifique caracteres especiais da senha para URL.

### Aplicar migrations do SQL Server

A fábrica de design time do Entity Framework lê a variável de ambiente `ConnectionStrings__Orders`. No terminal atual, defina essa variável antes de executar a migration:

```bash
export ConnectionStrings__Orders='Server=localhost,1433;Database=ApiDePedidos;User Id=sa;Password=SUA_SENHA_SQL;TrustServerCertificate=True'
dotnet ef database update --project ApiDePedidos.Infrastructure --startup-project ApiDePedidos.Api
unset ConnectionStrings__Orders
```

## Executar a API

Na raiz da solução:

```bash
dotnet run --project ApiDePedidos.Api --launch-profile http
```

A API fica em `http://localhost:5191`.

- Swagger: <http://localhost:5191/swagger>
- Documento OpenAPI: <http://localhost:5191/swagger/v1/swagger.json>

Mantenha o terminal da API aberto enquanto testa o front.

## Executar o front-end

Em outro terminal:

```bash
cd apidepedidos-web
npm install
npm run dev
```

O Vite normalmente abre a interface em <http://localhost:5173>.

Se o navegador bloquear as chamadas do front para a API por CORS, configure uma política permitindo `http://localhost:5173` em `ApiDePedidos.Api/Program.cs`.

## Testar os endpoints no Swagger

### Criar pedido

Envie um produto existente e uma quantidade maior que zero para `POST /api/orders`:

```json
{
  "items": [
    {
      "productId": "00000000-0000-0000-0000-000000000001",
      "quantity": 1
    }
  ]
}
```

Uma criação bem-sucedida retorna `201 Created` e o ID do pedido.

### Listar pedidos

Execute `GET /api/orders`. A resposta contém os resumos dos pedidos, incluindo ID, status e total. Os itens não aparecem nessa listagem resumida.

### Consultar detalhes

Copie o ID retornado pelo POST e informe em `GET /api/orders/{id}`. Essa rota retorna o pedido e seus itens. Um pedido inexistente deve retornar `404 Not Found`.

### Atualizar quantidades

Use `GET /api/orders/{id}` para obter o ID de cada item. Em `PUT /api/orders/{id}`, envie todos os itens existentes com suas quantidades novas:

```json
{
  "items": [
    {
      "itemId": "ID_DO_ITEM_RETORNADO_PELO_GET",
      "quantity": 2
    }
  ]
}
```

O PUT atualiza quantidades. Ele exige que a lista contenha os mesmos itens do pedido; não adiciona nem remove itens individuais.

### Remover pedido

Use `DELETE /api/orders/{id}` com um ID existente. A resposta esperada é `204 No Content`. A remoção não pode ser desfeita pela API.

## Estrutura do fluxo

- `POST`, `PUT` e `DELETE` são comandos encaminhados pelo MediatR para os handlers da Application.
- O Entity Framework Core grava as mudanças no SQL Server.
- O writer do MongoDB atualiza a projeção usada pelas consultas.
- `GET` encaminha consultas aos handlers e ao repositório MongoDB.

## Observações

- SQL Server e MongoDB precisam estar acessíveis para os fluxos da API.
- A gravação no SQL Server e a atualização no MongoDB são operações separadas; não há uma transação única entre os dois bancos.
- Se uma gravação SQL for concluída e a atualização no Mongo falhar, confira se o pedido foi gravado antes de repetir o POST, para evitar duplicatas.
- A interface React depende de a API estar ativa e de o CORS permitir a origem local do Vite.