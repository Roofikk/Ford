using System.Collections.Generic;

namespace Ford.WebApi.Data
{
    public class UpdatingHorseOwnersDto
    {
        public string HorseId { get; set; }
        public IEnumerable<CreationHorseUser> HorseOwners { get; set; }
    }
}
