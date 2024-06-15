using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace LostFindingApi.Models.DTO.MLDTOs
{
    public class CompareTextDTO
    {
        public string find_object { get; set; }

        public List<string> founded_objects { get; set; }
    }
}
