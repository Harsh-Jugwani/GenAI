using GenAI_With_AgentFramework.Services;
using GenAI_With_AgentFramework.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // 1. Configuration
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        // 2. Service Collection (DI)
        var services = new ServiceCollection();

        // Ollama Chat Client
        services.AddSingleton(new OllamaChatClient(
            new Uri(configuration["ChatClient:Endpoint"]),
            modelId: configuration["ChatClient:ChatModel"]));

        // Ollama Embedding Client
        services.AddSingleton(new OllamaEmbeddingGenerator(
            new Uri(configuration["ChatClient:Endpoint"]),
            modelId: configuration["ChatClient:EmbeddingModel"]));

        // Qdrant Client
        services.AddSingleton(new QdrantClient(
            host: configuration["Qdrant:Host"],
            port: 6334,
            https: true,
            apiKey: configuration["Qdrant:ApiKey"]
        ));

        // Custom Services
        services.AddSingleton<QdrantService>();
        services.AddSingleton<EmbeddingService>();

        // Tools
        services.AddSingleton<IngestDocumentTool>();
        services.AddSingleton<SearchKnowledgeTool>();

        // Build Provider
        var provider = services.BuildServiceProvider();

        // Resolve dependencies
        var chatClient = provider.GetRequiredService<OllamaChatClient>();

        var ingestTool = provider.GetRequiredService<IngestDocumentTool>();
        var searchTool = provider.GetRequiredService<SearchKnowledgeTool>();


        
        // 3. Create Agent
        AIAgent agent = chatClient.AsAIAgent(
            instructions: """
You are an intelligent RAG assistant.

STRICT RULES:
1. If user provides a file path → MUST call IngestDocumentTool
2. If user asks a question → MUST call SearchKnowledgeTool FIRST
3. Use retrieved context to answer
4. If no data found → say "I don't have enough knowledge"
""",
            name: "LocalAssistant",
            tools:
            [
                AIFunctionFactory.Create(ingestTool.Ingest),
                AIFunctionFactory.Create(searchTool.Search)
            ]);

        // 4. Chat Loop
        Console.WriteLine("🤖 Chatbot started (type 'exit' to quit)\n");

        while (true)
        {
            Console.Write("You: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            try
            {
                var response = await agent.RunAsync(input);

                Console.WriteLine($"\nBot: {response}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }
}