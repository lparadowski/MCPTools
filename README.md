# MCP Project Tools

An [MCP (Model Context Protocol)](https://modelcontextprotocol.io/) server that gives AI assistants like Claude the ability to interact with project management tools — Jira, Trello, Confluence, Miro, Azure DevOps, and Chrome browser automation.

Built with .NET 10 and Docker. Each integration runs as an independent microservice behind a unified MCP interface.

## What It Does

Connect this MCP server to Claude (or any MCP-compatible client) and it can:

- **Jira** — Create, update, search, and transition issues. Manage sprints, boards, labels, comments, issue links, and worklogs. Track user activity.
- **Trello** — Manage boards, cards, lists, labels, and comments.
- **Confluence** — Create and edit pages and spaces. Search using CQL (Confluence Query Language).
- **Miro** — List boards and manage sticky notes (create, update, delete with color/position support).
- **Azure DevOps** — Manage projects, work items, boards, sprints, and teams.
- **Chrome** — List open tabs, navigate to URLs, and capture screenshots via Chrome DevTools Protocol.

## Architecture

```
┌─────────────────┐      ┌──────────────────────┐
│   Claude CLI    │◄────►│     MCP Server        │
│  (MCP Client)   │ stdio│  (tool definitions)   │
└─────────────────┘      └──┬───┬───┬───┬───┬────┘
                            │   │   │   │   │
                 HTTP       │   │   │   │   │
              ┌─────────────┘   │   │   │   └─────────────┐
              ▼                 ▼   ▼   ▼                 ▼
       ┌────────────┐  ┌──────────┐ ┌──────────┐  ┌─────────────┐
       │ Trello API │  │ Jira API │ │ Miro API │  │ Confluence  │
       │   :5001    │  │  :5004   │ │  :5002   │  │  API :5003  │
       └────────────┘  └──────────┘ └──────────┘  └─────────────┘
              ┌──────────────┐  ┌──────────┐
              │ AzureDevOps  │  │   Seq    │
              │  API :5005   │  │  :5341   │
              └──────────────┘  └──────────┘
                   Docker containers
```

Each backend service follows **clean architecture** with four layers:
- **Api** — ASP.NET Core controllers, request/response DTOs, exception handling
- **Application** — Business logic, service interfaces, error types (FluentResults)
- **Domain** — Entities and value objects (no dependencies)
- **Infrastructure** — External API clients, mapping (Mapster)

Shared code (exception handling, result extensions, error types) lives in the **Shared** project.

Chrome automation runs in-process via Chrome DevTools Protocol (WebSocket).

**Seq** provides centralized structured logging across all services at `http://localhost:5341`.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker & Docker Compose](https://docs.docker.com/get-docker/)
- [Chrome](https://www.google.com/chrome/) (for browser automation tools — must be launched with `--remote-debugging-port=9222`)
- API credentials for the services you want to use (see [Configuration](#configuration))

## Getting Started

### 1. Clone and configure

```bash
git clone https://github.com/lparadowski/McpProjectTools.git
cd McpProjectTools
cp .env.example .env
```

Edit `.env` with your API credentials (see [Configuration](#configuration)).

### 2. Start the backend services

```bash
docker compose up -d
```

This starts the API containers:
| Service       | Port |
|---------------|------|
| Trello API    | 5001 |
| Miro API      | 5002 |
| Confluence    | 5003 |
| Jira API      | 5004 |
| Azure DevOps  | 5005 |
| Seq           | 5341 |

### 3. Build the MCP server

```bash
dotnet build src/McpServer/McpServer.csproj
```

### 4. Connect to Claude

Create a `.mcp.json` file in your project root (or configure your MCP client):

```json
{
  "mcpServers": {
    "project-tools": {
      "command": "/path/to/McpProjectTools/src/McpServer/bin/Debug/net10.0/McpServer"
    }
  }
}
```

On Windows, use the `.exe` extension:
```json
{
  "mcpServers": {
    "project-tools": {
      "command": "C:\\path\\to\\McpProjectTools\\src\\McpServer\\bin\\Debug\\net10.0\\McpServer.exe"
    }
  }
}
```

## Configuration

All credentials are passed via environment variables through the `.env` file.

### Required Variables

Only configure the services you plan to use:

```env
# Trello (https://trello.com/power-ups/admin — generate API key and token)
TRELLO_API_KEY=your_api_key
TRELLO_TOKEN=your_token

# Jira (https://id.atlassian.com/manage-profile/security/api-tokens)
JIRA_BASE_URL=https://your-domain.atlassian.net
JIRA_EMAIL=your-email@example.com
JIRA_API_TOKEN=your_api_token

# Confluence (same Atlassian API token as Jira)
CONFLUENCE_BASE_URL=https://your-domain.atlassian.net/wiki
CONFLUENCE_EMAIL=your-email@example.com
CONFLUENCE_API_TOKEN=your_api_token

# Miro (https://miro.com/app/settings/user-profile/apps — create an app and get access token)
MIRO_ACCESS_TOKEN=your_access_token

# Azure DevOps (https://dev.azure.com — generate a Personal Access Token)
AZURE_DEVOPS_ORGANIZATION=your_organization
AZURE_DEVOPS_PAT=your_personal_access_token
```

### Corporate Proxy / SSL Issues

If you're behind a corporate proxy that intercepts HTTPS with a custom root CA, the Trello container may fail with SSL certificate errors. Set this in your `.env`:

```env
DISABLE_SSL_VALIDATION=true
```

This disables SSL certificate validation for outbound API calls from the Trello container. **Only use this in development/corporate environments — never in production.**

### Chrome Setup

Chrome must be running with remote debugging enabled:

```bash
# macOS
open -a "Google Chrome" --args --remote-debugging-port=9222

# Windows
chrome.exe --remote-debugging-port=9222

# Linux
google-chrome --remote-debugging-port=9222
```

## Available Tools

### Jira (20+ tools)
| Tool | Description |
|------|-------------|
| `list_jira_projects` | List all projects |
| `get_jira_project` | Get project by key or ID |
| `create_jira_issue` | Create issue (Epic, Story, Task, Bug, Subtask) with custom fields |
| `get_jira_issue` | Get issue details including status, assignee, labels |
| `update_jira_issue` | Update summary, description, custom fields |
| `delete_jira_issue` | Permanently delete an issue |
| `search_jira_issues` | Search using JQL |
| `get_jira_transitions` | Get available workflow transitions |
| `transition_jira_issue` | Move issue through workflow |
| `get_jira_comments` | Get all comments on an issue |
| `add_jira_comment` | Add a comment |
| `add_jira_label` / `remove_jira_label` | Manage labels |
| `assign_jira_issue` | Assign or unassign |
| `link_jira_issues` | Link two issues (Blocks, Relates, etc.) |
| `list_jira_boards` / `get_jira_board` | Board management |
| `list_jira_sprints` | List sprints for a board |
| `move_issues_to_sprint` | Move issues to a sprint |
| `log_jira_work` | Log time spent on an issue |
| `get_jira_worklogs` | Get worklog entries |
| `search_jira_user_worklogs` | Search worklogs by user and date range |
| `update_jira_worklog` / `delete_jira_worklog` | Manage worklogs |
| `get_jira_user_activity` | Get user activity (transitions, comments, changes) |
| `list_jira_fields` | List all fields with IDs for custom field lookup |

### Trello (14 tools)
| Tool | Description |
|------|-------------|
| `list_trello_boards` | List all boards |
| `create_trello_board` / `get_trello_board` | Create or get board details |
| `archive_trello_board` / `delete_trello_board` | Archive or delete a board |
| `list_trello_cards` | List all cards on a board |
| `create_trello_card` / `get_trello_card` | Create or get card details |
| `update_trello_card` / `move_trello_card` | Update or move cards between lists |
| `archive_trello_card` / `delete_trello_card` | Archive or delete a card |
| `get_trello_card_comments` / `post_trello_comment` | Read and post comments |
| `list_trello_board_labels` / `create_trello_board_label` | Manage board labels |
| `add_label_to_trello_card` / `remove_label_from_trello_card` | Label cards |

### Confluence (9 tools)
| Tool | Description |
|------|-------------|
| `list_confluence_spaces` / `get_confluence_space` | Browse spaces |
| `create_confluence_space` / `delete_confluence_space` | Create or delete spaces |
| `list_confluence_pages` / `get_confluence_page` | Browse pages |
| `create_confluence_page` / `update_confluence_page` | Create or update pages |
| `delete_confluence_page` | Delete a page |
| `search_confluence` | Search using CQL |

### Miro (5 tools)
| Tool | Description |
|------|-------------|
| `list_miro_boards` / `get_miro_board` | Browse boards |
| `list_miro_sticky_notes` | Get all sticky notes from a board |
| `create_miro_sticky_note` | Create with content, color, shape, position |
| `update_miro_sticky_note` / `delete_miro_sticky_note` | Update or delete |

### Azure DevOps (10+ tools)
| Tool | Description |
|------|-------------|
| `list_azure_projects` / `get_azure_project` | Browse projects |
| `create_azure_work_item` | Create work items (Epic, Story, Task, Bug) |
| `get_azure_work_item` / `update_azure_work_item` | Get or update work items |
| `delete_azure_work_item` | Delete a work item |
| `search_azure_work_items` | Search using WIQL |
| `add_azure_work_item_comment` / `get_azure_work_item_comments` | Manage comments |
| `list_azure_boards` | List boards |
| `list_azure_sprints` | List sprints |
| `list_azure_teams` | List teams |

### Chrome (3 tools)
| Tool | Description |
|------|-------------|
| `list_chrome_tabs` | List open browser tabs |
| `navigate_to_url` | Navigate a tab to a URL |
| `take_tab_screenshot` | Capture a screenshot of a tab |

## Project Structure

```
src/
├── McpServer/                  # MCP server entry point
│   ├── Program.cs              # HttpClient + MCP registration
│   └── Tools/                  # Tool definitions (thin HTTP delegates)
│       ├── JiraTools.cs
│       ├── TrelloTools.cs
│       ├── ConfluenceTools.cs
│       ├── MiroTools.cs
│       ├── AzureDevOpsTools.cs
│       └── ChromeTools.cs
│
├── Jira/                       # Clean architecture
│   ├── Jira.Api/               # Controllers, DTOs, Dockerfile
│   ├── Jira.Application/       # Services, interfaces
│   ├── Jira.Domain/            # Entities, value objects
│   └── Jira.Infrastructure/    # Jira REST API client
│
├── Trello/                     # Same layered pattern
├── Confluence/                 # Same layered pattern
├── Miro/                       # Same layered pattern
├── AzureDevOps/                # Same layered pattern
│
├── Shared/
│   ├── Shared.Api/             # GlobalExceptionHandler, ResultExtensions
│   └── Shared.Application/     # Error types, content chunking
│
└── Chrome/
    └── Chrome.Core/            # Chrome DevTools Protocol client
```

## Tech Stack

- **.NET 10** — Runtime and SDK
- **ASP.NET Core** — Backend API services
- **ModelContextProtocol** (0.8.0-preview) — MCP SDK for .NET
- **Docker Compose** — Service orchestration
- **FluentResults** — Railway-oriented error handling
- **Mapster** — Object mapping
- **Serilog** — Structured logging
- **Seq** — Centralized log viewer
- **Manatee.Trello** — Trello SDK

## Contributing

Contributions are welcome! The codebase follows clean architecture consistently across all services, so use any existing integration as a template when adding new ones.

## License

[MIT](LICENSE)
