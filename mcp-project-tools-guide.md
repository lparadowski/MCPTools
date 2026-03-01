# MCP Project Tools Server

A personal MCP server that exposes Trello, Jira, and Miro APIs to Claude CLI, enabling project management directly from your development workflow.

## Architecture Overview

```
Claude CLI (stdio)
    ↓ spawns on demand
MCP Server (.NET console app)
    ↓ HTTP calls
┌─────────────────────────────────────────┐
│  Docker Compose (always running)        │
│  ├── trello-api    → localhost:5001     │
│  ├── jira-api      → localhost:5002     │
│  └── miro-api      → localhost:5003     │
└─────────────────────────────────────────┘
    ↓ external calls
┌─────────────────────────────────────────┐
│  Trello API  │  Jira Cloud  │  Miro API │
└─────────────────────────────────────────┘
```

Claude CLI spawns the MCP server via stdio — no manual startup needed. The APIs run as Docker containers in the background. Each API is a thin wrapper that handles authentication and maps clean internal endpoints to the external vendor APIs.

---

## Project Structure

```
mcp-project-tools/
├── src/
│   ├── McpServer/                          # MCP server (console app, stdio)
│   │   ├── Program.cs
│   │   ├── Tools/
│   │   │   ├── TrelloTools.cs
│   │   │   ├── MiroTools.cs
│   │   │   ├── AiGuessrTools.cs
│   │   │   ├── AiGuessrSearchTools.cs
│   │   │   ├── CountryTools.cs
│   │   │   └── Chrome/
│   │   │       ├── ChromeTools.cs
│   │   │       ├── ChromeDevTools.cs
│   │   │       └── ChromeTab.cs
│   │   ├── appsettings.json
│   │   └── McpServer.csproj
│   │
│   ├── Trello/
│   │   ├── Trello.Api/                     # Presentation layer
│   │   │   ├── Controllers/
│   │   │   │   └── CardsController.cs
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   ├── Dockerfile
│   │   │   └── Trello.Api.csproj
│   │   ├── Trello.Application/             # Use cases, interfaces, DTOs
│   │   │   ├── Cards/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── CreateCard/
│   │   │   │   │   │   ├── CreateCardCommand.cs
│   │   │   │   │   │   └── CreateCardCommandHandler.cs
│   │   │   │   │   └── UpdateCard/
│   │   │   │   │       ├── UpdateCardCommand.cs
│   │   │   │   │       └── UpdateCardCommandHandler.cs
│   │   │   │   └── Queries/
│   │   │   │       └── ListCards/
│   │   │   │           ├── ListCardsQuery.cs
│   │   │   │           └── ListCardsQueryHandler.cs
│   │   │   ├── Interfaces/
│   │   │   │   └── ITrelloClient.cs
│   │   │   ├── DependencyInjection.cs
│   │   │   └── Trello.Application.csproj
│   │   ├── Trello.Domain/                  # Entities, value objects
│   │   │   ├── Entities/
│   │   │   │   ├── Card.cs
│   │   │   │   └── BoardList.cs
│   │   │   ├── ValueObjects/
│   │   │   │   └── Label.cs
│   │   │   └── Trello.Domain.csproj
│   │   └── Trello.Infrastructure/          # External API integration
│   │       ├── ExternalApi/
│   │       │   ├── TrelloClient.cs
│   │       │   └── Models/
│   │       │       ├── TrelloCardResponse.cs
│   │       │       └── TrelloListResponse.cs
│   │       ├── Mapping/
│   │       │   └── TrelloMappingProfile.cs
│   │       ├── DependencyInjection.cs
│   │       └── Trello.Infrastructure.csproj
│   │
│   ├── Jira/
│   │   ├── Jira.Api/                       # Presentation layer
│   │   │   ├── Controllers/
│   │   │   │   └── IssuesController.cs
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   ├── Dockerfile
│   │   │   └── Jira.Api.csproj
│   │   ├── Jira.Application/              # Use cases, interfaces, DTOs
│   │   │   ├── Issues/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── CreateIssue/
│   │   │   │   │   │   ├── CreateIssueCommand.cs
│   │   │   │   │   │   └── CreateIssueCommandHandler.cs
│   │   │   │   │   └── CreateEpicWithStories/
│   │   │   │   │       ├── CreateEpicWithStoriesCommand.cs
│   │   │   │   │       └── CreateEpicWithStoriesCommandHandler.cs
│   │   │   │   └── Queries/
│   │   │   │       └── SearchIssues/
│   │   │   │           ├── SearchIssuesQuery.cs
│   │   │   │           └── SearchIssuesQueryHandler.cs
│   │   │   ├── Interfaces/
│   │   │   │   └── IJiraClient.cs
│   │   │   ├── DependencyInjection.cs
│   │   │   └── Jira.Application.csproj
│   │   ├── Jira.Domain/                    # Entities, value objects, enums
│   │   │   ├── Entities/
│   │   │   │   └── Issue.cs
│   │   │   ├── Enums/
│   │   │   │   ├── IssueType.cs
│   │   │   │   └── Priority.cs
│   │   │   └── Jira.Domain.csproj
│   │   └── Jira.Infrastructure/            # External API integration
│   │       ├── ExternalApi/
│   │       │   ├── JiraClient.cs
│   │       │   └── Models/
│   │       │       ├── JiraIssueResponse.cs
│   │       │       └── JiraCreatePayload.cs
│   │       ├── Mapping/
│   │       │   └── JiraMappingProfile.cs
│   │       ├── DependencyInjection.cs
│   │       └── Jira.Infrastructure.csproj
│   │
│   └── Miro/
│       ├── Miro.Api/                       # Presentation layer
│       │   ├── Controllers/
│       │   │   ├── BoardController.cs
│       │   │   └── DiagramController.cs
│       │   ├── Program.cs
│       │   ├── appsettings.json
│       │   ├── Dockerfile
│       │   └── Miro.Api.csproj
│       ├── Miro.Application/              # Use cases, interfaces, DTOs
│       │   ├── Boards/
│       │   │   └── Commands/
│       │   │       └── CreateEpicBoard/
│       │   │           ├── CreateEpicBoardCommand.cs
│       │   │           └── CreateEpicBoardCommandHandler.cs
│       │   ├── Diagrams/
│       │   │   └── Commands/
│       │   │       └── RenderAndAttach/
│       │   │           ├── RenderAndAttachCommand.cs
│       │   │           └── RenderAndAttachCommandHandler.cs
│       │   ├── Interfaces/
│       │   │   ├── IMiroClient.cs
│       │   │   └── IMermaidRenderer.cs
│       │   ├── DependencyInjection.cs
│       │   └── Miro.Application.csproj
│       ├── Miro.Domain/                    # Entities, value objects
│       │   ├── Entities/
│       │   │   ├── Frame.cs
│       │   │   ├── StickyNote.cs
│       │   │   └── DiagramImage.cs
│       │   ├── ValueObjects/
│       │   │   ├── Position.cs
│       │   │   └── Color.cs
│       │   └── Miro.Domain.csproj
│       └── Miro.Infrastructure/            # External API + Mermaid rendering
│           ├── ExternalApi/
│           │   ├── MiroClient.cs
│           │   └── Models/
│           │       ├── MiroStickyPayload.cs
│           │       ├── MiroFramePayload.cs
│           │       └── MiroImagePayload.cs
│           ├── Rendering/
│           │   └── MermaidCliRenderer.cs
│           ├── Mapping/
│           │   └── MiroMappingProfile.cs
│           ├── DependencyInjection.cs
│           └── Miro.Infrastructure.csproj
│
├── docker-compose.yml
├── .env                                    # API keys (gitignored)
├── McpProjectTools.sln
└── README.md
```

---

## Phase 1: API Wrappers

Each API is a minimal ASP.NET Web API that handles auth and exposes clean internal endpoints. These run in Docker so they're always available.

### 1.1 Trello API

**External dependency:** Trello REST API (`https://api.trello.com/1/`)

**Get your credentials:**
- Go to https://trello.com/power-ups/admin → create a Power-Up → get API Key
- Generate a Token from the same page
- Get your Board ID from the board URL: `https://trello.com/b/{BOARD_ID}/...`

**Endpoints to implement:**

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/cards` | Create a card |
| GET | `/api/cards` | List cards (optional list filter) |
| PUT | `/api/cards/{id}` | Update a card |
| GET | `/api/lists` | Get all lists on the board |
| POST | `/api/cards/{id}/labels` | Add label to card |

**Domain layer:**

```csharp
// Trello.Domain/Entities/Card.cs
public class Card
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ListName { get; set; }
    public string Url { get; set; }
    public List<Label> Labels { get; set; } = new();
}

// Trello.Domain/ValueObjects/Label.cs
public record Label(string Id, string Name, string Color);
```

**Application layer — interface (defined here, implemented in Infrastructure):**

```csharp
// Trello.Application/Interfaces/ITrelloClient.cs
public interface ITrelloClient
{
    Task<Card> CreateCardAsync(string title, string description, string listId, string[]? labelIds);
    Task<IReadOnlyList<Card>> GetCardsAsync(string? listId = null);
    Task<Card> UpdateCardAsync(string cardId, string? title, string? description, string? listId);
    Task<string> ResolveListIdAsync(string listName);
    Task<string[]> ResolveLabelIdsAsync(string[] labelNames);
}
```

**Application layer — command/handler:**

```csharp
// Trello.Application/Cards/Commands/CreateCard/CreateCardCommand.cs
public record CreateCardCommand(
    string Title,
    string Description,
    string List = "Backlog",
    string[]? Labels = null
);

// Trello.Application/Cards/Commands/CreateCard/CreateCardCommandHandler.cs
public class CreateCardCommandHandler
{
    private readonly ITrelloClient _client;

    public CreateCardCommandHandler(ITrelloClient client) => _client = client;

    public async Task<Card> HandleAsync(CreateCardCommand command)
    {
        var listId = await _client.ResolveListIdAsync(command.List);

        string[]? labelIds = command.Labels != null
            ? await _client.ResolveLabelIdsAsync(command.Labels)
            : null;

        return await _client.CreateCardAsync(
            command.Title,
            command.Description,
            listId,
            labelIds);
    }
}
```

**Infrastructure layer — external API client:**

```csharp
// Trello.Infrastructure/ExternalApi/TrelloClient.cs
public class TrelloClient : ITrelloClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _token;
    private readonly string _boardId;

    public TrelloClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://api.trello.com/1/");
        _apiKey = config["Trello:ApiKey"];
        _token = config["Trello:Token"];
        _boardId = config["Trello:BoardId"];
    }

    private string Auth => $"key={_apiKey}&token={_token}";

    public async Task<Card> CreateCardAsync(
        string title, string description, string listId, string[]? labelIds)
    {
        var payload = new
        {
            name = title,
            desc = description,
            idList = listId,
            idLabels = labelIds != null ? string.Join(",", labelIds) : null
        };

        var response = await _http.PostAsJsonAsync($"cards?{Auth}", payload);
        response.EnsureSuccessStatusCode();
        var trelloCard = await response.Content.ReadFromJsonAsync<TrelloCardResponse>();
        return trelloCard.ToDomain();
    }

    public async Task<string> ResolveListIdAsync(string listName)
    {
        var lists = await _http.GetFromJsonAsync<List<TrelloListResponse>>(
            $"boards/{_boardId}/lists?{Auth}");
        return lists.First(l =>
            l.Name.Equals(listName, StringComparison.OrdinalIgnoreCase)).Id;
    }

    public async Task<string[]> ResolveLabelIdsAsync(string[] labelNames)
    {
        var labels = await _http.GetFromJsonAsync<List<TrelloLabelResponse>>(
            $"boards/{_boardId}/labels?{Auth}");
        return labelNames
            .Select(name => labels.First(l =>
                l.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Id)
            .ToArray();
    }
}
```

**Infrastructure layer — external API response models + mapping:**

```csharp
// Trello.Infrastructure/ExternalApi/Models/TrelloCardResponse.cs
public record TrelloCardResponse(string Id, string Name, string Desc, string Url, string ShortUrl)
{
    public Card ToDomain() => new()
    {
        Id = Id, Name = Name, Description = Desc, Url = Url
    };
}

public record TrelloListResponse(string Id, string Name);
public record TrelloLabelResponse(string Id, string Name, string Color);
```

**Infrastructure DI registration:**

```csharp
// Trello.Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddTrelloInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<ITrelloClient, TrelloClient>();
        return services;
    }
}

// Trello.Application/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddTrelloApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateCardCommandHandler>();
        services.AddScoped<UpdateCardCommandHandler>();
        services.AddScoped<ListCardsQueryHandler>();
        return services;
    }
}
```

**API controller (thin — delegates to handler):**

```csharp
// Trello.Api/Controllers/CardsController.cs
[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly CreateCardCommandHandler _createHandler;
    private readonly ListCardsQueryHandler _listHandler;

    public CardsController(
        CreateCardCommandHandler createHandler,
        ListCardsQueryHandler listHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardCommand command)
    {
        var card = await _createHandler.HandleAsync(command);
        return Ok(new { card.Id, card.Url, card.Name });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? list = null)
    {
        var cards = await _listHandler.HandleAsync(new ListCardsQuery(list));
        return Ok(cards);
    }
}
```

**API Program.cs:**

```csharp
// Trello.Api/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTrelloApplication();
builder.Services.AddTrelloInfrastructure(builder.Configuration);

var app = builder.Build();
app.MapControllers();
app.Run();
```

### 1.2 Jira API

**External dependency:** Jira Cloud REST API (`https://{your-domain}.atlassian.net/rest/api/3/`)

**Get your credentials:**
- Go to https://id.atlassian.com/manage-profile/security/api-tokens → create token
- Your email + API token are used for Basic auth
- Get your Project Key from the Jira project settings

**Endpoints to implement:**

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/issues` | Create an issue |
| GET | `/api/issues` | Search/list issues (JQL) |
| PUT | `/api/issues/{key}` | Update an issue |
| GET | `/api/issue-types` | List available issue types |
| POST | `/api/issues/{key}/link` | Link issues together |
| POST | `/api/issues/epic-with-stories` | Create epic + child stories |

**Domain layer:**

```csharp
// Jira.Domain/Entities/Issue.cs
public class Issue
{
    public string Id { get; set; }
    public string Key { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public IssueType Type { get; set; }
    public Priority? Priority { get; set; }
    public string? ParentKey { get; set; }
    public List<string> Labels { get; set; } = new();
    public string Url { get; set; }
}

// Jira.Domain/Enums/IssueType.cs
public enum IssueType { Epic, Story, Task, Bug, SubTask }

// Jira.Domain/Enums/Priority.cs
public enum Priority { Highest, High, Medium, Low, Lowest }
```

**Application layer — interface:**

```csharp
// Jira.Application/Interfaces/IJiraClient.cs
public interface IJiraClient
{
    Task<Issue> CreateIssueAsync(
        string summary, string description, IssueType type,
        Priority? priority, string[]? labels, string? parentKey);
    Task<IReadOnlyList<Issue>> SearchAsync(string jql);
    Task<Issue> UpdateIssueAsync(string key, string? summary, string? description);
    Task LinkIssuesAsync(string inwardKey, string outwardKey, string linkType);
}
```

**Application layer — command/handler:**

```csharp
// Jira.Application/Issues/Commands/CreateIssue/CreateIssueCommand.cs
public record CreateIssueCommand(
    string Title,
    string Description,
    string IssueType = "Story",
    string? Priority = null,
    string[]? Labels = null,
    string? ParentKey = null
);

// Jira.Application/Issues/Commands/CreateIssue/CreateIssueCommandHandler.cs
public class CreateIssueCommandHandler
{
    private readonly IJiraClient _client;

    public CreateIssueCommandHandler(IJiraClient client) => _client = client;

    public async Task<Issue> HandleAsync(CreateIssueCommand command)
    {
        var issueType = Enum.Parse<IssueType>(command.IssueType, ignoreCase: true);
        var priority = command.Priority != null
            ? Enum.Parse<Priority>(command.Priority, ignoreCase: true)
            : (Priority?)null;

        return await _client.CreateIssueAsync(
            command.Title,
            command.Description,
            issueType,
            priority,
            command.Labels,
            command.ParentKey);
    }
}

// Jira.Application/Issues/Commands/CreateEpicWithStories/CreateEpicWithStoriesCommand.cs
public record CreateEpicWithStoriesCommand(
    string EpicTitle,
    string EpicDescription,
    List<StoryInput> Stories
);

public record StoryInput(string Title, string Description);

// Jira.Application/Issues/Commands/CreateEpicWithStories/CreateEpicWithStoriesCommandHandler.cs
public class CreateEpicWithStoriesCommandHandler
{
    private readonly IJiraClient _client;

    public CreateEpicWithStoriesCommandHandler(IJiraClient client) => _client = client;

    public async Task<EpicWithStoriesResult> HandleAsync(CreateEpicWithStoriesCommand command)
    {
        // Create the epic first
        var epic = await _client.CreateIssueAsync(
            command.EpicTitle, command.EpicDescription,
            IssueType.Epic, null, null, null);

        // Create stories under the epic
        var stories = new List<Issue>();
        foreach (var story in command.Stories)
        {
            var issue = await _client.CreateIssueAsync(
                story.Title, story.Description,
                IssueType.Story, null, null,
                parentKey: epic.Key);
            stories.Add(issue);
        }

        return new EpicWithStoriesResult(epic, stories);
    }
}

public record EpicWithStoriesResult(Issue Epic, List<Issue> Stories);
```

**Infrastructure layer — external API client:**

```csharp
// Jira.Infrastructure/ExternalApi/JiraClient.cs
public class JiraClient : IJiraClient
{
    private readonly HttpClient _http;
    private readonly string _projectKey;

    public JiraClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.BaseAddress = new Uri(
            $"https://{config["Jira:Domain"]}.atlassian.net/rest/api/3/");

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{config["Jira:Email"]}:{config["Jira:ApiToken"]}"));
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);

        _projectKey = config["Jira:ProjectKey"];
    }

    public async Task<Issue> CreateIssueAsync(
        string summary, string description, IssueType type,
        Priority? priority, string[]? labels, string? parentKey)
    {
        var payload = new JiraCreatePayload
        {
            Fields = new JiraFields
            {
                Project = new { Key = _projectKey },
                Summary = summary,
                Description = BuildAdfDescription(description),
                IssueType = new { Name = type.ToString() },
                Labels = labels ?? Array.Empty<string>(),
                Priority = priority.HasValue ? new { Name = priority.ToString() } : null,
                Parent = parentKey != null ? new { Key = parentKey } : null
            }
        };

        var response = await _http.PostAsJsonAsync("issue", payload);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JiraIssueResponse>();
        return result.ToDomain(_http.BaseAddress.Host);
    }

    private static object BuildAdfDescription(string text) => new
    {
        type = "doc",
        version = 1,
        content = new[]
        {
            new
            {
                type = "paragraph",
                content = new[] { new { type = "text", text } }
            }
        }
    };
}
```

**API layer (thin controller + Program.cs):**

```csharp
// Jira.Api/Controllers/IssuesController.cs
[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly CreateIssueCommandHandler _createHandler;
    private readonly CreateEpicWithStoriesCommandHandler _epicHandler;
    private readonly SearchIssuesQueryHandler _searchHandler;

    public IssuesController(
        CreateIssueCommandHandler createHandler,
        CreateEpicWithStoriesCommandHandler epicHandler,
        SearchIssuesQueryHandler searchHandler)
    {
        _createHandler = createHandler;
        _epicHandler = epicHandler;
        _searchHandler = searchHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIssueCommand command)
    {
        var issue = await _createHandler.HandleAsync(command);
        return Ok(new { issue.Key, issue.Url, issue.Summary });
    }

    [HttpPost("epic-with-stories")]
    public async Task<IActionResult> CreateEpicWithStories(
        [FromBody] CreateEpicWithStoriesCommand command)
    {
        var result = await _epicHandler.HandleAsync(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var issues = await _searchHandler.HandleAsync(new SearchIssuesQuery(query));
        return Ok(issues);
    }
}
```

### 1.3 Miro API

**External dependency:** Miro REST API (`https://api.miro.com/v2/`)

**Get your credentials:**
- Go to https://miro.com/app/settings/user-profile/apps → create app
- Set scopes: `boards:read`, `boards:write`
- Install to your team and get the access token
- Get your Board ID from the board URL: `https://miro.com/app/board/{BOARD_ID}/`

**Endpoints to implement:**

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/boards/{id}/epic` | Create full epic board (frames + stickies) |
| POST | `/api/boards/{id}/stickies` | Create individual sticky notes |
| POST | `/api/boards/{id}/frames` | Create a frame |
| POST | `/api/boards/{id}/diagrams` | Render Mermaid and attach as image |

**Domain layer:**

```csharp
// Miro.Domain/Entities/Frame.cs
public class Frame
{
    public string Id { get; set; }
    public string Title { get; set; }
    public Position Position { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<StickyNote> StickyNotes { get; set; } = new();
}

// Miro.Domain/Entities/StickyNote.cs
public class StickyNote
{
    public string Id { get; set; }
    public string Content { get; set; }
    public Color Color { get; set; }
    public Position Position { get; set; }
}

// Miro.Domain/Entities/DiagramImage.cs
public class DiagramImage
{
    public string Id { get; set; }
    public string Title { get; set; }
    public Position Position { get; set; }
}

// Miro.Domain/ValueObjects/Position.cs
public record Position(int X, int Y);

// Miro.Domain/ValueObjects/Color.cs
public record Color(string Hex)
{
    public static Color Yellow => new("#fff9b1");
    public static Color Blue => new("#d0e7ff");
    public static Color Green => new("#d5f5e3");
    public static Color Pink => new("#f8d7da");
}
```

**Application layer — interfaces:**

```csharp
// Miro.Application/Interfaces/IMiroClient.cs
public interface IMiroClient
{
    Task<Frame> CreateFrameAsync(string boardId, string title, Position position, int width, int height);
    Task<StickyNote> CreateStickyAsync(string boardId, string content, Color color, Position position, string? parentFrameId);
    Task<DiagramImage> UploadImageAsync(string boardId, byte[] imageBytes, string title, Position position);
}

// Miro.Application/Interfaces/IMermaidRenderer.cs
public interface IMermaidRenderer
{
    Task<byte[]> RenderToImageAsync(string mermaidSyntax);
}
```

**Application layer — command/handlers:**

```csharp
// Miro.Application/Boards/Commands/CreateEpicBoard/CreateEpicBoardCommand.cs
public record CreateEpicBoardCommand(
    string EpicName,
    List<FeatureInput> Features,
    string? BoardId = null
);

public record FeatureInput(string Name, string[] Stories, string? Color = null);

// Miro.Application/Boards/Commands/CreateEpicBoard/CreateEpicBoardCommandHandler.cs
public class CreateEpicBoardCommandHandler
{
    private readonly IMiroClient _miro;

    public CreateEpicBoardCommandHandler(IMiroClient miro) => _miro = miro;

    public async Task<EpicBoardResult> HandleAsync(CreateEpicBoardCommand command)
    {
        var boardId = command.BoardId; // Falls back to default in infra layer
        var results = new List<FeatureFrameResult>();
        var xOffset = 0;
        const int frameWidth = 600;
        const int frameSpacing = 100;
        const int stickySize = 150;
        const int stickyPadding = 20;

        foreach (var feature in command.Features)
        {
            var frameHeight = 200 + (feature.Stories.Length * (stickySize + stickyPadding));
            var color = feature.Color != null ? new Color(feature.Color) : Color.Yellow;

            // Create frame for the feature
            var frame = await _miro.CreateFrameAsync(
                boardId, feature.Name,
                new Position(xOffset, 0),
                frameWidth, frameHeight);

            // Create sticky notes inside the frame
            var stickies = new List<StickyNote>();
            for (int i = 0; i < feature.Stories.Length; i++)
            {
                var position = new Position(xOffset, 100 + (i * (stickySize + stickyPadding)));
                var sticky = await _miro.CreateStickyAsync(
                    boardId, feature.Stories[i], color, position, frame.Id);
                stickies.Add(sticky);
            }

            results.Add(new FeatureFrameResult(feature.Name, frame.Id, stickies));
            xOffset += frameWidth + frameSpacing;
        }

        return new EpicBoardResult(command.EpicName, results,
            $"https://miro.com/app/board/{boardId}/");
    }
}

public record EpicBoardResult(string EpicName, List<FeatureFrameResult> Features, string BoardUrl);
public record FeatureFrameResult(string Name, string FrameId, List<StickyNote> Stickies);

// Miro.Application/Diagrams/Commands/RenderAndAttach/RenderAndAttachCommand.cs
public record RenderAndAttachCommand(
    string MermaidSyntax,
    string Title,
    string? BoardId = null,
    int? X = null,
    int? Y = null
);

// Miro.Application/Diagrams/Commands/RenderAndAttach/RenderAndAttachCommandHandler.cs
public class RenderAndAttachCommandHandler
{
    private readonly IMermaidRenderer _renderer;
    private readonly IMiroClient _miro;

    public RenderAndAttachCommandHandler(IMermaidRenderer renderer, IMiroClient miro)
    {
        _renderer = renderer;
        _miro = miro;
    }

    public async Task<DiagramImage> HandleAsync(RenderAndAttachCommand command)
    {
        var imageBytes = await _renderer.RenderToImageAsync(command.MermaidSyntax);
        var position = new Position(command.X ?? 0, command.Y ?? 0);

        return await _miro.UploadImageAsync(
            command.BoardId, imageBytes, command.Title, position);
    }
}
```

**Infrastructure layer — Miro API client:**

```csharp
// Miro.Infrastructure/ExternalApi/MiroClient.cs
public class MiroClient : IMiroClient
{
    private readonly HttpClient _http;
    private readonly string _defaultBoardId;

    public MiroClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://api.miro.com/v2/");
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", config["Miro:AccessToken"]);
        _defaultBoardId = config["Miro:DefaultBoardId"];
    }

    public async Task<Frame> CreateFrameAsync(
        string boardId, string title, Position position, int width, int height)
    {
        var id = boardId ?? _defaultBoardId;
        var payload = new
        {
            data = new { title, type = "freeform" },
            position = new { x = position.X, y = position.Y },
            geometry = new { width, height }
        };

        var response = await _http.PostAsJsonAsync($"boards/{id}/frames", payload);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<MiroFrameResponse>();
        return result.ToDomain();
    }

    public async Task<StickyNote> CreateStickyAsync(
        string boardId, string content, Color color, Position position, string? parentFrameId)
    {
        var id = boardId ?? _defaultBoardId;
        var payload = new
        {
            data = new { content, shape = "square" },
            style = new { fillColor = color.Hex },
            position = new { x = position.X, y = position.Y },
            geometry = new { width = 150, height = 150 },
            parent = parentFrameId != null ? new { id = parentFrameId } : null
        };

        var response = await _http.PostAsJsonAsync($"boards/{id}/sticky_notes", payload);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<MiroStickyResponse>();
        return result.ToDomain();
    }

    public async Task<DiagramImage> UploadImageAsync(
        string boardId, byte[] imageBytes, string title, Position position)
    {
        var id = boardId ?? _defaultBoardId;
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(imageBytes), "resource", $"{title}.png");
        content.Add(new StringContent(JsonSerializer.Serialize(new
        {
            title,
            position = new { x = position.X, y = position.Y }
        })), "data");

        var response = await _http.PostAsync($"boards/{id}/images", content);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<MiroImageResponse>();
        return result.ToDomain();
    }
}
```

**Infrastructure layer — Mermaid renderer:**

```csharp
// Miro.Infrastructure/Rendering/MermaidCliRenderer.cs
public class MermaidCliRenderer : IMermaidRenderer
{
    public async Task<byte[]> RenderToImageAsync(string mermaidSyntax)
    {
        var tempInput = Path.GetTempFileName() + ".mmd";
        var tempOutput = Path.GetTempFileName() + ".png";

        try
        {
            await File.WriteAllTextAsync(tempInput, mermaidSyntax);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "mmdc",
                    Arguments = $"-i \"{tempInput}\" -o \"{tempOutput}\" " +
                                $"-t neutral -w 1200 -b transparent",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new InvalidOperationException($"Mermaid render failed: {error}");
            }

            return await File.ReadAllBytesAsync(tempOutput);
        }
        finally
        {
            File.Delete(tempInput);
            File.Delete(tempOutput);
        }
    }
}
```

**Dockerfile for Miro API (includes mermaid-cli):**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Install Node.js and mermaid-cli for diagram rendering
RUN apt-get update && apt-get install -y curl gnupg \
    && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs chromium \
    && npm install -g @mermaid-js/mermaid-cli \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

ENV PUPPETEER_EXECUTABLE_PATH=/usr/bin/chromium

COPY publish/ /app
WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Miro.Api.dll"]
```

---

## Phase 2: Docker Compose

```yaml
# docker-compose.yml
services:
  trello-api:
    build:
      context: ./src/Trello
      dockerfile: Trello.Api/Dockerfile
    ports:
      - "5001:8080"
    environment:
      - Trello__ApiKey=${TRELLO_API_KEY}
      - Trello__Token=${TRELLO_TOKEN}
      - Trello__BoardId=${TRELLO_BOARD_ID}
    restart: unless-stopped

  jira-api:
    build:
      context: ./src/Jira
      dockerfile: Jira.Api/Dockerfile
    ports:
      - "5002:8080"
    environment:
      - Jira__Domain=${JIRA_DOMAIN}
      - Jira__Email=${JIRA_EMAIL}
      - Jira__ApiToken=${JIRA_API_TOKEN}
      - Jira__ProjectKey=${JIRA_PROJECT_KEY}
    restart: unless-stopped

  miro-api:
    build:
      context: ./src/Miro
      dockerfile: Miro.Api/Dockerfile
    ports:
      - "5003:8080"
    environment:
      - Miro__AccessToken=${MIRO_ACCESS_TOKEN}
      - Miro__DefaultBoardId=${MIRO_BOARD_ID}
    restart: unless-stopped
```

**Environment file:**

```bash
# .env (gitignored)
TRELLO_API_KEY=your_key
TRELLO_TOKEN=your_token
TRELLO_BOARD_ID=your_board_id

JIRA_DOMAIN=yourcompany
JIRA_EMAIL=you@company.com
JIRA_API_TOKEN=your_token
JIRA_PROJECT_KEY=PROJ

MIRO_ACCESS_TOKEN=your_token
MIRO_BOARD_ID=your_board_id
```

**Standard Dockerfile for Trello and Jira APIs:**

```dockerfile
# e.g. src/Trello/Trello.Api/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Trello.Api/Trello.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY --from=build /app/publish /app
WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Trello.Api.dll"]
```

**Start everything:**

```bash
docker-compose up -d
```

---

## Phase 3: MCP Server

The MCP server is a .NET console app that Claude CLI spawns via stdio. It defines tools that call the Docker-hosted APIs.

### 3.1 Project Setup

```bash
dotnet new console -n McpServer
cd McpServer
dotnet add package ModelContextProtocol --prerelease
dotnet add package Microsoft.Extensions.Hosting
```

### 3.2 Program.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("TrelloApi", c => c.BaseAddress = new Uri("http://localhost:5001"));
builder.Services.AddHttpClient("JiraApi", c => c.BaseAddress = new Uri("http://localhost:5002"));
builder.Services.AddHttpClient("MiroApi", c => c.BaseAddress = new Uri("http://localhost:5003"));

builder.Services
    .AddMcpServer()
    .WithStdioTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
```

### 3.3 Trello Tools

```csharp
// Tools/TrelloTools.cs
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public static class TrelloTools
{
    [McpServerTool("create_trello_card",
        Description = "Create a Trello card. Use when the user wants to track a bug, task, feature, or improvement.")]
    public static async Task<string> CreateCard(
        IHttpClientFactory httpFactory,
        [Description("Card title")] string title,
        [Description("Detailed description")] string description,
        [Description("List name: Backlog, To Do, In Progress, Done")] string list = "Backlog",
        [Description("Comma-separated labels: bug, feature, tech-debt, improvement")] string? labels = null)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var payload = new { title, description, list, labels = labels?.Split(',') };
        var response = await http.PostAsJsonAsync("/api/cards", payload);
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool("list_trello_cards",
        Description = "List Trello cards, optionally filtered by list name.")]
    public static async Task<string> ListCards(
        IHttpClientFactory httpFactory,
        [Description("Filter by list name")] string? list = null)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var query = list != null ? $"?list={list}" : "";
        var response = await http.GetAsync($"/api/cards{query}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool("update_trello_card",
        Description = "Update an existing Trello card's title, description, or move it to another list.")]
    public static async Task<string> UpdateCard(
        IHttpClientFactory httpFactory,
        [Description("Card ID")] string cardId,
        [Description("New title")] string? title = null,
        [Description("New description")] string? description = null,
        [Description("Move to list")] string? list = null)
    {
        var http = httpFactory.CreateClient("TrelloApi");
        var payload = new { title, description, list };
        var response = await http.PutAsJsonAsync($"/api/cards/{cardId}", payload);
        return await response.Content.ReadAsStringAsync();
    }
}
```

### 3.4 Jira Tools

```csharp
// Tools/JiraTools.cs
[McpServerToolType]
public static class JiraTools
{
    [McpServerTool("create_jira_issue",
        Description = "Create a Jira issue. Supports Epic, Story, Task, Bug, and Sub-task types.")]
    public static async Task<string> CreateIssue(
        IHttpClientFactory httpFactory,
        [Description("Issue summary/title")] string title,
        [Description("Detailed description")] string description,
        [Description("Issue type: Epic, Story, Task, Bug, Sub-task")] string issueType = "Story",
        [Description("Priority: Highest, High, Medium, Low, Lowest")] string? priority = null,
        [Description("Comma-separated labels")] string? labels = null,
        [Description("Parent issue key for sub-tasks or stories under epics (e.g. PROJ-123)")] string? parentKey = null)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var payload = new
        {
            title,
            description,
            issueType,
            priority,
            labels = labels?.Split(','),
            parentKey
        };
        var response = await http.PostAsJsonAsync("/api/issues", payload);
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool("search_jira_issues",
        Description = "Search Jira issues using JQL or simple text query.")]
    public static async Task<string> SearchIssues(
        IHttpClientFactory httpFactory,
        [Description("JQL query or search text")] string query)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var response = await http.GetAsync($"/api/issues?query={Uri.EscapeDataString(query)}");
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool("create_jira_epic_with_stories",
        Description = "Create a Jira Epic and its child Stories in one operation. Use for planning features or workstreams.")]
    public static async Task<string> CreateEpicWithStories(
        IHttpClientFactory httpFactory,
        [Description("Epic title")] string epicTitle,
        [Description("Epic description")] string epicDescription,
        [Description("JSON array of stories, e.g. [{\"title\":\"...\",\"description\":\"...\"}]")] string storiesJson)
    {
        var http = httpFactory.CreateClient("JiraApi");
        var payload = new { epicTitle, epicDescription, stories = storiesJson };
        var response = await http.PostAsJsonAsync("/api/issues/epic-with-stories", payload);
        return await response.Content.ReadAsStringAsync();
    }
}
```

### 3.5 Miro Tools

```csharp
// Tools/MiroTools.cs
[McpServerToolType]
public static class MiroTools
{
    [McpServerTool("create_miro_epic_board",
        Description = "Create a visual epic breakdown on Miro with frames per feature and sticky notes for stories. Use for visual project planning.")]
    public static async Task<string> CreateEpicBoard(
        IHttpClientFactory httpFactory,
        [Description("Epic name")] string epicName,
        [Description("JSON array of features with stories, e.g. [{\"name\":\"Auth\",\"stories\":[\"Login\",\"OAuth\"],\"color\":\"#fff9b1\"}]")] string featuresJson,
        [Description("Miro board ID (uses default if not specified)")] string? boardId = null)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var payload = new { epicName, features = featuresJson, boardId };
        var response = await http.PostAsJsonAsync("/api/boards/epic", payload);
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool("create_miro_diagram",
        Description = "Render a Mermaid diagram to an image and place it on a Miro board. Use for architecture diagrams, sequence diagrams, flowcharts, class diagrams, and ERDs.")]
    public static async Task<string> CreateDiagram(
        IHttpClientFactory httpFactory,
        [Description("Mermaid syntax for the diagram")] string mermaidSyntax,
        [Description("Diagram title")] string title,
        [Description("Miro board ID (uses default if not specified)")] string? boardId = null,
        [Description("X position on board")] int? x = null,
        [Description("Y position on board")] int? y = null)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var payload = new { mermaidSyntax, title, boardId, x, y };
        var response = await http.PostAsJsonAsync("/api/boards/diagrams", payload);
        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool("add_miro_sticky_notes",
        Description = "Add individual sticky notes to a Miro board. Use for quick visual notes or brainstorming.")]
    public static async Task<string> AddStickyNotes(
        IHttpClientFactory httpFactory,
        [Description("JSON array of sticky notes, e.g. [{\"content\":\"Note text\",\"color\":\"#fff9b1\"}]")] string notesJson,
        [Description("Miro board ID (uses default if not specified)")] string? boardId = null)
    {
        var http = httpFactory.CreateClient("MiroApi");
        var payload = new { notes = notesJson, boardId };
        var response = await http.PostAsJsonAsync("/api/boards/stickies", payload);
        return await response.Content.ReadAsStringAsync();
    }
}
```

### 3.6 Country Tools

Exposes `C:\Geoguessr\countries.json` (122 countries, ~400KB of structured GeoGuessr reference data) via two tools. The file is read from disk on every call so edits are picked up immediately — no restart needed.

```csharp
// Tools/CountryTools.cs
[McpServerToolType]
public static class CountryTools
{
    [McpServerTool(Name = "search_countries")]
    // Search countries.json for countries where ALL keywords appear in a specific field or across all fields.
    // keywords: string[] — all must match (case-insensitive)
    // field: string? — e.g. "bollard", "plates", "signs", "driving_side". Null = search all fields.
    // Returns: country names + matched field content

    [McpServerTool(Name = "get_country")]
    // Retrieve the full countries.json entry for a specific country (case-insensitive match).
    // Returns: pretty-printed JSON of the entire country object
}
```

**No HTTP dependency** — these tools read directly from disk, unlike other tools that call Docker-hosted APIs.

**Future:** Caching planned for production (deserialize once, invalidate via `FileSystemWatcher`). See Trello ticket.

---

## Phase 4: Claude CLI Configuration

### 4.1 Register the MCP Server

Add to your global settings or per-project:

**Option A: Global** (`~/.claude/settings.json`)

```json
{
  "mcpServers": {
    "project-tools": {
      "command": "dotnet",
      "args": ["run", "--project", "/home/leo/tools/mcp-project-tools/src/McpServer"]
    }
  }
}
```

**Option B: Published binary** (faster cold start)

```bash
cd src/McpServer
dotnet publish -c Release -o bin/publish --self-contained
```

```json
{
  "mcpServers": {
    "project-tools": {
      "command": "/home/leo/tools/mcp-project-tools/src/McpServer/bin/publish/McpServer"
    }
  }
}
```

**Option C: Per-project** (`.claude/settings.json` in your repo)

Same format as above. Use this if only specific projects need these tools.

### 4.2 Verify Setup

```bash
# Make sure APIs are running
docker-compose up -d

# Start Claude CLI and check tools are detected
claude

# In Claude CLI, ask:
> What tools do you have available?
# Should list create_trello_card, create_jira_issue, create_miro_epic_board, etc.
```

---

## Phase 5: End-to-End Tests

These are conversational tests to run in Claude CLI to verify each integration works.

### Test 1: Trello — Create a Card

```
You: Create a Trello card for a bug: the login page crashes when 
     the email field is empty. Put it in the Backlog list with 
     a "bug" label.
```

**Expected:** Claude calls `create_trello_card` and returns a Trello card URL.

### Test 2: Jira — Create an Issue

```
You: Create a Jira Story: "As a student, I want to see my semester 
     timetable so I can plan my week." Priority Medium, label it 
     "mvp".
```

**Expected:** Claude calls `create_jira_issue` and returns a Jira issue key (e.g. `PROJ-42`).

### Test 3: Jira — Create Epic with Stories

```
You: Plan an epic for the Student Planner authentication system. 
     Break it into stories covering login, registration, password 
     reset, OAuth with university SSO, and session management.
```

**Expected:** Claude calls `create_jira_epic_with_stories`, creates the epic and 5 stories underneath it.

### Test 4: Miro — Create Epic Board

```
You: Create a Miro board for the auth epic. Group the stories by 
     feature area: credential management, SSO integration, and 
     session handling. Use different colors for each group.
```

**Expected:** Claude calls `create_miro_epic_board` with three features, each containing relevant stories as sticky notes.

### Test 5: Miro — Render Architecture Diagram

```
You: Create a sequence diagram showing the OAuth login flow with 
     the university SSO and put it on the Miro board.
```

**Expected:** Claude generates Mermaid syntax, calls `create_miro_diagram`, and a rendered diagram appears on the board.

### Test 6: Combined — Full Planning Flow

```
You: I want to plan a new "Notifications" feature for Student Planner. 
     Break it into stories, create a Jira epic with all stories, 
     set up a visual board on Miro with the story breakdown, and 
     generate an architecture diagram showing how notifications will 
     flow through the system.
```

**Expected:** Claude chains multiple tool calls:
1. `create_jira_epic_with_stories` → Jira epic + stories
2. `create_miro_epic_board` → visual board with stickies
3. `create_miro_diagram` → architecture diagram on the board

Returns links to Jira epic and Miro board.

### Test 7: Cross-Tool — Bug to Ticket with Context

```
You: [while reviewing code in Claude CLI]
     This null reference on line 47 needs fixing. Create a bug ticket 
     in both Trello and Jira for it.
```

**Expected:** Claude creates cards/issues in both systems using context from the code it's currently viewing.

---

## Implementation Order

1. **Trello API + MCP tool** — simplest API, fastest to get working end-to-end
2. **Jira API + MCP tools** — slightly more complex auth and payload structure
3. **Miro stickies + epic board** — visual layout, more API calls per operation
4. **Miro diagrams** — requires mermaid-cli in the container, adds rendering step
5. **Combined flows** — no new code needed, just verify Claude chains tools correctly

---

## Troubleshooting

**MCP server not detected by Claude CLI:**
- Check the path in settings.json is correct
- Run the MCP server manually: `dotnet run --project src/McpServer` — should start without errors
- Ensure `ModelContextProtocol` NuGet package is installed

**API containers not starting:**
- Check `.env` file has all required variables
- Run `docker-compose logs trello-api` to see errors
- Verify API keys are valid by testing with curl:
  ```bash
  curl http://localhost:5001/api/lists
  ```

**Mermaid rendering fails:**
- Check Chromium is installed in the Miro API container
- Verify `PUPPETEER_EXECUTABLE_PATH` is set correctly
- Test rendering directly:
  ```bash
  docker exec miro-api mmdc -i /tmp/test.mmd -o /tmp/test.png
  ```

**Tool calls failing silently:**
- MCP server logs go to stderr — check Claude CLI's output
- Add logging to your MCP tools to trace HTTP calls
- Test APIs directly with curl before involving the MCP layer

---

## Future Extensions

- **Code Search MCP Tool** — embed your codebases with Voyage/Qodo and expose semantic search
- **Slack integration** — notify channels when epics/stories are created
- **Confluence/Wiki** — auto-generate technical design docs from the planning session
- **GitHub Issues** — alternative to Trello/Jira for open-source projects
- **Bidirectional sync** — read Trello/Jira state back into Claude CLI for status updates
