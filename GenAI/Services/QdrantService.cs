using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GenAI_With_AgentFramework.Services
{
    public class QdrantService
    {
        private readonly QdrantClient _client;
        private const string Collection = "test2";

        public QdrantService(QdrantClient client)
        {
            _client = client;
        }

        public async Task UpsertAsync(string text, float[] vector)
        {
            try
            {
                await _client.UpsertAsync(
                collectionName: Collection,
                points: new[]
                {
            new PointStruct
            {
                Id = Guid.NewGuid(),
                Vectors = vector.ToArray(),
                Payload =
                {
                    ["text"] = text
                }
            }
                }
            );
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<List<string>> SearchAsync(float[] vector)
        {
            var result = await _client.SearchAsync(Collection, vector, limit: 3);

            return result.Select(r => r.Payload["text"].ToString()).ToList();
        }
    }
}
