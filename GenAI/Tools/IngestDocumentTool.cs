using GenAI_With_AgentFramework.Services;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GenAI_With_AgentFramework.Tools
{
    public class IngestDocumentTool
    {
        private readonly QdrantService _qdrant;
        private readonly EmbeddingService _embedding;

        public IngestDocumentTool(QdrantService qdrant, EmbeddingService embedding)
        {
            _qdrant = qdrant;
            _embedding = embedding;
        }

        [Description("Use this when user provides a file path to store knowledge into memory")]
        public async Task<string> Ingest(string filePath)
        {
            if (!File.Exists(filePath))
                return "File not found";

            var text = await File.ReadAllTextAsync(filePath);
            var chunks = text
     .Split("\n\n")
     .Where(c => c.Length > 50)
     .ToList();

            foreach (var chunk in chunks)
            {
                var embedding = await _embedding.GenerateAsync(chunk);
                await _qdrant.UpsertAsync(chunk, embedding);
            }

            return "Document ingested successfully.";
        }
    }
}

