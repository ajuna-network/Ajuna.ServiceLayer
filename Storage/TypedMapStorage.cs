using Ajuna.NetApi;
using Ajuna.NetApi.Model.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ajuna.ServiceLayer.Extensions;
using Serilog;
using Ajuna.NetApi.Model.Types.Base;

namespace Ajuna.ServiceLayer.Storage
{
    public class TypedMapStorage<T> where T : IType, new()
    {
        internal string Identifier { get; private set; }
        public Dictionary<string, T> Dictionary { get; private set; }

        public TypedMapStorage(string identifier)
        {
            Identifier = identifier;
        }

        public async Task InitializeAsync(SubstrateClient client, string module, string moduleItem)
        {
            Dictionary = await client.GetStorageDictAsync<T>(module, moduleItem);
            Log.Information("loaded storage map {storage} with {count} entries", moduleItem, Dictionary.Count);
        }

        public bool ContainsKey(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        public T Get(string key)
        {
            return Dictionary[key];
        }

        public void Update(string key, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                Dictionary.Remove(key);
                Log.Debug($"[{Identifier}] item {{key}} was deleted.", key);
            }
            else
            {
                var iType = new T();
                iType.Create(data);

                if (Dictionary.ContainsKey(key))
                {
                    Dictionary[key] = iType;
                    Log.Debug($"[{Identifier}] item {{key}} was updated.", key);
                }
                else
                {
                    Dictionary.Add(key, iType);
                    Log.Debug($"[{Identifier}] item {{key}} was created.", key);
                }
            }
        }
    }
}
