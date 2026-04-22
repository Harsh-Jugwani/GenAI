using GenAI_With_AgentFramework.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAI_With_AgentFramework.Tools
{
    public class SearchKnowledgeTool
    {
        private readonly QdrantService _qdrant;
        private readonly EmbeddingService _embedding;

        public SearchKnowledgeTool(QdrantService qdrant, EmbeddingService embedding)
        {
            _qdrant = qdrant;
            _embedding = embedding;
        }

        [Description("Use this to answer user questions from stored knowledge")]
        public async Task<string> Search(string query)
        {
            var vector = await _embedding.GenerateAsync(query);

            var results = await _qdrant.SearchAsync(vector);

            return string.Join("\n", results);
        }
    }
}
