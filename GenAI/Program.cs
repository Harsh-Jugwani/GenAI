using GenAI_With_AgentFramework.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// 1. Configuration
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// 2. DI container
var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);
services.AddHttpClient();
services.AddTransient<WeatherTool>();

var provider = services.BuildServiceProvider();

// 3. Create tool instance FROM DI
var weatherTool = provider.GetRequiredService<WeatherTool>();

// 4. Chat client
var chatClient = new OllamaChatClient(
    new Uri("http://20.20.20.186:11434/"),
    modelId: "llama3.2:3b");

// 5. Agent
var agent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant running locally via Ollama. Use tools when needed.",
    tools: [
        AIFunctionFactory.Create(weatherTool.GetWeatherAsync) 
    ]);

// 6. Run
var result = await agent.RunAsync("What is the weather of indore?");
Console.WriteLine(result);