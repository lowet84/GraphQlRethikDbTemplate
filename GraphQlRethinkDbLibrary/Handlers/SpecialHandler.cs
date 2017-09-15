using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GraphQlRethinkDbLibrary.Handlers
{
    public abstract class SpecialHandler
    {
        public abstract string Path { get; }
        public abstract Task Process(HttpContext context);
    }
}
