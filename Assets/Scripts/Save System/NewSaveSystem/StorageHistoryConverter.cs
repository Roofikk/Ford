using Ford.SaveSystem;
using Ford.SaveSystem.Ver2;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

public class StorageHistoryConverter : CustomCreationConverter<StorageAction<IStorageAction>>
{
    public override StorageAction<IStorageAction> Create(Type objectType)
    {
        throw new NotImplementedException();
    }

    public StorageAction<IStorageAction> Create(Type objectType, JObject jObject)
    {
        var stringType = (string)jObject.Property("ActionType");
        var type = Enum.Parse<ActionType>(stringType);

        switch (type)
        {
            case ActionType.CreateHorse:
                return new StorageAction<IStorageAction>(ActionType.CreateHorse, new HorseBase());
            case ActionType.UpdateHorse:
                return new StorageAction<IStorageAction>(ActionType.UpdateHorse, new HorseBase());
            case ActionType.DeleteHorse:
                return new StorageAction<IStorageAction>(ActionType.DeleteHorse, new HorseBase());
            case ActionType.CreateSave:
                return new StorageAction<IStorageAction>(ActionType.CreateSave, new FullSaveInfo());
            case ActionType.UpdateSave:
                return new StorageAction<IStorageAction>(ActionType.UpdateSave, new SaveInfo());
            case ActionType.DeleteSave:
                return new StorageAction<IStorageAction>(ActionType.DeleteSave, new SaveInfo());
        }

        throw new NotImplementedException($"The action type {type} is not supported");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        var target = Create(objectType, jObject);
        serializer.Populate(jObject.CreateReader(), target);
        return target;
    }
}
