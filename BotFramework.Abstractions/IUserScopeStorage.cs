namespace BotFramework.Abstractions;

public interface IUserScopeStorage
{
    IFeatureCollection Get(long usedId);
}