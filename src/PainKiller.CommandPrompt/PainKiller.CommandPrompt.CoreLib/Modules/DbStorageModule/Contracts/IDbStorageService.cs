namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Contracts;

public interface IDbStorageService<T> where T : new()
{
    TIdentity InsertObject<TIdentity>(T storeObject);
    TIdentity UpdateObject<TIdentity>(T storeObject, Func<T, bool> match);
    bool DeleteObject(Func<T, bool> match);
    List<T> GetAll();
}