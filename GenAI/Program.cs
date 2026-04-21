using System;
using GenAI_With_AgentFramework.Constants;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var chatClient = new OllamaChatClient(
    new Uri("http://20.20.20.186:11434/"),
    modelId: "llama3.2:3b");

AIAgent agent = chatClient.AsAIAgent(instructions: PromptBuilder.BuildSeniorDevPrompt(), name: "SeniorDev");


var history = new List<ChatMessage>();

while (true)
{
    Console.WriteLine("Ask a question (or type 'exit' to quit):");
    string input = Console.ReadLine();

    if (input?.ToLower() == "exit")
        break;

    history.Add(new ChatMessage(ChatRole.User, input));

    Console.WriteLine("Answer:");

    var response = "";

    await foreach (var update in agent.RunStreamingAsync(history))
    {
        Console.Write(update);
        response += update;
    }

    Console.WriteLine();

    // Add assistant response to history
    history.Add(new ChatMessage(ChatRole.Assistant, response));
}