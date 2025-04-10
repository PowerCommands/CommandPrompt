namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Contracts;

public interface IDbStorageService<T> where T : new()
{
    TIdentity InsertObject<TIdentity>(T storeObject);
    TIdentity UpdateObject<TIdentity>(T storeObject, Func<T, bool> match);
    bool DeleteObject<TIdentity>(Func<T, bool> match);
    T GetObject(Func<T, bool> match);
    List<T> GetObjects(Func<List<T>, bool> where);
}