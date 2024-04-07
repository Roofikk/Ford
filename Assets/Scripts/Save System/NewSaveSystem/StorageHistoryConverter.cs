using Ford.SaveSystem.Data;
using Ford.SaveSystem.Ver2;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

public class StorageHistoryConverter : CustomCreationConverter<StorageAction>
{
    public override StorageAction Create(Type objectType)
    {
        throw new NotImplementedException();
    }

    public StorageAction Create(Type objectType, JObject jObject)
    {
        var stringType = (string)jObject.Property("ActionType");
        var type = Enum.Parse<ActionType>(stringType);

        switch (type)
        {
            case ActionType.CreateHorse:
                return new StorageAction(ActionType.CreateHorse, new HorseBase());
            case ActionType.UpdateHorse:
                return new StorageAction(ActionType.UpdateHorse, new HorseBase());
            case ActionType.DeleteHorse:
                return new StorageAction(ActionType.DeleteHorse, new HorseBase());
            case ActionType.CreateSave:
                return new StorageAction(ActionType.CreateSave, new FullSaveInfo());
            case ActionType.UpdateSave:
                return new StorageAction(ActionType.UpdateSave, new SaveInfo());
            case ActionType.DeleteSave:
                return new StorageAction(ActionType.DeleteSave, new SaveInfo());
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
