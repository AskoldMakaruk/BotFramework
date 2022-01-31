﻿using BotFramework.Services.Clients;

namespace BotFramework.Services.Factories;

public class FeatureCollectionFactory
{
    public IFeatureCollection Get(long userId)
    {
        var featureCollection = new FeatureCollection
        {
            [typeof(PriorityUpdateConsumer)] = new PriorityUpdateConsumer()
        };
        
        return featureCollection;
    }
}