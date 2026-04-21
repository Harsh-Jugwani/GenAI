using System;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var chatClient = new OllamaChatClient(
    new Uri("http://20.20.20.186:11434/"),
    modelId: "llama3.2:3b");


AIAgent agent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant running locally via Ollama.");

AgentSession session = await agent.CreateSessionAsync();

while (true)
{
    Console.Write("User: ");
    string userInput = Console.ReadLine();
    if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
        break;
    
    await foreach(var update in agent.RunStreamingAsync(userInput, session))
    {
        Console.Write(update);
    }
}

