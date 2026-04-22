using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAI_With_AgentFramework.Services
{
    public class EmbeddingService
    {
        private readonly OllamaEmbeddingGenerator _client;

        public EmbeddingService(OllamaEmbeddingGenerator client)
        {
            _client = client;
        }

        public async Task<float[]> GenerateAsync(string text)
        {
            var embedding = await _client.GenerateAsync(text);
            return embedding.Vector.ToArray();
        }


    }
}
