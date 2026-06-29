# MusicaStreaming - API

API REST de um serviço de streaming de música com sistema de pagamento, desenvolvida em .NET 9 aplicando Domain-Driven Design (DDD), arquitetura em camadas, CQRS e os princípios SOLID.

Projeto acadêmico. O front-end que consome esta API está em um repositório separado: [MusicaStreaming-Front](https://github.com/Natanaelssousa/MusicaStreaming-Front).

---

## Sumário

- [Funcionalidades](#funcionalidades)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Configuração](#configuração)
- [Banco de dados (migrations)](#banco-de-dados-migrations)
- [Executando a API](#executando-a-api)
- [Autenticação (JWT)](#autenticação-jwt)
- [Endpoints](#endpoints)
- [Testes](#testes)
- [CI/CD](#cicd)
- [Considerações e melhorias futuras](#considerações-e-melhorias-futuras)

---

## Funcionalidades

- **Conta**: criação de conta, confirmação de e-mail, login com JWT e alteração de senha.
- **Assinatura**: escolha de plano (Free, Premium, Family), criação, consulta e renovação.
- **Autorização de transação**: tenta autorizar uma transação para um comerciante, validando o estado da conta (usuário ativo, assinatura não expirada) e o intervalo mínimo de 5 minutos desde a última transação autorizada no mesmo cartão. Transações negadas são persistidas para auditoria.
- **Notificação de transação**: ao autorizar, o dono do cartão e o comerciante são notificados de forma assíncrona, através de um serviço em segundo plano (BackgroundService).
- **Catálogo**: busca de músicas (por título ou gênero) e de bandas/artistas (por nome).
- **Favoritos**: favoritar e desfavoritar músicas e bandas, e listar os favoritos do usuário.

---

## Arquitetura

O projeto segue uma arquitetura em camadas com DDD, organizada em cinco projetos. A direção das dependências aponta sempre para o domínio, que não depende de nada externo.

```
+---------------------------+
|   MusicaStreaming.Api     |  Camada de apresentação (Controllers, Swagger, JWT)
+---------------------------+
            |
            v
+---------------------------+
| MusicaStreaming.Application |  Camada de aplicação/serviços
|  (Handlers MediatR, CQRS,   |
|   interfaces de repositório)|
+---------------------------+
            |
            v
+---------------------------+
|  MusicaStreaming.Domain   |  Camada de negócio (Agregados, Value Objects, regras)
+---------------------------+
            ^
            |
+---------------------------+
| MusicaStreaming.Infrastructure | Camada de acesso a dados (EF Core, repositórios, DbContext)
+---------------------------+

MusicaStreaming.Tests  ->  testes unitários (xUnit) de domínio e casos de uso
```

Pontos de destaque do desenho:

- **Inversão de dependência**: as interfaces dos repositórios ficam em `Application/Abstractions`, e a `Infrastructure` as implementa. Assim a camada de aplicação depende de abstrações, não da infraestrutura concreta.
- **CQRS com MediatR**: cada caso de uso é um Command ou Query com seu Handler.
- **Domínio rico**: as regras de negócio (validações, autorização de transação, estados) vivem nos agregados e Value Objects, não nos handlers.

---

## Tecnologias

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- MediatR (CQRS)
- JWT (autenticação)
- Swagger / OpenAPI (documentação e testes manuais)
- xUnit, Moq, FluentAssertions (testes)
- GitHub Actions (CI/CD)

---

## Pré-requisitos

- .NET 9 SDK
- SQL Server (a edição Express é suficiente)
- Visual Studio 2022 (ou outra IDE com suporte a .NET 9) / .NET CLI

---

## Configuração

### Connection string

No projeto `MusicaStreaming.Api`, em `appsettings.json`, configure a string de conexão com o seu SQL Server. Substitua os placeholders pelos seus dados — **não versione credenciais reais**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=MusicaStreaming;User Id=SEU_USUARIO;Password=SUA_SENHA;Encrypt=false;TrustServerCertificate=true;"
}
```

Se o seu SQL usa autenticação integrada do Windows, a alternativa é `Trusted_Connection=true` no lugar de usuário/senha.

### JWT

Ainda em `appsettings.json`, configure as opções do JWT. A `SecretKey` deve ter no mínimo 32 caracteres:

```json
"JwtSettings": {
  "SecretKey": "SUA_CHAVE_SECRETA_COM_NO_MINIMO_32_CARACTERES",
  "Issuer": "MusicaStreaming",
  "Audience": "MusicaStreamingUsers",
  "ExpirationMinutes": 60
}
```

---

## Banco de dados (migrations)

O esquema do banco é gerenciado por migrations do EF Core. Para criar/atualizar o banco, use o Package Manager Console no Visual Studio (ou a CLI `dotnet ef`):

```
Update-Database -Project MusicaStreaming.Infrastructure -StartupProject MusicaStreaming.Api
```

As migrations já existentes cobrem a criação inicial e as evoluções (comerciante na transação, notificações e favoritos de artista). Ao subir a aplicação, dados de exemplo (artistas e músicas) são inseridos automaticamente (seed) caso o catálogo esteja vazio.

---

## Executando a API

Pelo Visual Studio: defina `MusicaStreaming.Api` como projeto de inicialização e rode (F5).

Pela CLI:

```
dotnet run --project MusicaStreaming.Api
```

A API sobe por padrão em `http://localhost:5057` (a porta pode variar conforme a configuração de launch; confira a saída do console). A documentação interativa fica em `http://localhost:5057/swagger`.

---

## Autenticação (JWT)

O fluxo é: registrar -> confirmar e-mail -> login. O login retorna um token JWT, que deve ser enviado nas chamadas protegidas no cabeçalho:

```
Authorization: Bearer <token>
```

No Swagger, use o botão **Authorize** e cole apenas o token (o Swagger adiciona o prefixo `Bearer`).

Observações de implementação relevantes:

- O token é assinado e validado com `Encoding.UTF8` em ambos os lados.
- Em ambiente de desenvolvimento, o redirecionamento HTTPS é tratado com cuidado, pois um redirect HTTP→HTTPS descarta o cabeçalho `Authorization`. Use a mesma origem (http ou https) entre cliente e API durante os testes.

---

## Endpoints

Principais rotas (todas sob `/api`):

**Usuário**
- `POST /usuario/registrar` — cria conta
- `POST /usuario/confirmar-email-publico` — confirma e-mail (sem autenticação)
- `POST /usuario/login` — autentica e retorna o token
- `POST /usuario/alterar-senha` — altera a senha (autenticado)

**Assinatura** (autenticado)
- `GET /assinatura` — consulta a assinatura atual do usuário
- `POST /assinatura/criar` — cria assinatura (planType: 1=Free, 2=Premium, 3=Family)
- `POST /assinatura/renovar` — renova a assinatura

**Transação** (autenticado)
- `POST /transacao/autorizar` — autoriza uma transação (cartaoId, valor, comerciante)

**Catálogo**
- `GET /catalog/musicas/buscar?termo=` — busca músicas por título ou gênero
- `POST /catalog/musica/criar` — cria música
- `POST /catalog/musica/favoritar` — favorita/desfavorita música (autenticado)
- `GET /catalog/favoritos` — lista músicas favoritas (autenticado)

**Artista**
- `GET /artista/buscar?termo=` — busca bandas/artistas por nome
- `POST /artista/criar` — cria artista
- `POST /artista/favoritar` — favorita/desfavorita banda (autenticado)
- `GET /artista/favoritos` — lista bandas favoritas (autenticado)

**Saúde**
- `GET /saude` — health check

---

## Testes

A suíte é composta por testes unitários (xUnit) que cobrem os agregados de domínio, os Value Objects e os handlers de casos de uso. Os handlers são testados com repositórios mockados (Moq), sem dependência de banco de dados.

Rodar pela CLI:

```
dotnet test
```

Ou pelo Test Explorer do Visual Studio.

---

## CI/CD

O repositório possui um pipeline no GitHub Actions (`.github/workflows/`) que executa a cada push e pull request na branch principal:

- **CI**: restaura dependências, compila em Release e roda todos os testes. Se algum teste falha, o pipeline falha.
- **CD**: somente se o CI passar, faz o `dotnet publish` da API e publica os binários como artefato de build (pacote pronto para implantação).

---

## Considerações e melhorias futuras

Alguns pontos foram conscientemente delimitados ao escopo acadêmico:

- **Performance da busca**: as buscas usam `Contains` (traduzido para `LIKE '%termo%'`), adequado ao volume do projeto. Há índices nas colunas de nome/título. Para escala maior, a evolução natural seria o Full-Text Search do SQL Server.
- **CD**: o pipeline empacota a aplicação (publish + artefato). O passo final de deploy em um ambiente real (ex.: Azure App Service) exigiria configurar um destino e credenciais, fora do escopo atual.
- **Testes de integração**: a suíte é de testes unitários. Testes de integração contra um banco real seriam a evolução natural para cobrir o mapeamento EF e as queries de ponta a ponta.
- **Autorização de endpoints de cadastro**: os endpoints de criação de artista e música não exigem autenticação no escopo atual; em um cenário real seriam restritos a um perfil administrativo.
