# Codex configuration

- `../AGENTS.md`: project instructions adapted from `CLAUDE.md`. Shared coding and testing rules should be updated in both files.
- `config.toml`: the binlog MCP server provided by the enabled dotnet-msbuild plugin.
- `rules/project.rules`: reusable command permissions translated from `.claude/settings.local.json`.
- `../.agents/skills/`: copies of the skills from the seven enabled dotnet-agent-skills plugins, with supporting files preserved. No dependency on a user's Claude cache paths.
- `skill-sources.json`: source plugin, version and skill inventory for this migration.
- `dotnet-skills-LICENSE`: upstream license for the copied skills.

This is a one-time migration. Claude configuration remains intact. Codex does not consume Claude's `enabledPlugins` setting or `Bash(...)` permission syntax. Copied skills use Codex's repository skill discovery instead.

Claude-specific LSP integrations and subagent definitions are not installed by copying skills. The binlog MCP integration is configured separately. Broad Claude `Read(...)` grants, one-off scratch commands, and certificate operations are not translated; filesystem access remains governed by Codex's sandbox and managed policy.

Restart Codex in this repository to load the instructions and skills. Project config and rules require the project to be trusted; managed policy can impose stricter permissions. No model or sandbox overrides are set here.

Conventions:
- https://learn.chatgpt.com/docs/agent-configuration/agents-md
- https://learn.chatgpt.com/docs/config-file/config-basic
- https://learn.chatgpt.com/docs/build-skills
- https://learn.chatgpt.com/docs/agent-configuration/rules
