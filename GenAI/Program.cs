using System;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var chatClient = new OllamaChatClient(
    new Uri("http://20.20.20.186:11434/"),
    modelId: "llama3.2:3b");

AIAgent agent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant running locally via Ollama.");

Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));

