# Claude Hooks Configuration

This directory contains hooks that automatically run commands at specific points during Claude Code interactions.

## Available Hooks

### post-edit.json
**Triggers:** After editing any `.cs` file
**Action:** Runs `dotnet build --no-restore -v quiet`
**Purpose:** Verifies the solution still compiles after code changes
**Output:** Only shown on error (to minimize noise)

### post-write.json
**Triggers:** After writing any new `.cs` file
**Action:** Runs `dotnet build --no-restore -v quiet`
**Purpose:** Verifies new files compile correctly
**Output:** Only shown on error

### pre-commit.json
**Triggers:** Before creating a git commit
**Action:** Runs `dotnet test --no-build --verbosity minimal`
**Purpose:** Ensures all tests pass before committing
**Output:** Always shown
**Blocking:** Will prevent commit if tests fail

## Manual Verification

If you need to manually verify build/tests:

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Build and run tests
dotnet build && dotnet test
```

## Disabling Hooks

To temporarily disable hooks, set the environment variable:
```bash
export CLAUDE_HOOKS_ENABLED=false
```

Or run commands with `--no-hooks` flag where supported.

## Adding New Hooks

Create a new `.json` file in this directory with the structure:

```json
{
  "name": "Hook Name",
  "description": "What this hook does",
  "pattern": "**/*.ext",  // Optional: file pattern filter
  "command": "cd \"${WORKSPACE_ROOT}\" && your-command",
  "on": "event-name",     // edit, write, commit, etc.
  "timeout": 120000,      // Milliseconds
  "show_output": "always" | "on_error" | "never",
  "blocking": true | false
}
```

## Troubleshooting

### Hook Not Running
- Check file permissions (should be readable)
- Verify JSON syntax is valid
- Check pattern matches the file being edited

### Build/Test Taking Too Long
- Hooks use `--no-restore` to skip NuGet restore
- Consider increasing `timeout` value
- Use `-v quiet` or `--verbosity minimal` to reduce output

### False Positives
- If build fails due to incomplete work, you can ignore hook errors
- Hooks are advisory for edit/write, blocking only for commit
