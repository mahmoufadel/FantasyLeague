using McpServer;
using McpServer.Services;
using ModelContextProtocol.Server;
using MCPServer = ModelContextProtocol.Server.McpServer;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║      World Cup MCP Server                                 ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Create the store and seed data
var store = new WorldCupStore();
store.SeedSampleData();

Console.WriteLine("Starting MCP server with stdio transport...");
Console.WriteLine();

// Create server options
var options = new McpServerOptions
{
    ServerInfo = new() { Name = "worldcup-mcp-server", Version = "1.0.0" },
    Capabilities = new() { Tools = new() }
};

// Create server with stdio transport
var transport = new StdioServerTransport("worldcup-mcp-server");
var server = await MCPServer.CreateAsync(transport, options);

// Log available tools
Console.WriteLine("Available tools:");
Console.WriteLine("  - get_all_matches");
Console.WriteLine("  - get_match_by_id");
Console.WriteLine("  - get_matches_by_stage");
Console.WriteLine("  - get_matches_by_team");
Console.WriteLine("  - create_match");
Console.WriteLine("  - update_match_score");
Console.WriteLine("  - start_match");
Console.WriteLine("  - add_user_message");
Console.WriteLine("  - get_messages_for_match");
Console.WriteLine("  - get_messages_by_user");
Console.WriteLine("  - delete_message");
Console.WriteLine("  - create_replay_session");
Console.WriteLine("  - start_replay");
Console.WriteLine("  - pause_replay");
Console.WriteLine("  - stop_replay");
Console.WriteLine("  - reset_replay");
Console.WriteLine("  - get_next_message");
Console.WriteLine("  - get_replay_status");
Console.WriteLine("  - get_tournament_stats");
Console.WriteLine();
Console.WriteLine("Waiting for MCP requests...");

// Run the server
await server.StartAsync();

// Keep running until cancelled
await Task.Delay(Timeout.Infinite);
